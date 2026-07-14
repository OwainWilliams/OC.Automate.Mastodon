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

### 2. Add the token to configuration

Access tokens are stored in configuration, not the backoffice. Add the following to your `appsettings.json` (or use environment variables):

```json
{
  "Umbraco": {
    "Automate": {
      "Secrets": {
        "MastodonAccessToken": "your-access-token-here"
      }
    }
  }
}
```

For production, use environment variables instead of a config file:

```
Umbraco__Automate__Secrets__MastodonAccessToken=your-access-token-here
```

To use a different configuration key, update the `AccessToken` default value in the connection settings.

### 3. Create the connection in the backoffice

1. Go to **Automate → Connections** and create a new **Mastodon** connection (in the **Custom** group).
2. Enter your instance URL (e.g. `https://mastodon.social`).
3. Click **Test connection** to verify.

Note: The access token is automatically resolved from configuration — no manual entry needed.

## Usage

Add the **Send Mastodon Post** action to any automation and select your Mastodon connection. Available fields:

| Field | Description |
|---|---|
| Content | The post text. Supports `${ binding }` expressions. |
| Visibility | `public`, `unlisted`, `private`, or `direct`. Defaults to `public`. |
| Post URL | Optional URL appended to the post on a new line. |
| Sensitive | Marks the post as sensitive/NSFW. |
| Spoiler Text | Content warning shown before the post body. |

## Migration from 1.x to 2.x

Version 2.x is a **breaking change**. If you're upgrading from 1.x, you must update your configuration:

### Configuration Structure
- **Old (1.x)**: `OC:Automate:Mastodon:AccessTokens:connectionName` or `Umbraco:Automate:Providers:OCAutomateMastodon:AccessTokens:connectionName`
- **New (2.x)**: `Umbraco:Automate:Secrets:MastodonAccessToken`

### Connection Setup
- **Old (1.x)**: Connections required a "Connection Name" field matching an appsettings key
- **New (2.x)**: Only the instance URL is needed; the access token is automatically resolved from configuration

### Steps to migrate:
1. Update your `appsettings.json` or environment variables to use the new configuration path
2. Recreate your Mastodon connections in the backoffice (select the new **Mastodon** connection type in the **Custom** group)
3. Enter only the instance URL; the access token is automatically loaded from configuration

## Compatibility

| Package version | Umbraco Automate | Umbraco CMS |
|---|---|---|
| 2.x | 17.x – 18.x | 17.x – 18.x |
| 1.x | 17.x – 18.x | 17.x – 18.x |

## Links

- [Source code](https://github.com/OwainWilliams/OC.Automate.Mastodon)
- [Report an issue](https://github.com/OwainWilliams/OC.Automate.Mastodon/issues)
