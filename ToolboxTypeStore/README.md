# `aemarcoToolboxTypeStore`

<a href=https://www.nuget.org/packages/aemarcoToolboxTypeStore><img src="https://buildstats.info/nuget/aemarcoToolboxTypeStore"></a><br/>

The `aemarcoToolboxTypeStore` library provides a robust, flexible, and non-intrusive way to persist single instances of Plain Old CLR Objects (POCOs) to disk. It's ideal for managing application settings, session data, or any small, mutable data structure that needs to be loaded on startup and saved on exit.

---

## Features

*   **POCO Persistence:** Persist any C# class without modifying its structure with specific attributes or properties.
*   **Metadata Smuggling:** Automatically injects and extracts persistence-related metadata (timestamps, version) directly into the JSON file, keeping your data models clean.
*   **Attribute-Based Configuration:** Use attributes on your data classes to easily configure versioning and user-specific data protection.
*   **Dependency Injection Integration:** Seamlessly integrates with Microsoft.Extensions.DependencyInjection for easy setup and usage.
*   **Custom File Paths:** Allows types to define custom storage locations, or uses intelligent fallbacks.
*   **Save Event Hooks:** Provides hooks for pre-save and post-save logic directly on your data models.
*   **.NET 8+ & Windows-Specific Protection:** Leverages `System.Text.Json` for efficient serialization and `ProtectedData` for Windows-specific encryption.

---

## Installation

Install the `aemarcoToolboxTypeStore` NuGet package into your project:

```bash
dotnet add package aemarcoToolboxTypeStore
```

---

## Setup (Dependency Injection)

The library integrates with `Microsoft.Extensions.DependencyInjection`. You configure the store once during your application's startup.

```csharp

using aemarcoCommons.ToolboxTypeStore;


// Basic setup: Uses defaults
services.SetupTypeStore();

// Custom setup:
services.SetupTypeStore(options =>
{
    ...
});

// Custom setup using ServiceProvider:
services.SetupTypeStore((sp, options) =>
{
    ...
});

```

---

## Defining a Storable Type (`T`)

For a class to be used with `ITypeToFileStore<T>`, it must:
*   Be a `class` (reference type).
*   Have a parameterless constructor (`new()`).
*   Implement `ITypeToFileValue`.

Here's an example showcasing custom path, save events, and attribute-based settings:

```csharp
using aemarcoCommons.ToolboxTypeStore;

[TypeToFileSettings(isUserProtected: true, version: 2)]
public class MyUserSettings : ITypeToFileValue
{
    public string UserName { get; set; } = "DefaultUser";
    public int AppTheme { get; set; } = 0;



    // Optional: Custom File Path !! Should NOT rely on the instance's state.
    public string? GetCustomFilePath() => ...;

    // Optional: Save Event Hooks
    public void OnSaving() => ...;
    public void OnSaved() => ...;
}
```

---

## Usage

Inject `ITypeToFileStore<T>` into your services.

```csharp
using aemarcoCommons.ToolboxTypeStore;

// Retrieve the store
var userSettingsStore = serviceProvider.GetRequiredService<ITypeToFileStore<MyUserSettings>>();

// Access and modify settings
userSettingsStore.Instance.UserName = "NewUser";
userSettingsStore.SaveChanges(); // Explicitly save changes

// Access metadata
Console.WriteLine(
$"First Created: {userSettingsStore.TimestampCreated}, Last Saved: {userSettingsStore.TimestampSaved}");

// Reset to default
userSettingsStore.CommitReset();

// On application exit, the store's Dispose() method is automatically called by the DI container,
// which triggers SaveChanges() if any modifications were made to the Instance.
```
