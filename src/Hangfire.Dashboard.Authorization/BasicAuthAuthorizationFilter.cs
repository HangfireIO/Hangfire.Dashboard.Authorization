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
#pragma warning disable CS0618 // Type or member is obsolete
    public class BasicAuthAuthorizationFilter : IAuthorizationFilter, IDashboardAuthorizationFilter
#pragma warning restore CS0618 // Type or member is obsolete
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

        public bool Authorize(DashboardContext context)
        {
            return Authorize(context.GetOwinEnvironment());
        }

       public bool Authorize(IDictionary<string, object> owinEnvironment)
{
    OwinContext context = new OwinContext(owinEnvironment);

    // Redirect to HTTPS if SslRedirect is enabled and the request is not secure
    if (_options.SslRedirect && context.Request.Uri.Scheme != "https")
    {
        context.Response.OnSendingHeaders(state =>
        {
            string redirectUri = new UriBuilder("https", context.Request.Uri.Host, 443, context.Request.Uri.PathAndQuery).ToString();
            context.Response.StatusCode = 301;
            context.Response.Redirect(redirectUri);
        }, null);
        return false;
    }

    // Reject request if RequireSsl is enabled and the request is not secure
    if (_options.RequireSsl && !context.Request.IsSecure)
    {
        context.Response.Write("Secure connection is required to access Hangfire Dashboard.");
        return false;
    }

    // Check for Authorization header
    string header = context.Request.Headers["Authorization"];
    if (!string.IsNullOrWhiteSpace(header))
    {
        AuthenticationHeaderValue authValues = AuthenticationHeaderValue.Parse(header);

        // Process Basic authentication
        if (authValues.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
        {
            string parameter = Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));
            var parts = parameter.Split(':');

            if (parts.Length > 1)
            {
                string login = parts[0];
                string password = parts[1];

                if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password))
                {
                    bool isValidUser = _options.Users.Any(user => user.Validate(login, password, _options.LoginCaseSensitive));
                    if (isValidUser)
                    {
                        return true;
                    }
                }
            }
        }
    }

    // Challenge the request if authorization fails
    return Challenge(context);
}

        private static bool Challenge(OwinContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");

            context.Response.Write("Authentication is required.");

            return false;
        }
    }
}
