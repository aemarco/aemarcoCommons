# `aemarcoToolboxConsole`

[![NuGet](https://img.shields.io/nuget/v/aemarcoToolboxConsole.svg)](https://www.nuget.org/packages/aemarcoToolboxConsole/)
![NuGet](https://img.shields.io/nuget/dt/aemarcoToolboxConsole.svg)



## `SetupSerilogging`

Precedence — code defaults < `appsettings.json` < the `configure` callback — is deliberate, and achieved differently per Serilog API since Serilog isn't symmetric here:

- **Enrichment properties** use first-set-wins semantics, so they're applied before `ReadFrom.Configuration`. `"app"`/`"EnvironmentApplicationName"` are derived from `app.Environment.ApplicationName` rather than left config-overridable, since that's reconfigured at the builder level, not through Serilog.
- **`MinimumLevel.Override`** uses last-set-wins semantics, so `ReadFrom.Configuration` runs after the code-level `"Microsoft": Information` default, and `configure` runs last of all. This is namespace-prefix matching, not glob — `"Microsoft*"` is a literal string matching nothing.

## Related Videos

- [How to create your own .NET CLI tools to make your life easier](https://youtu.be/JNDgcBDZPkU)