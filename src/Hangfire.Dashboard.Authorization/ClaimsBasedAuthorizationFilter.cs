using System;
using System.Collections.Generic;
using Microsoft.Owin;

namespace Hangfire.Dashboard
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class ClaimsBasedAuthorizationFilter : IAuthorizationFilter, IDashboardAuthorizationFilter
#pragma warning restore CS0618 // Type or member is obsolete
    {
        private readonly string _type;
        private readonly string _value;

        public ClaimsBasedAuthorizationFilter(string type, string value)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (value == null) throw new ArgumentNullException(nameof(value));

            _type = type;
            _value = value;
        }

        public bool Authorize(DashboardContext context)
        {
            return Authorize(context.GetOwinEnvironment());
        }

        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            var context = new OwinContext(owinEnvironment);
            
            if (context.Authentication.User == null)
                return false;
                
            return context.Authentication.User.HasClaim(_type, _value);
        }

    }
}
