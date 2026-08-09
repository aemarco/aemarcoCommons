# `ToolboxWeb`

[![NuGet](https://img.shields.io/nuget/v/ToolboxWeb.svg)](https://www.nuget.org/packages/ToolboxWeb/)
![NuGet](https://img.shields.io/nuget/dt/ToolboxWeb.svg)

Successor to `WebTools`, which is being discontinued. Unlike `WebTools`, this project is not meant to become a single catch-all again — each piece here should earn its place; anything that only one consumer needs, or that's small enough to inline, belongs in that consumer instead.

## Table of Contents

1. [Authentication](#authentication)
1. [Authorization](#authorization)
1. [Configuration](#configuration)
1. [Extensions](#extensions)
1. [Filter](#filter)
1. [Middleware](#middleware)
1. [OpenID Connect](#openid-connect)

## Authentication

Adds an authentication scheme that unconditionally succeeds and stamps the caller as `"Anonymous"` — it performs no real check. This exists for authorization policies where identity itself isn't the gate (e.g. LAN-IP-based policies below): ASP.NET Core's pipeline always requires *some* authentication scheme to produce a `ClaimsPrincipal` before a policy's requirements run, even when the actual decision happens entirely in a requirement handler. Never register it as the default scheme.

```csharp
services.AddAuthentication(...)
    .AddAnonymousScheme("Anon");
```

## Authorization

### LAN IP Authorization

Restricts access to requests originating from trusted IP ranges.
- Loopback addresses (`127.0.0.1`, `::1`) are always allowed.
- RFC-1918 private ranges (`10.0.0.0/8`, `172.16.0.0/12`, `192.168.0.0/16`) are trusted by default.

Register one or more named policies:

```csharp
// Default: allows all RFC-1918 ranges (10.0.0.0/8, 172.16.0.0/12, 192.168.0.0/16)
services.AddLanIpAddressPolicy("LanOnly");

// Custom subnets
services.AddLanIpAddressPolicy("LanOnly", opts =>
{
    opts.LocalSubnets = ["192.168.20.0/24"];
});

// Exact IPs — clears subnets and adds /32 (IPv4) or /128 (IPv6) entries; throws on invalid input
services.AddLanIpAddressPolicy("LanOnly", opts =>
{
    opts.WithIpAddresses("192.168.1.10", "192.168.1.11");
});

// Enable hairpin NAT support (allow own public IP)
services.AddLanIpAddressPolicy("LanOnly", opts =>
{
    opts.AllowOwnPublicIp = true;
});

// With a specific authentication scheme
services.AddLanIpAddressPolicy("Bearer", "LanOnly");
```

`AllowOwnPublicIp` resolves the server's own public IP via `IPublicIpService`, trying several external lookup services in order and caching the result. If every source fails, resolution returns `null` and the hairpin check is skipped for that request rather than failing open — it never auto-allows on a resolution failure.

### Allowed Clients

Adds a policy which requires the `client_id` claim to match one of the given client IDs.

```csharp
services.AddAllowedClientsPolicy("clientB", "clientB");
```

Policy documentation can be found at
https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies

Great read around DefaultPolicy and FallbackPolicy
https://scottsauber.com/2020/01/20/globally-require-authenticated-users-by-default-using-fallback-policies-in-asp-net-core

Read about Policy Conventions
https://andrewlock.net/setting-global-authorization-policies-using-the-defaultpolicy-and-the-fallbackpolicy-in-aspnet-core-3

## Configuration

`CertSettingsBase` — minimal POCO (`Authority`, `Path`, `Pwd`) for certificate-backed JWT bearer setups. Extend it and bind via your own settings pipeline (e.g. `ISettingsBase`).

## Extensions

`ContextExtensions` — `HttpContext` helpers for reconstructing the request's root/base/absolute URL (`GetRootPath`/`GetBasePath`/`GetAbsolutePath`) and reading the raw `Authorization` header value (`GetAccessToken`).

## Filter

Adds a filter, which only calls the action when the ModelState is valid.

```csharp
services.AddControllers(options =>
{
    options.Filters.Add(new ValidationFilter());
});
```

## Middleware

### Bearer To Header

Adds a middleware, which moves an access_token provided as query parameter, to an Authorization header for a Bearer.

```csharp
app.UseBearerToHeaderMiddleware(); //move bearer from url to header before routing
```

### Exception Middleware

Catches exceptions and writes a JSON `ErrorResponse`. `BadRequestException` maps to 400 with its message; anything else maps to 500 with a deliberately generic message — the real exception is logged, never sent to the client.

```csharp
app.UseExceptionMiddleware();            // production: catches everything
app.UseDeveloperExceptionMiddleware();   // dev: BadRequestException still returns 400 JSON, everything else goes to ASP.NET's developer exception page
```

## OpenID Connect

`OpenIdConnectSettings` — settings POCO that maps directly onto `OpenIdConnectOptions` via the generated `ApplyTo()` extension. Use it unchanged if you can; if you add properties, you're responsible for writing your own mapper (`ApplyTo` is generated with `RequiredMappingStrategy.Source`, so an unmapped new property fails to compile rather than silently being dropped).

```csharp
services.Configure<OpenIdConnectOptions>(options => oidcSettings.ApplyTo(options));
```
