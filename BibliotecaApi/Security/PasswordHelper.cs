using System;
using System.Security.Cryptography;

namespace BibliotecaApi.Security
{
    /// <summary>
    /// Hash y verificación de contraseñas con PBKDF2 (HMAC-SHA256).
    /// Formato almacenado: "iteraciones:base64(salt):base64(hash)".
    /// </summary>
    public static class PasswordHelper
    {
        private const int Iterations = 100000;
        private const int SaltSize = 16;   // bytes
        private const int KeySize = 32;    // bytes

        public static string Hash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] key = Pbkdf2(password, salt, Iterations, KeySize);
            return string.Format("{0}:{1}:{2}",
                Iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(key));
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stored))
                return false;

            string[] parts = stored.Split(':');
            if (parts.Length != 3) return false;

            int iterations;
            if (!int.TryParse(parts[0], out iterations)) return false;

            byte[] salt, expected;
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                expected = Convert.FromBase64String(parts[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] actual = Pbkdf2(password, salt, iterations, expected.Length);
            return FixedTimeEquals(actual, expected);
        }

        private static byte[] Pbkdf2(string password, byte[] salt, int iterations, int length)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(length);
            }
        }

        // Comparación en tiempo constante para evitar ataques de temporización.
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
