# Test Site Troubleshooting

Common issues and solutions when running the test sites.

## System.ArgumentNullException: 'Value cannot be null. (Parameter 'provider')'

**Cause**: Incomplete or missing `appsettings.json` configuration, particularly the database connection string.

**Solution**:

1. Verify your test site's `appsettings.json` includes the required sections:

```json
{
  "Umbraco": {
    "CMS": {
      "Global": {
        "Id": "00000000-0000-0000-0000-000000000000",
        "ServerRole": "Single"
      }
    }
  },
  "ConnectionStrings": {
    "umbracoDbDSN": "Filename=App_Data/Umbraco.db;Cache=Shared"
  }
}
```

2. Clean and rebuild:
```bash
dotnet clean
dotnet build
```

3. Delete any existing database:
```bash
rm -r src/OC.Automate.Mastodon.TestSite/App_Data/
```

4. Run again:
```bash
/preview start test-site-u18
```

## Port Already in Use

**Error**: Address already in use on port 5001 or 5002

**Solution**:

Use a different port:
```bash
cd src/OC.Automate.Mastodon.TestSite
dotnet run --urls "https://localhost:5003"
```

Or kill the existing process:
```bash
# Windows
netstat -ano | findstr :5001
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5001
kill -9 <PID>
```

## Database Locked

**Error**: Database is locked / file is in use

**Solution**:

```bash
# Delete the database and let Umbraco recreate it
rm -r src/OC.Automate.Mastodon.TestSite/App_Data/

# Restart the site
/preview start test-site-u18
```

## Umbraco Installation Wizard Hangs

**Cause**: The site is waiting for user input in the setup wizard but can't get it in non-interactive mode.

**Solution**:

1. Stop the site (Ctrl+C)
2. Delete the database:
```bash
rm -r src/OC.Automate.Mastodon.TestSite/App_Data/
```
3. Restart — the wizard should complete automatically with defaults

## Changes Not Reflected

**Cause**: C# code changes require recompilation; configuration changes require restart.

**Solution**:

For C# changes:
```bash
dotnet build
# Refresh browser if using hot reload
```

For configuration changes (appsettings.json):
1. Stop the site (Ctrl+C)
2. Restart:
```bash
/preview start test-site-u18
```

## Connection Test Fails

**Error**: "Could not reach {InstanceUrl}" or "Authentication failed"

**Cause**: Invalid token or incorrect instance URL.

**Solution**:

1. Verify your token is valid:
   - Go to your Mastodon instance's **Preferences → Development**
   - Check that the application exists and is active
   - Regenerate the token if needed

2. Verify the instance URL format:
   - Should be: `https://mastodon.social` (no trailing slash)
   - Not: `mastodon.social` or `https://mastodon.social/`

3. Check appsettings:
```json
{
  "Umbraco": {
    "Automate": {
      "Mastodon": {
        "InstanceUrl": "https://mastodon.social"
      },
      "Secrets": {
        "MastodonAccessToken": "your-actual-token"
      }
    }
  }
}
```

4. Verify the token is actually loaded:
   - Add logging to see if the token is null:
```bash
# Use environment variable instead
export Umbraco__Automate__Secrets__MastodonAccessToken=your-token
/preview start test-site-u18
```

## Can't Log In to Backoffice

**Cause**: First-time setup — the site is waiting for initial credentials.

**Solution**:

The Umbraco setup wizard should auto-complete on first run. If it doesn't:

1. Check the console output for setup status
2. Navigate to `http://localhost:5001/umbraco` (the wizard should redirect there)
3. Complete the setup form with any credentials you like
4. Log in with those credentials

After initial setup, you can:
- Email: `admin@example.com`
- Password: Whatever you set during setup

## Site Won't Start - Missing Dependencies

**Error**: Package not found errors or version mismatches

**Solution**:

```bash
# Restore packages
dotnet restore

# Clean and rebuild
dotnet clean
dotnet build

# If still failing, check the specific test site
cd src/OC.Automate.Mastodon.TestSite
dotnet build
```

## Both Test Sites Interfering

**Cause**: Running both U17 and U18 sites simultaneously can cause database conflicts if they use the same path.

**Solution**:

The test sites are configured with **different database paths** to avoid this:
- U18: `src/OC.Automate.Mastodon.TestSite/App_Data/Umbraco.db`
- U17: `src/OC.Automate.Mastodon.TestSite.U17/App_Data/Umbraco.db`

They can safely run side-by-side on different ports (5001 and 5002).

## Still Stuck?

If you're still seeing issues:

1. **Check the console output** — Umbraco writes detailed errors there
2. **Check appsettings.json** — Make sure it's valid JSON (use an online validator)
3. **Check for typos** — Umbraco config keys are case-sensitive
4. **Check permissions** — Ensure the app can write to `App_Data/`
5. **Try the other version** — Test on both U17 and U18 to isolate the issue

For more help, check:
- [TESTING.md](TESTING.md)
- [Umbraco Documentation](https://docs.umbraco.com)
- [Umbraco Community](https://community.umbraco.com)
