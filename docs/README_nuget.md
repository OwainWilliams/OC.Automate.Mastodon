# OC.Automate.Mastodon

[![Downloads](https://img.shields.io/nuget/dt/OC.Automate.Mastodon?color=cc9900)](https://www.nuget.org/packages/OC.Automate.Mastodon/)
[![NuGet](https://img.shields.io/nuget/vpre/OC.Automate.Mastodon?color=0273B3)](https://www.nuget.org/packages/OC.Automate.Mastodon)
[![GitHub license](https://img.shields.io/github/license/OwainWilliams/OC.Automate.Mastodon?color=8AB803)](https://github.com/OwainWilliams/OC.Automate.Mastodon/blob/main/LICENSE)

A Mastodon connection type and action for [Umbraco Automate](https://github.com/umbraco/Umbraco.Automate). Post statuses to any Mastodon instance as part of an automation workflow.

## Installation

```bash
dotnet add package OC.Automate.Mastodon
```

No further setup required. The composer registers itself automatically via Umbraco's `IComposer` discovery.

## Setup

### 1. Generate a Mastodon access token

In your Mastodon account go to **Preferences → Development → New application** and create an application with the `write:statuses` scope. Copy the access token.

### 2. Add your settings to configuration (recommended)

The instance URL and access token are entered on the connection in the backoffice, but instead of typing the values directly you can store them in configuration and reference them. This uses Umbraco Automate's built-in **Variables** (non-sensitive values) and **Secrets** (sensitive values) sections — the package no longer has its own configuration section.

Add the following to your `appsettings.json`:

```json
{
  "Umbraco": {
    "Automate": {
      "Variables": {
        "MastodonInstance": "https://umbracocommunity.social"
      },
      "Secrets": {
        "MastodonAccessToken": "your-access-token-here"
      }
    }
  }
}
```

For production, use environment variables instead of a config file:

```
Umbraco__Automate__Variables__MastodonInstance=https://umbracocommunity.social
Umbraco__Automate__Secrets__MastodonAccessToken=your-access-token-here
```

The key names (`MastodonInstance`, `MastodonAccessToken`) are your choice — they just need to match the references you enter on the connection.

### 3. Create the connection in the backoffice

1. Go to **Automate → Connections** and create a new **Mastodon** connection (in the **Custom** group).
2. **Instance URL** — enter the URL directly (e.g. `https://mastodon.social`), or reference your configuration value: `$Umbraco:Automate:Variables:MastodonInstance`
3. **Access Token** — enter the token directly, or (recommended) reference your configuration value: `$Umbraco:Automate:Secrets:MastodonAccessToken`
4. Click **Test connection** to verify.

## Usage

Add the **Send Mastodon Post** action to any automation and select your Mastodon connection. Available fields:

| Field | Description |
|---|---|
| Content | The post text. Supports `${ binding }` expressions. |
| Visibility | `public`, `unlisted`, `private`, or `direct`. Defaults to `public`. |
| Post URL | Optional URL appended to the post on a new line. |
| Sensitive | Marks the post as sensitive/NSFW. |
| Spoiler Text | Content warning shown before the post body. |

> **Note:** Most Mastodon instances limit posts to 500 characters (links count as 23 characters, and spoiler text counts toward the limit). Posts over the instance's limit are rejected by the Mastodon API and the action fails.

## Migration from 1.x to 2.x

Version 2.x is a **breaking change**. If you're upgrading from 1.x, you must update your configuration:

### Configuration Structure
- **Old (1.x)**: `OC:Automate:Mastodon:AccessTokens:connectionName` or `Umbraco:Automate:Providers:OCAutomateMastodon:AccessTokens:connectionName`
- **New (2.x)**: Umbraco Automate's built-in `Variables` and `Secrets` sections:
  - `Umbraco:Automate:Variables:MastodonInstance` (instance URL)
  - `Umbraco:Automate:Secrets:MastodonAccessToken` (access token)

### Connection Setup
- **Old (1.x)**: Connections required a "Connection Name" field matching an appsettings key
- **New (2.x)**: Connections have **Instance URL** and **Access Token** fields; each accepts either a literal value or a configuration reference (e.g. `$Umbraco:Automate:Secrets:MastodonAccessToken`)

### Steps to migrate:
1. Move your access token to `Umbraco:Automate:Secrets:MastodonAccessToken` (and optionally the instance URL to `Umbraco:Automate:Variables:MastodonInstance`) in `appsettings.json` or environment variables
2. Recreate your Mastodon connections in the backoffice (select the new **Mastodon** connection type in the **Custom** group)
3. Fill in both fields, using `$Umbraco:Automate:...` references for any values stored in configuration

## Compatibility

| Package version | Umbraco Automate | Umbraco CMS |
|---|---|---|
| 2.x | 17.x – 18.x | 17.4 – 18.x |
| 1.x | 17.x – 18.x | 17.x – 18.x |

## Links

- [Source code](https://github.com/OwainWilliams/OC.Automate.Mastodon)
- [Report an issue](https://github.com/OwainWilliams/OC.Automate.Mastodon/issues)
