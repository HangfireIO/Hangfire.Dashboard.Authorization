using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Hangfire.Dashboard
{
    /// <summary>
    /// Represents user to access Hangfire dashboard via basic authentication
    /// </summary>
    public class BasicAuthAuthorizationUser
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="BasicAuthAuthorizationUser"/> class with
        /// <see cref="SHA1"/> as a crypto provider.
        /// </summary>
        public BasicAuthAuthorizationUser()
            : this(SHA1.Create)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="BasicAuthAuthorizationUser"/> with the
        /// specified <paramref name="cryptoProviderFactory"/> that will be used as a crypto
        /// provider.
        /// </summary>
        public BasicAuthAuthorizationUser(Func<HashAlgorithm> cryptoProviderFactory)
        {
            CryptoProviderFactory = cryptoProviderFactory ?? throw new ArgumentNullException(nameof(cryptoProviderFactory));
        }

        /// <summary>
        /// Gets the callback that is used for password hash calculation when setting or
        /// validating a password.
        /// </summary>
        public Func<HashAlgorithm> CryptoProviderFactory { get; }

        /// <summary>
        /// Gets or sets a login name for the user.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets a password hash for the user. Please use the <see cref="PasswordClear"/>
        /// property to set a plain text password that will be hashed automatically.
        /// </summary>
        public byte[] Password { get; set; }

        /// <summary>
        /// Sets the <see cref="Password"/> property from a string by automatically hashing it
        /// using the given <see cref="CryptoProviderFactory"/>.
        /// </summary>
        public string PasswordClear
        {
            set
            {
                using (var cryptoProvider = CryptoProviderFactory())
                {
                    Password = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(value));
                }
            }
        }

        /// <summary>
        /// Validate user
        /// </summary>
        /// <param name="login">User name</param>
        /// <param name="password">User password</param>
        /// <param name="loginCaseSensitive">Whether or not login checking is case sensitive</param>
        /// <returns></returns>
        public bool Validate(string login, string password, bool loginCaseSensitive)
        {
            if (String.IsNullOrWhiteSpace(login) == true)
                throw new ArgumentNullException("login");

            if (String.IsNullOrWhiteSpace(password) == true)
                throw new ArgumentNullException("password");

            if (login.Equals(Login, loginCaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) == true)
            {
                using (var cryptoProvider = CryptoProviderFactory())
                {
                    byte[] passwordHash = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return StructuralComparisons.StructuralEqualityComparer.Equals(passwordHash, Password);
                }
            }
            else
                return false;
        }
    }
}