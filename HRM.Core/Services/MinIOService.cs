using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRM.Core.Services;

public interface IMinIOService
{
    Task<bool> UploadFileAsync(IFormFile file, string bucketName, string filePath);

    Task<string> GetFilePath(string bucketName, string filePath, string defaultFile = "");

    Task<bool> DeleteFileAsync(string bucketName, string objectName);

    Task<bool> IsFileExistsAsync(string bucketName, string filePath);
}

public class MinIOService : IMinIOService
{
    private readonly AmazonS3Client s3Client;
    private readonly ILogger<MinIOService> logger;
    private readonly string externalUrl;

    public MinIOService(IConfiguration configuration, ILogger<MinIOService> logger)
    {
        var accessKey = configuration["AWS:AccessKey"];
        var secretKey = configuration["AWS:SecretKey"];
        var internalUrl = configuration["AWS:InternalUrl"];
        externalUrl = configuration["AWS:ExternalUrl"];

        var config = new AmazonS3Config
        {
            ServiceURL = internalUrl,
            ForcePathStyle = true,
            UseHttp = true
        };

        s3Client = new AmazonS3Client(accessKey, secretKey, config);
        this.logger = logger;
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
        catch (AmazonS3Exception e)
        {
            logger.LogError(e, "Upload to s3 Exception. Message: {Message}", e.Message);
            return false;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Upload to s3 Exception. Message: {Message}", e.Message);
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

    public async Task<string> GetFilePath(string bucketName, string filePath, string defaultFile = "")
    {
        var isExistFile = await IsFileExistsAsync(bucketName, filePath);
        var fileName = isExistFile ? filePath : defaultFile;
        return $"{externalUrl}/{bucketName}/{fileName}";
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
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (AmazonS3Exception e)
        {
            logger.LogError(e, "Delete to s3 Exception. Message: {Message}", e.Message);
            return false;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Delete to s3 Exception. Message: {Message}", e.Message);
            return false;
        }
    }
}