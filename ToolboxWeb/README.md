# `ToolboxWeb`

[![NuGet](https://img.shields.io/nuget/v/ToolboxWeb.svg)](https://www.nuget.org/packages/ToolboxWeb/)
![NuGet](https://img.shields.io/nuget/dt/ToolboxWeb.svg)

Successor to `WebTools`, which is being discontinued. Unlike `WebTools`, this project is not meant to become a single catch-all again — each piece here should earn its place; anything that only one consumer needs, or that's small enough to inline, belongs in that consumer instead.

## Table of Contents

1. [Authentication](#authentication)
1. [Authorization](#authorization)
1. [Configuration](#configuration)
1. [Extensions](#extensions)
1. [Middleware](#middleware)

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

### X509 Certificate Settings

`X509CertificateSettingsBase` — POCO (`Authority`, `Audience`, `Path`, `Pwd`) for certificate-backed JWT bearer setups. `Authority` and `Audience` are independent settings — don't reuse one for both, or you lose the distinction between "who signed this token" and "who it's meant for". Call `ApplyTo(TokenValidationParameters)` to load the cert and wire up `ValidIssuer`/`ValidAudience`/`IssuerSigningKey` in one go:

```csharp
services.AddOptions<JwtBearerOptions>(scheme)
    .Configure<CertSettings>((options, certSettings) =>
        certSettings.ApplyTo(options.TokenValidationParameters));
```

If you are using this (unchanged), nothing extra is needed. If you extend, you need to roll your own mapper. To use together with ToolboxAppOptions, just do:

```csharp
public class CertSettings : X509CertificateSettingsBase, ISettingsBase;
```

### OpenID Connect Settings

`OpenIdConnectSettings` — settings POCO that maps directly onto `OpenIdConnectOptions` via the generated `ApplyTo()` extension. If you are using this (unchanged), nothing extra is needed. If you extend, you need to roll your own mapper. To use together with ToolboxAppOptions, just do:

```csharp
public class OidcSettings : OpenIdConnectSettings, ISettingsBase;
```

```csharp
services.AddOptions<OpenIdConnectOptions>(scheme)
    .Configure<OidcSettings>((options, oidcSettings) => oidcSettings.ApplyTo(options));
```

## Extensions

`ContextExtensions` — `HttpContext` helpers for reconstructing the request's root/base/absolute URL (`GetRootPath`/`GetBasePath`/`GetAbsolutePath`) and reading the caller's bearer token (`GetAccessToken`). `GetAccessToken` returns just the token — the `"Bearer "` scheme prefix is stripped — or `null` if the `Authorization` header isn't a bearer token at all.

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
