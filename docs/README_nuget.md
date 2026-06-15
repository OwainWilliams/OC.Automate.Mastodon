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

1. Go to **Automate → Connections** and create a new **Mastodon** connection.
2. Enter your instance URL (e.g. `https://mastodon.social`) and an access token.
   - Generate a token at **Preferences → Development → New application** with the `write:statuses` scope.
3. Click **Test connection** to verify.

## Usage

Add the **Send Mastodon Post** action to any automation and select your Mastodon connection. Available fields:

| Field | Description |
|---|---|
| Content | The post text. Supports `${ binding }` expressions. |
| Visibility | `public`, `unlisted`, `private`, or `direct`. Defaults to `public`. |
| Post URL | Optional URL appended to the post on a new line. |
| Sensitive | Marks the post as sensitive/NSFW. |
| Spoiler Text | Content warning shown before the post body. |

## Compatibility

| Package version | Umbraco Automate | Umbraco CMS |
|---|---|---|
| 1.x | 17.x | 17.x |

## Links

- [Source code](https://github.com/OwainWilliams/OC.Automate.Mastodon)
- [Report an issue](https://github.com/OwainWilliams/OC.Automate.Mastodon/issues)
