using System.Security.Cryptography;
using System.Text;

namespace Barriot.Extensions
{
    internal static class SecurityExtensions
    {
        private readonly static SHA256 _hasher = SHA256.Create();

        private readonly static Random _random = new();

        private static DateTime? _sudoSpan = null;

        public static bool InSudo
            => _sudoSpan.HasValue && _sudoSpan.Value <= DateTime.UtcNow.AddMinutes(30);

        public static void EnableSudoMode()
            => _sudoSpan = DateTime.UtcNow;

        /// <summary>
        ///     Computes a hash from the provided values.
        /// </summary>
        /// <param name="value">The value to encrypt.</param>
        /// <param name="salt">The encryption salt.</param>
        /// <returns>A hash computed by the hasher.</returns>
        public static string ComputeHash(string value, string salt)
        {
            var bitSet = _hasher.ComputeHash(Encoding.UTF8.GetBytes(value + salt));

            return BitConverter
                .ToString(bitSet)
                .Replace("-", string.Empty);
        }

        /// <summary>
        ///     Generates a random validation key.
        /// </summary>
        /// <returns>The newly generated validation key.</returns>
        public static string GenerateValidationKey()
            => _random.Next(100000, 999999).ToString();

        /// <summary>
        ///     Generates a random salt.
        /// </summary>
        /// <returns>The newly generated salt.</returns>
        public static string GenerateSalt()
            => _random.NextDouble().ToString();

        public static bool IsApplicationDeveloper(UserEntity user)
            => user.Flags.Any(x => x.Emoji == UserFlag.Developer.Emoji);
    }
}
