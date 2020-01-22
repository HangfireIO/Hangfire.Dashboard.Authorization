using System;
using Hangfire.Annotations;
using Microsoft.Owin;

namespace Hangfire.Dashboard
{
    public class ClaimsBasedDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string _type;
        private readonly string _value;

        public ClaimsBasedDashboardAuthorizationFilter(string type, string value)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (value == null) throw new ArgumentNullException("value");

            _type = type;
            _value = value;
        }

        public bool Authorize([NotNull] DashboardContext dashboardContext)
        {
            var context = new OwinContext(dashboardContext.GetOwinEnvironment());

            if (context.Authentication.User == null)
                return false;

            return context.Authentication.User.HasClaim(_type, _value);
        }
    }
}
