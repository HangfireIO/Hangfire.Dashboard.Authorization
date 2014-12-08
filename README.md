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
    app.UseHangfire(config => 
    {
        config.UseAuthorizationFilters(
            new AuthorizationFilter { Users = "admin, superuser", Roles = "advanced" },
            new ClaimsBasedAuthorizationFilter("name", "value"));
    });
}
```

## Basic authentication

 *Note:* If you are using basic authentication together with OWIN security, configure Hangfire *BEFORE* OWIN security configuration.

```csharp
using Hangfire.Dashboard;

public void Configure(IAppBuilder app)
{
    app.UseHangfire(config => 
    {
        config.UseAuthorizationFilters(
                new BasicAuthAuthorizationFilter(
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
                                    Password = Password = new byte[]{0xa9, 0x4a, 0x8f, 0xe5, 0xcc, 0xb1, 0x9b, 0xa6, 0x1c, 0x4c, 0x08, 0x73, 0xd3, 0x91, 0xe9, 0x87, 0x98, 0x2f, 0xbb, 0xd3}
                                }
                            }
                        }));
    });
}
```