using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Owin;

namespace Hangfire.Dashboard
{
    [Obsolete("Please use `DashboardAuthorizationFilter` instead. Will be removed in 4.0.0.")]
    public class AuthorizationFilter : IAuthorizationFilter
    {
        private string _roles;
        private string[] _rolesSplit = StringHelpers.EmptyArray;
        private string _users;
        private string[] _usersSplit = StringHelpers.EmptyArray;

        /// <summary>
        /// Gets or sets the authorized roles.
        /// </summary>
        /// <value>
        /// The roles string.
        /// </value>
        /// <remarks>Multiple role names can be specified using the comma character as a separator.</remarks>
        public string Roles
        {
            get { return _roles ?? String.Empty; }
            set
            {
                _roles = value;
                _rolesSplit = StringHelpers.SplitString(value);
            }
        }

        /// <summary>
        /// Gets or sets the authorized users.
        /// </summary>
        /// <value>
        /// The users string.
        /// </value>
        /// <remarks>Multiple role names can be specified using the comma character as a separator.</remarks>
        public string Users
        {
            get { return _users ?? String.Empty; }
            set
            {
                _users = value;
                _usersSplit = StringHelpers.SplitString(value);
            }
        }

        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            var context = new OwinContext(owinEnvironment);
            IPrincipal user = context.Authentication.User;

            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return false;
            }

            if (_usersSplit.Length > 0 && !_usersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            if (_rolesSplit.Length > 0 && !_rolesSplit.Any(user.IsInRole))
            {
                return false;
            }

            return true;
        }
    }
}
