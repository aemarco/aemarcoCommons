# `aemarcoWebTools`

<a href=https://www.nuget.org/packages/aemarcoWebTools><img src="https://buildstats.info/nuget/aemarcoWebTools"></a>

## Table of Contents

1. [Authorization](#authorization)
1. [Filter](#filter)
1. [Middleware](#middleware)

## Authorization


Adds policies with given names, which ensure LAN access only.
```
services.AddLanIpAddressPolicy("lanOnly");
```
Adds a policy, which requires client_id with one of the client id´s 
```
services.AddAllowedClientsPolicy("clientB", "clientB");
```


Policy documentation can be found at
https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies

Great read around DefaultPolicy and FallbackPolicy
https://scottsauber.com/2020/01/20/globally-require-authenticated-users-by-default-using-fallback-policies-in-asp-net-core

Read about Policy Conventions
https://andrewlock.net/setting-global-authorization-policies-using-the-defaultpolicy-and-the-fallbackpolicy-in-aspnet-core-3


## Filter

Add´s a filter, which only calls the action when the ModelState is valid
```
services.AddControllers(options =>
            {
                options.Filters.Add(new ValidationFilter());
            });
```


## Middleware

Adds a middleware, which moves an access_token provided as query parameter, to an Authorization heder for a Bearer.
```
app.UseBearerToHeaderMiddleware(); //move bearer from url to header before routing
```
