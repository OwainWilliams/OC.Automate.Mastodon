# OC.Automate.Mastodon

A Mastodon connection type and action for [Umbraco Automate](https://github.com/umbraco/Umbraco.Automate).

Post statuses to any Mastodon instance as part of an automation workflow — for example, automatically tooting when a blog post is published.

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

## License

MIT
