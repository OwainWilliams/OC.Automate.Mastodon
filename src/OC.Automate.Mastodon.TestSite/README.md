# OC.Automate.Mastodon Test Site (Umbraco 18)

A minimal Umbraco 18 CMS site preconfigured with Umbraco Automate and the OC.Automate.Mastodon package for rapid testing.

## Quick Start

### 1. Configure Your Test Token

Edit `appsettings.json` and replace the placeholder:

```json
"Umbraco": {
  "Automate": {
    "Secrets": {
      "MastodonAccessToken": "your-actual-test-token-here"
    }
  }
}
```

You can also use environment variables (avoid committing secrets):

```bash
export Umbraco__Automate__Secrets__MastodonAccessToken=your-token-here
```

### 2. Run the Site

```bash
cd src/OC.Automate.Mastodon.TestSite
dotnet run
```

Or use the Claude Code launch configuration (if configured in `.claude/launch.json`):

```bash
# In Claude Code, run:
/preview start test-site
```

### 3. Access the Backoffice

- **URL**: http://localhost:5000/umbraco
- **Default User**: admin@example.com
- **Default Password**: Setup wizard will appear on first run

### 4. Test the Connection

1. Navigate to **Automate → Connections**
2. Create a new **Mastodon** connection (in the **Custom** group)
3. Enter your instance URL (e.g., `https://mastodon.social`)
4. Click **Test connection** to verify

### 5. Test the Action

1. Create a new automation
2. Add the **Send Mastodon Post** action
3. Select your Mastodon connection
4. Configure the post settings
5. Run the automation to send a test post

## File Structure

```
OC.Automate.Mastodon.TestSite/
├── OC.Automate.Mastodon.TestSite.csproj  (project references local package)
├── Program.cs                             (Umbraco configuration)
├── appsettings.json                       (test configuration)
├── appsettings.local.json                 (.gitignored, local overrides)
└── README.md                              (this file)
```

## Development Workflow

### Project Reference vs. NuGet Package

This test site **uses a direct project reference** to `OC.Automate.Mastodon`:

```xml
<ItemGroup>
  <ProjectReference Include="..\OC.Automate.Mastodon\OC.Automate.Mastodon.csproj" />
</ItemGroup>
```

This means:
- ✅ Changes to the package code are reflected immediately (no pack/restore cycle)
- ✅ You can debug into the package code
- ✅ Hot reload works for both projects

### Testing the Final Package

Before release, verify the **packaged version** works:

1. Build and pack the main package:
   ```bash
   cd src/OC.Automate.Mastodon
   dotnet pack --configuration Release
   ```

2. Switch to NuGet reference (temporarily):
   ```xml
   <!-- Replace project reference with: -->
   <ItemGroup>
     <PackageReference Include="OC.Automate.Mastodon" Version="2.0.0" />
   </ItemGroup>
   ```

3. Test the site with the packaged version

4. Switch back to project reference for further development

## Environment Variables

Use `appsettings.local.json` (gitignored) for local testing:

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

Or set directly in your shell:

```bash
# Linux/macOS
export Umbraco__Automate__Secrets__MastodonAccessToken=your-token

# Windows PowerShell
$env:Umbraco__Automate__Secrets__MastodonAccessToken="your-token"
```

## Troubleshooting

### Port Already in Use

If port 5000 is taken, specify a different port:

```bash
dotnet run --urls "https://localhost:5001"
```

### Configuration Not Loading

Verify the `appsettings.json` is valid JSON and in the right location. Check the console logs for configuration errors.

### Changes Not Reflected

1. For **C# code changes**: Full rebuild may be needed
   ```bash
   dotnet clean
   dotnet build
   dotnet run
   ```

2. For **configuration changes**: Restart the site

### Database Locked

If the Umbraco database is locked, delete:

```bash
rm -rf App_Data/
```

The site will reinitialize on next run.

## Notes

- This is a **test/development site only** — not suitable for production
- The database is SQLite (created in `App_Data/`)
- Umbraco will run the setup wizard on first run (auto-completes with defaults)
- Credentials are not persisted in git (use `appsettings.local.json`)
