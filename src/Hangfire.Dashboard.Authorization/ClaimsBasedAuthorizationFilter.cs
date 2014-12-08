using System;
using System.Collections.Generic;
using Microsoft.Owin;

namespace Hangfire.Dashboard
{
    public class ClaimsBasedAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string _type;
        private readonly string _value;

        public ClaimsBasedAuthorizationFilter(string type, string value)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (value == null) throw new ArgumentNullException("value");

            _type = type;
            _value = value;
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
