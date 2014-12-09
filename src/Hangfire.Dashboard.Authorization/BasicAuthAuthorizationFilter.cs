using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Owin;

namespace Hangfire.Dashboard
{
    /// <summary>
    /// Represents Hangfire authorization filter for basic authentication.
    /// </summary>
    /// <remarks>If you are using this together with OWIN security, configure Hangfire BEFORE OWIN security configuration.</remarks>
    public class BasicAuthAuthorizationFilter : IAuthorizationFilter
    {
        private readonly BasicAuthAuthorizationFilterOptions _options;
        
        public BasicAuthAuthorizationFilter()
        	: this(new BasicAuthAuthorizationFilterOptions())
        {
        }
        
        public BasicAuthAuthorizationFilter(BasicAuthAuthorizationFilterOptions options)
        {
        	_options = options;
        }
        
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            OwinContext context = new OwinContext(owinEnvironment);

            if ((_options.SslRedirect == true) && (context.Request.Uri.Scheme != "https"))
            {
                context.Response.OnSendingHeaders(state =>
                {
                    string redirectUri = new UriBuilder("https", context.Request.Uri.Host, 443, context.Request.Uri.PathAndQuery).ToString();

                    context.Response.StatusCode = 301;
                    context.Response.Redirect(redirectUri);
                }, null);
                return false;
            }

            if ((_options.RequireSsl == true) && (context.Request.IsSecure == false))
            {
                context.Response.Write("Secure connection is required to access Hangfire Dashboard.");
                return false;
            }

            string header = context.Request.Headers["Authorization"];

            if (String.IsNullOrWhiteSpace(header) == false)
            {
                AuthenticationHeaderValue authValues = AuthenticationHeaderValue.Parse(header);

                if ("Basic".Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase))
                {
                    string parameter = Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));
                    var parts = parameter.Split(':');

                    if (parts.Length > 1)
                    {
                        string login = parts[0];
                        string password = parts[1];

                        if ((String.IsNullOrWhiteSpace(login) == false) && (String.IsNullOrWhiteSpace(password) == false))
                        {
                            return _options
                                .Users
                                .Any(user => user.Validate(login, password, _options.LoginCaseSensitive))
                                   || Challenge(context);
                        }
                    }
                }
            }

            return Challenge(context);
        }

        private bool Challenge(OwinContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");

            context.Response.Write("Authentication is required.");

            return false;
        }
    }
}
