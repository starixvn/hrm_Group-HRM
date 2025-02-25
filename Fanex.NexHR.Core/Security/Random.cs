namespace Fanex.NexHR.Core.Security
{
    using System.Security.Cryptography;

    public static class Random
    {
        #region Numeric

        public static string Numberic(int size)
        {
            const string numeric = "0123456789";
            return RandomString(numeric, size);
        }

        #endregion

        #region Alphabet

        public static string Alphabet(int size)
        {
            const string alphabet = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
            return RandomString(alphabet, size);
        }

        public static string AnyAlphabet(int size)
        {
            const string alphabet = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890~!@#$%&()<>?-+=\|/";
            return RandomString(alphabet, size);
        }

        #endregion

        #region RNGCrypRandom

        public static byte[] RNGCrypto(int size)
        {
            byte[] valueArray = new byte[size];
            using (var serviceProvider = new RNGCryptoServiceProvider())
            {
                serviceProvider.GetBytes(valueArray);
            }

            return valueArray;
        }

        #endregion

        private static string RandomString(string allowChars, int size)
        {
            int length = allowChars.Length;
            char[] valueArray = new char[size];

            System.Random rd = new System.Random();
            for (int i = 0; i < size; i++)
            {
                valueArray[i] = allowChars[rd.Next(length)];
            }

            return new string(valueArray);
        }
    }
}
