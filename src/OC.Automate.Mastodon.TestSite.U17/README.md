# OC.Automate.Mastodon Test Site (Umbraco 17)

A minimal Umbraco 17 CMS site preconfigured with Umbraco Automate and the OC.Automate.Mastodon package for rapid testing against Umbraco 17.

See [../README.md](../README.md) for the general testing guide. This site follows the same workflow as the Umbraco 18 test site, but uses Umbraco 17 packages.

## Quick Start

### 1. Configure Your Test Token

Edit `appsettings.json` or create `appsettings.local.json`:

```json
{
  "Umbraco": {
    "Automate": {
      "Secrets": {
        "MastodonAccessToken": "your-actual-test-token-here"
      }
    }
  }
}
```

### 2. Run the Test Site

```bash
cd src/OC.Automate.Mastodon.TestSite.U17
dotnet run --urls "https://localhost:5002"
```

Access at: http://localhost:5002/umbraco

### 3. Test Your Changes

Create a Mastodon connection and test the Send Post action in Automate.

## Umbraco Version

- **Umbraco CMS**: 17.x
- **Umbraco Automate**: 17.x

See the main [README.md](../README.md) for full testing documentation.
