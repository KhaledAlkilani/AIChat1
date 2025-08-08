using System.Security.Cryptography;

namespace AIChat1.Helpers
{
    public class CustomPasswordHasher
    {
        private const int SaltSize = 16; // Size of the salt in bytes 128 bits
        private const int HashSize = 20; // Size of the hash in bytes 256 bits
        private const int Iterations = 10000; // Number of iterations for the PBKDF2 algorithm

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }

            byte[] hashBytes = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash))
                return false;

            // 1. Decode the stored Base64
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // 2. Extract the salt from the hash
            if (hashBytes.Length < SaltSize + HashSize)
                return false; // Data is corrupt or invalid

            // 3. Extract the original hash
            byte[] originalHash = new byte[HashSize];
            Buffer.BlockCopy(hashBytes, SaltSize, originalHash, 0, HashSize);

            // 4. Derive a new hash using the same salt, iterations, and hash size
            byte[] newHash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, hashBytes.Take(SaltSize).ToArray(), Iterations, HashAlgorithmName.SHA256))
            {
                newHash = pbkdf2.GetBytes(HashSize);
            }

            // 5. Compare the new hash with the original hash in a constant-time manner
            return CryptographicOperations.FixedTimeEquals(originalHash, newHash);
        }
    }
}
