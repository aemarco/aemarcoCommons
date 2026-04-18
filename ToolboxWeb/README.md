# `ToolboxWeb`

[![NuGet](https://img.shields.io/nuget/v/ToolboxWeb.svg)](https://www.nuget.org/packages/ToolboxWeb/)
![NuGet](https://img.shields.io/nuget/dt/ToolboxWeb.svg)

## LAN IP Authorization

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
