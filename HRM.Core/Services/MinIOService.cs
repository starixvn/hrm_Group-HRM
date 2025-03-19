using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Fanex.Logging;
using HRM.Core.Models;
using Microsoft.AspNetCore.Http;

namespace HRM.Core.Services;

public interface IMinIOService
{
    Task<bool> UploadFileAsync(IFormFile file, string bucketName, string filePath);

    Task<IEnumerable<S3FileObject>> GetFileAsync(string bucketName, string directoryBucket);

    Task<string> GetFilePath(string bucketName, string filePath, string defaultFile = "");

    Task<bool> DeleteFileAsync(string bucketName, string objectName);

    Task<bool> IsFileExistsAsync(string bucketName, string filePath);
}

public class MinIOService : IMinIOService
{
    private readonly IAmazonS3 s3Client;
    private readonly AmazonStorageConfig s3Config;
    private readonly ILogger logger;

    public MinIOService(AmazonStorageConfig s3Config, ILogger logger)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = s3Config.InternalUrl,
            ForcePathStyle = true,
            UseHttp = true
        };

        s3Client = new AmazonS3Client(s3Config.AccessKey, s3Config.SecretKey, config);
        this.logger = logger;
        this.s3Config = s3Config;
    }

    public async Task<bool> UploadFileAsync(IFormFile file, string bucketName, string filePath)
    {
        try
        {
            await using var fileStream = file.OpenReadStream();
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = filePath,
                InputStream = fileStream
            };

            var response = await s3Client.PutObjectAsync(putRequest);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (AmazonS3Exception ex)
        {
            await logger.ErrorAsync($"Upload to s3 Exception. Message: {ex.Message}", ex);
            return false;
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync($"Upload to s3 Exception. Message: {ex.Message}", ex);
            return false;
        }
    }

    public async Task<bool> IsFileExistsAsync(string bucketName, string filePath)
    {
        var request = new ListObjectsV2Request
        {
            BucketName = bucketName,
            Prefix = filePath,
            MaxKeys = 1
        };

        var response = await s3Client.ListObjectsV2Async(request);
        return response.S3Objects.Any(o => o.Key == filePath);
    }

    public async Task<IEnumerable<S3FileObject>> GetFileAsync(string bucketName, string directoryBucket)
    {
        var request = new ListObjectsV2Request
        {
            BucketName = bucketName,
            Prefix = directoryBucket
        };

        var response = await s3Client.ListObjectsV2Async(request);
        return response.S3Objects.Select(o => new S3FileObject
        {
            FileName = o.Key.Split("/").Last(),
            FilePath = $"{s3Config.ExternalUrl}/{bucketName}/{o.Key}"
        });
    }

    public async Task<string> GetFilePath(string bucketName, string filePath, string defaultFile = "")
    {
        var isExistFile = await IsFileExistsAsync(bucketName, filePath);
        var fileName = isExistFile ? filePath : defaultFile;
        return $"{s3Config.ExternalUrl}/{bucketName}/{fileName}";
    }

    public async Task<bool> DeleteFileAsync(string bucketName, string objectName)
    {
        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = objectName
            };

            var response = await s3Client.DeleteObjectAsync(deleteRequest);
            return response.HttpStatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent;
        }
        catch (AmazonS3Exception ex)
        {
            await logger.ErrorAsync($"Delete to s3 Exception. Message: {ex.Message}", ex);
            return false;
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync($"Delete to s3 Exception. Message: {ex.Message}", ex.Message);
            return false;
        }
    }
}