using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.Owin;

namespace Hangfire.Dashboard
{
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private static readonly string[] EmptyArray = new string[0];

        private string _roles;
        private string[] _rolesSplit = EmptyArray;
        private string _users;
        private string[] _usersSplit = EmptyArray;

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

        public bool Authorize(DashboardContext dashboardContext)
        {
            var context = new OwinContext(dashboardContext.GetOwinEnvironment());
            IPrincipal user = context.Authentication.User;

            if (user?.Identity == null || !user.Identity.IsAuthenticated)
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