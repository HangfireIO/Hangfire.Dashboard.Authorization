﻿using System;
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
        /// Represents user's name
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// HMAC hashed password
        /// </summary>
        public byte[] Password { get; set; }

        /// <summary>
        /// Setter to update password as plain text
        /// </summary>
        public string PasswordClear
        {
            set
            {
                using (var cryptoProvider = HMAC.Create())
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
                using (var cryptoProvider = HMAC.Create())
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