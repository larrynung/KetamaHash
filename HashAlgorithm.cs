using System.Security.Cryptography;
using System.Text;

namespace KetamaHash
{
    internal class HashAlgorithm
    {
        public static long Hash(byte[] digest, int nTime)
        {
            const int mask = 0xFF;
            return ((long) (digest[3 + nTime*4] & mask) << 24)
                   | ((long) (digest[2 + nTime*4] & mask) << 16)
                   | ((long) (digest[1 + nTime*4] & mask) << 8)
                   | ((long) digest[0 + nTime*4] & mask);
        }

        /// <summary>
        /// Get the md5 of the given key.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static byte[] ComputeMd5(string k)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(k));
            }
        }
    }
}
