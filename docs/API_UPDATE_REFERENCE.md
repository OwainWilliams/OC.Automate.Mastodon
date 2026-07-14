# Umbraco Automate Connection Type Upgrade Pattern

This document outlines the upgrade process for refactoring connection types to follow Umbraco's recommended approach, as implemented in OC.Automate.Mastodon v2.x.

## Why This Change?

The recommended Umbraco Automate approach consolidates connection configuration and leverages automatic discovery via attributes, reducing boilerplate and complexity.

**Benefits:**
- Settings and connection type are colocated
- Auto-discovery via `[ConnectionType]` attribute (no manual registration)
- Configuration references support environment-based secrets
- Simplified dependency injection
- Cleaner code structure

---

## Old Pattern (v1.x)

### 1. Split Settings Classes

**MastodonConnectionSettings.cs** (backoffice fields):
```csharp
public class MastodonConnectionSettings
{
    [Field(Label = "Instance URL")]
    public string InstanceUrl { get; set; } = string.Empty;

    [Field(Label = "Connection Name")]
    public string ConnectionName { get; set; } = string.Empty;
}
```

**MastodonSettings.cs** (credential holder):
```csharp
public sealed class MastodonSettings
{
    [Field(Label = "Access Token", IsSensitive = true)]
    public string AccessTokens { get; set; } = "$OC:Automate:Mastodon:AccessTokens:default";
}
```

### 2. Separate Connection Type File

**MastodonConnectionType.cs**:
```csharp
[ConnectionType("mastodon", "Mastodon")]
public class MastodonConnectionType : ConnectionTypeBase<MastodonConnectionSettings>
{
    private readonly MastodonClientFactory _clientFactory;

    public override async Task<ConnectionValidationResult> ValidateAsync(object? settings, CancellationToken cancellationToken)
    {
        var mastodonSettings = settings as MastodonConnectionSettings;
        if (!_clientFactory.TryCreateClient(mastodonSettings.ConnectionName, out var client, out var error))
            return ConnectionValidationResult.Failure(error);
        // ...validation logic
    }
}
```

### 3. Complex Factory with Token Lookup

**MastodonClientFactory.cs**:
```csharp
public bool TryCreateClient(string connectionName, out HttpClient client, out string error)
{
    client = null!;
    error = string.Empty;

    if (_settings.CurrentValue.AccessTokens != null)
    {
        client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.CurrentValue.AccessTokens);
        return true;
    }

    error = $"No access token found for connection name '{connectionName}'.";
    return false;
}
```

### 4. Manual Composer Registration

**MastodonComposer.cs**:
```csharp
public void Compose(IUmbracoBuilder builder)
{
    builder.WithCollectionBuilder<ConnectionTypeCollectionBuilder>()
        .Add<MastodonConnectionType>();  // Manual registration
    // ...
}
```

### 5. appsettings.json Structure

```json
{
  "OC": {
    "Automate": {
      "Mastodon": {
        "AccessTokens": {
          "myaccount": "token-here"
        }
      }
    }
  }
}
```

---

## New Pattern (v2.x) - Recommended

### 1. Unified Settings Class

**MastodonSettings.cs** (consolidates both):
```csharp
public sealed class MastodonSettings
{
    public const string SectionName = "Umbraco:Automate:Mastodon";

    [Field(Label = "Instance URL")]
    public string InstanceUrl { get; set; } = string.Empty;

    [Field(Label = "Access Token", IsSensitive = true)]
    public string AccessToken { get; set; } = "$Umbraco:Automate:Secrets:MastodonAccessToken";
}
```

### 2. Connection Type in Same File

**MastodonSettings.cs** (continues):
```csharp
[ConnectionType("mastodon", "Mastodon")]
public sealed class MastodonConnectionType : ConnectionTypeBase<MastodonSettings>
{
    public MastodonConnectionType(ConnectionTypeInfrastructure infrastructure)
        : base(infrastructure)  // Simplified - no factory injection
    {
    }

    public override async Task<ConnectionValidationResult> ValidateAsync(object? settings, CancellationToken cancellationToken)
    {
        var mastodonSettings = settings as MastodonSettings;
        
        if (string.IsNullOrWhiteSpace(mastodonSettings?.InstanceUrl))
            return ConnectionValidationResult.Failure("Instance URL is required.");

        if (string.IsNullOrWhiteSpace(mastodonSettings.AccessToken))
            return ConnectionValidationResult.Failure("Access token is required.");

        // Validation logic uses settings directly
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", mastodonSettings.AccessToken);
        // ...
    }
}
```

### 3. Simplified Factory

**MastodonClientFactory.cs**:
```csharp
public sealed class MastodonClientFactory
{
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(15);
    private readonly IHttpClientFactory _httpClientFactory;

    public MastodonClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public HttpClient CreateClient(MastodonSettings settings)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = RequestTimeout;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.AccessToken);
        return client;
    }
}
```

