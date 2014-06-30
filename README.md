Hangfire.Dashboard.Authorization
================================

Some authorization filters for Hangfire's Dashboard

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
