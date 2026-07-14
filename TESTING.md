# Testing OC.Automate.Mastodon

This guide explains how to test the package quickly using the integrated test site.

## The Problem (Before)

- Pack the NuGet package
- Create a new Umbraco site
- Install the package
- Test it
- Repeat for every change
- ❌ Slow and tedious

## The Solution (Now)

Use the test site included in this repo with a **direct project reference** — changes show up immediately without any packing or reinstalling.

## Choose Your Test Version

This repo includes test sites for both **Umbraco 17** and **Umbraco 18**. Test against the version(s) you need to support.

## Quick Start

### 1. Configure Your Mastodon Token

Create `appsettings.local.json` in the test site you're using:

**For Umbraco 18**: `src/OC.Automate.Mastodon.TestSite/appsettings.local.json`  
**For Umbraco 17**: `src/OC.Automate.Mastodon.TestSite.U17/appsettings.local.json`

```json
{
  "Umbraco": {
    "Automate": {
      "Secrets": {
        "MastodonAccessToken": "your-actual-test-token"
      }
    }
  }
}
```

Or use an environment variable:

```bash
export Umbraco__Automate__Secrets__MastodonAccessToken=your-token
```

### 2. Start the Test Site

**Option A: Using Claude Code (Recommended)**

For Umbraco 18:
```bash
/preview start test-site-u18
```
Access at: http://localhost:5001/umbraco

For Umbraco 17:
```bash
/preview start test-site-u17
```
Access at: http://localhost:5002/umbraco

**Option B: Using CLI**

For Umbraco 18:
```bash
cd src/OC.Automate.Mastodon.TestSite
dotnet run --urls "https://localhost:5001"
```

For Umbraco 17:
```bash
cd src/OC.Automate.Mastodon.TestSite.U17
dotnet run --urls "https://localhost:5002"
```

### 3. Log In & Test

1. First run: Umbraco will prompt for initial setup
2. Navigate to **Automate → Connections**
3. Create a new **Mastodon** connection
4. Test your changes in the backoffice

## Development Workflow

### Making Changes

1. **Edit code** in `src/OC.Automate.Mastodon/`
2. **Rebuild** (automatic with hot reload, or `dotnet build`)
3. **Refresh browser** to see changes
4. **Test** in the backoffice

No pack. No install. No waiting.

### Debugging

- Set breakpoints in either the package code or test site code
- Both are in the same solution — full debugging experience
- Attach debugger to the running test site process

### Before Final Release

Before pushing to NuGet, verify the **packaged version** works:

1. Pack the main project:
   ```bash
   cd src/OC.Automate.Mastodon
   dotnet pack --configuration Release
   ```

2. Temporarily switch the test site to use the NuGet package instead of project reference:
   ```xml
   <!-- In OC.Automate.Mastodon.TestSite.csproj -->
   <ItemGroup>
     <!-- Remove: <ProjectReference ... /> -->
     <!-- Add: -->
     <PackageReference Include="OC.Automate.Mastodon" Version="2.0.0" />
   </ItemGroup>
   ```

3. Restore and test:
   ```bash
   dotnet clean
   dotnet build
   dotnet run
   ```

4. Once verified, switch back to project reference for development

## File Structure

```
OC.Automate.Mastodon/
├── src/
│   ├── OC.Automate.Mastodon/                ← Package code
│   ├── OC.Automate.Mastodon.TestSite/       ← Test site for Umbraco 18
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── OC.Automate.Mastodon.TestSite.csproj
│   │   └── README.md
│   └── OC.Automate.Mastodon.TestSite.U17/   ← Test site for Umbraco 17
│       ├── Program.cs
│       ├── appsettings.json
│       ├── OC.Automate.Mastodon.TestSite.U17.csproj
│       └── README.md
├── .claude/
│   └── launch.json                        ← Launch configs for both sites
├── TESTING.md                             ← This file
└── OC.Automate.Mastodon.slnx
```

## Tips & Tricks

### Reset the Database

If Umbraco state gets messy:

```bash
rm -rf src/OC.Automate.Mastodon.TestSite/App_Data/
```

The site will reinitialize on next run.

### Use Local Configuration

Keep sensitive data out of git by using `appsettings.local.json` (already in .gitignore):

```json
{
  "Umbraco": {
    "Automate": {
      "Secrets": {
        "MastodonAccessToken": "your-real-token"
      }
    }
  }
}
```

### Multiple Test Accounts

To test with different Mastodon accounts, create multiple connections with different instance URLs in the backoffice. The token is shared, so you'd need to update it in appsettings or use environment variables for different tokens.

### Hot Reload

Changes to C# code will hot reload if you're using `dotnet watch` or the debugger. For configuration changes, restart the site.

## Troubleshooting

Common issues are documented in [TROUBLESHOOTING.md](TROUBLESHOOTING.md), including:

- `System.ArgumentNullException: 'Value cannot be null (Parameter 'provider')`
- Port already in use
- Database locked
- Configuration not loading
- Changes not reflected
- Can't log in to backoffice
- And more...

## See Also

- [TROUBLESHOOTING.md](TROUBLESHOOTING.md) — Detailed troubleshooting guide
- [OC.Automate.Mastodon.TestSite README](src/OC.Automate.Mastodon.TestSite/README.md)
- [Package README](README.md)
- [Umbraco Docs](https://docs.umbraco.com)
- [Umbraco Automate Docs](https://docs.umbraco.com/umbraco-automate)
