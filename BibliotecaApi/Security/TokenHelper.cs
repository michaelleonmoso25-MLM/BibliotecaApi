using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using BibliotecaApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BibliotecaApi.Security
{
    /// <summary>
    /// Genera y valida tokens JWT (HS256) sin dependencias externas.
    /// Estructura: base64url(header) . base64url(payload) . base64url(firma HMAC-SHA256).
    /// </summary>
    public static class TokenHelper
    {
        // Datos que viajan dentro del token, ya validados.
        public class TokenData
        {
            public int id { get; set; }
            public string correo { get; set; }
            public string rol { get; set; }
        }

        private static readonly int ExpiraHoras = ReadInt("TokenExpiraHoras", 8);

        private static byte[] Secret()
        {
            string s = ConfigurationManager.AppSettings["TokenSecret"];
            if (string.IsNullOrEmpty(s))
                throw new InvalidOperationException("Falta el appSetting 'TokenSecret' en Web.config.");
            return Encoding.UTF8.GetBytes(s);
        }

        public static string Generate(Usuario u)
        {
            var header = new JObject { ["alg"] = "HS256", ["typ"] = "JWT" };

            long exp = DateTimeOffset.UtcNow.AddHours(ExpiraHoras).ToUnixTimeSeconds();
            var payload = new JObject
            {
                ["id"] = u.id,
                ["correo"] = u.correo,
                ["rol"] = u.rol,
                ["exp"] = exp
            };

            string h = Base64UrlEncode(Encoding.UTF8.GetBytes(header.ToString(Formatting.None)));
            string p = Base64UrlEncode(Encoding.UTF8.GetBytes(payload.ToString(Formatting.None)));
            string signingInput = h + "." + p;
            string sig = Base64UrlEncode(Sign(signingInput));

            return signingInput + "." + sig;
        }

        /// <summary>Valida firma y expiración. Devuelve null si el token no es válido.</summary>
        public static TokenData Validate(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            string[] parts = token.Split('.');
            if (parts.Length != 3) return null;

            string signingInput = parts[0] + "." + parts[1];
            byte[] expectedSig = Sign(signingInput);
            byte[] actualSig;
            try { actualSig = Base64UrlDecode(parts[2]); }
            catch { return null; }

            if (!FixedTimeEquals(expectedSig, actualSig)) return null;

            JObject payload;
            try
            {
                string json = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
                payload = JObject.Parse(json);
            }
            catch { return null; }

            long exp = payload.Value<long?>("exp") ?? 0;
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= exp) return null;

            return new TokenData
            {
                id = payload.Value<int?>("id") ?? 0,
                correo = (string)payload["correo"],
                rol = (string)payload["rol"]
            };
        }

        private static byte[] Sign(string input)
        {
            using (var hmac = new HMACSHA256(Secret()))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }

        private static int ReadInt(string key, int fallback)
        {
            int v;
            return int.TryParse(ConfigurationManager.AppSettings[key], out v) ? v : fallback;
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string s = input.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
            }
            return Convert.FromBase64String(s);
        }

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
