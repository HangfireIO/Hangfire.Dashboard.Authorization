Hangfire.Dashboard.Authorization
================================

Some authorization filters for Hangfire's Dashboard.

Installation
-------------

This library is available as a NuGet Package:

```
Install-Package Hangfire.Dashboard.Authorization
```

Usage
------

## OWIN-based authentication

```csharp
using Hangfire.Dashboard;

public void Configure(IAppBuilder app)
{
    var options = new DashboardOptions
    {
        Authorization = new [] 
        {
            new DashboardAuthorizationFilter { Users = "admin, superuser", Roles = "advanced" },
            new ClaimsBasedDashboardAuthorizationFilter("name", "value")
        }
    };
    app.UseHangfireDashboard("/hangfire", options);
}
```

## Basic authentication

 *Note:* If you are using basic authentication together with OWIN security, configure Hangfire *BEFORE* OWIN security configuration.
 
Please, keep in mind, if you have no SSL-based instance for your web application you have to disable `SslRedirect` and `RequireSsl` options (it's enabled by default for security reasons). Otherwise you will have dead redirect.

```csharp
var filter = new BasicDashboardAuthorizationFilter(
    new BasicAuthAuthorizationFilterOptions
    {
        // Require secure connection for dashboard
        RequireSsl = true,
        // Case sensitive login checking
        LoginCaseSensitive = true,
        // Users
        Users = new[]
        {
            new BasicAuthAuthorizationUser
            {
                Login = "Administrator-1",
                // Password as plain text
                PasswordClear = "test"
            },
            new BasicAuthAuthorizationUser
            {
                Login = "Administrator-2",
                // Password as SHA1 hash
                Password = new byte[]{0xa9,
                    0x4a, 0x8f, 0xe5, 0xcc, 0xb1, 0x9b,
                    0xa6, 0x1c, 0x4c, 0x08, 0x73, 0xd3,
                    0x91, 0xe9, 0x87, 0x98, 0x2f, 0xbb,
                    0xd3}
            }
        }
});
```

### How to generate password hash

Just run this code:

```csharp
string password = "<your password here>";
using (var cryptoProvider = System.Security.Cryptography.SHA1.Create())
{
    byte[] passwordHash = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(password));
    string result = "new byte[] { " + 
        String.Join(",", passwordHash.Select(x => "0x" + x.ToString("x2")).ToArray())
         + " } ";
}
```

The `result` variable will contain byte array definition with your passowrd.
