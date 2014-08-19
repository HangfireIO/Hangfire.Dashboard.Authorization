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
