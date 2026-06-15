# OC.Automate.Mastodon

A Mastodon connection type and action for [Umbraco Automate](https://github.com/umbraco/Umbraco.Automate).

Post statuses to any Mastodon instance as part of an automation workflow — for example, automatically tooting when a blog post is published.

## Installation

```bash
dotnet add package OC.Automate.Mastodon
```

No further setup required. The composer registers itself automatically via Umbraco's `IComposer` discovery.

## Setup

### 1. Generate a Mastodon access token

In your Mastodon account go to **Preferences → Development → New application** and create an application with the `write:statuses` scope. Copy the access token.

### 2. Add the token to appsettings

Access tokens are stored in configuration, not the backoffice. Add the following to your `appsettings.json` (or `appsettings.Production.json`):

```json
{
  "OC.Automate.Mastodon": {
    "AccessTokens": {
      "myaccount": "your-access-token-here"
    }
  }
}
```

The key (`myaccount` above) is a name you choose — you will reference it when creating the connection in the backoffice. You can add multiple entries if you need to post from more than one account.

For production it is recommended to supply tokens via environment variables rather than a config file:

```
OC__Automate__Mastodon__AccessTokens__myaccount=your-access-token-here
```

### 3. Create the connection in the backoffice

1. Go to **Automate → Connections** and create a new **Mastodon** connection.
2. Enter your instance URL (e.g. `https://mastodon.social`).
3. Enter the **Connection Name** — this must match the key you used in appsettings (e.g. `myaccount`).
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

## Compatibility

| Package version | Umbraco Automate | Umbraco CMS |
|---|---|---|
| 1.x | 17.x | 17.x |

## License

MIT
