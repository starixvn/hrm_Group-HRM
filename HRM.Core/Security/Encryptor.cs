namespace HRM.Core.Security
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class Encryptor
    {
        public static string ComputeHash(string plainText, int saltSize = 8)
        {
            var saltBuffer = new byte[saltSize];
            return ComputeHash(plainText, saltBuffer);
        }

        public static bool CompareStringToHash(string plainText, string encrypt, int saltSize = 8)
        {
            var encryptBuffer = Convert.FromBase64String(encrypt);
            var encryptLength = encryptBuffer.Length;

            var saltBuffer = new byte[saltSize];
            Array.Copy(encryptBuffer, encryptLength - saltSize, saltBuffer, 0, saltSize);

            var hash = ComputeHash(plainText, saltBuffer);
            return string.Equals(encrypt, hash);
        }

        private static string ComputeHash(string plainText, byte[] salt)
        {
            var data = Encoding.UTF8.GetBytes(plainText);
            var dataLength = data.Length;

            var saltLength = salt.Length;

            var dataSalt = new byte[dataLength + saltLength];

            Array.Copy(data, dataSalt, dataLength);
            Array.Copy(salt, 0, dataSalt, dataLength, saltLength);

            HashAlgorithm hash = new SHA1Managed();
            var encrypt = hash.ComputeHash(dataSalt);
            var encryptLength = encrypt.Length;

            var encryptSalt = new byte[encryptLength + saltLength];
            Array.Copy(encrypt, encryptSalt, encryptLength);
            Array.Copy(salt, 0, encryptSalt, encryptLength, saltLength);

            return Convert.ToBase64String(encryptSalt);
        }
    }
}