### 4. Auto-Discovery (No Manual Registration)

**MastodonComposer.cs**:
```csharp
public void Compose(IUmbracoBuilder builder)
{
    builder.Services.AddOptions<MastodonSettings>()
        .BindConfiguration(MastodonSettings.SectionName);

    builder.Services.AddSingleton<MastodonClientFactory>();

    builder.WithCollectionBuilder<ActionCollectionBuilder>()
        .Add<SendMastodonPostAction>();

    builder.Services.AddSingleton<IPackageManifestReader, MastodonPackageManifestReader>();
    
    // No manual ConnectionType registration needed!
}
```

### 5. appsettings.json Structure

```json
{
  "Umbraco": {
    "Automate": {
      "Mastodon": {
        "InstanceUrl": "https://mastodon.social"
      },
      "Secrets": {
        "MastodonAccessToken": "token-here"
      }
    }
  }
}
```

Or with environment variables:
```bash
Umbraco__Automate__Secrets__MastodonAccessToken=token-here
```

---

## Step-by-Step Upgrade Checklist

- [ ] **Consolidate Settings**: Merge separate settings classes into one
- [ ] **Move Connection Type**: Move `ConnectionTypeBase` to settings file
- [ ] **Add Attribute**: Add `[ConnectionType("alias", "Display Name")]`
- [ ] **Update Validation**: Simplify `ValidateAsync` to use settings directly
- [ ] **Simplify Factory**: Remove connection name lookup, accept settings directly
- [ ] **Update Composer**: Remove manual connection type registration
- [ ] **Update Configuration**: Change to `Umbraco:Automate:Secrets:*` pattern
- [ ] **Update README**: Document breaking changes and migration path
- [ ] **Verify Build**: Ensure project compiles without errors
- [ ] **Test Connection**: Verify validation works in backoffice

---

## Key Principles

1. **One Settings Class**: All configuration fields live in a single settings model
2. **Colocated Type**: Connection type class should be in the same file as settings
3. **Auto-Discovery**: Use `[ConnectionType]` attribute instead of manual registration
4. **Simplified DI**: Connection type only needs `ConnectionTypeInfrastructure`
5. **Configuration References**: Use `$Umbraco:Automate:Secrets:*` for sensitive data
6. **Direct Settings Usage**: Factory and validation work directly with settings, no lookup

---

## Common Pitfalls to Avoid

❌ **Don't**: Keep connection settings and credentials in separate files  
✅ **Do**: Consolidate into one settings class

❌ **Don't**: Manually register connection types in composer  
✅ **Do**: Use `[ConnectionType]` attribute for auto-discovery

❌ **Don't**: Use connection name as a lookup key  
✅ **Do**: Pass settings directly to factory methods

❌ **Don't**: Store multiple tokens in a nested structure  
✅ **Do**: Use a single token field with configuration references

❌ **Don't**: Forget to remove old files  
✅ **Do**: Clean up after consolidation (delete separate connection type files)

---

## File Structure Comparison

### Before (v1.x)
```
Settings/
├── MastodonConnectionSettings.cs    (backoffice fields)
├── MastodonSettings.cs               (credentials)
├── MastodonConnectionType.cs         (connection logic)
├── SendMastodonPostAction.cs         (action using connection)
Factory/
├── MastodonClientFactory.cs          (token lookup + HTTP client)
```

### After (v2.x)
```
Settings/
├── MastodonSettings.cs               (unified: fields + connection type)
├── MastodonPostSettings.cs           (action settings, unchanged)
├── SendMastodonPostAction.cs         (action, simplified)
Factory/
├── MastodonClientFactory.cs          (simplified: just HTTP client)
```

---

## Configuration Migration Example

### Migrate from 1.x to 2.x

**Old appsettings.json (1.x)**:
```json
{
  "OC": {
    "Automate": {
      "Mastodon": {
        "AccessTokens": {
          "account1": "token-abc",
          "account2": "token-xyz"
        }
      }
    }
  }
}
```

**New appsettings.json (2.x)**:
```json
{
  "Umbraco": {
    "Automate": {
      "Mastodon": {
        "InstanceUrl": "https://mastodon.social"
      },
      "Secrets": {
        "MastodonAccessToken": "token-abc"
      }
    }
  }
}
```

**For different instances, create multiple connections in the backoffice** instead of multiple keys in appsettings.

---

## References

- [Umbraco Automate Docs: Custom Connection Type](https://docs.umbraco.com/umbraco-automate/extending/custom-connection-type)
- [OC.Automate.Mastodon v2.x Migration](../README.md#migration-from-1x-to-2x)
