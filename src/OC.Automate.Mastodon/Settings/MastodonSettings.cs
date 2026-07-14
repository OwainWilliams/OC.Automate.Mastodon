using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Umbraco.Automate.Core.Connections;
using Umbraco.Automate.Core.Settings;

namespace OC.Automate.Mastodon.Settings;

public sealed class MastodonSettings
{
    public const string SectionName = "Umbraco:Automate:Mastodon";

    [Field(Label = "Instance URL", Description = "The base URL of your Mastodon instance (e.g. https://umbracocommunity.social).")]
    public string InstanceUrl { get; set; } = string.Empty;

    [Field(
        Label = "Access Token",
        Description = "The access token required by Mastodon for this connection",
        IsSensitive = true,
        SortOrder = 1)]
    public string AccessToken { get; set; } = string.Empty;
}

[ConnectionType("mastodon", "Mastodon")]
public sealed class MastodonConnectionType : ConnectionTypeBase<MastodonSettings>
{
    public MastodonConnectionType(ConnectionTypeInfrastructure infrastructure)
        : base(infrastructure)
    {
    }

    public override async Task<ConnectionValidationResult> ValidateAsync(object? settings, CancellationToken cancellationToken)
    {
        var mastodonSettings = settings as MastodonSettings;

        if (string.IsNullOrWhiteSpace(mastodonSettings?.InstanceUrl))
            return ConnectionValidationResult.Failure("Instance URL is required.");

        if (string.IsNullOrWhiteSpace(mastodonSettings.AccessToken))
            return ConnectionValidationResult.Failure("Access token is required.");

        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", mastodonSettings.AccessToken);

            var response = await client.GetAsync(
                $"{mastodonSettings.InstanceUrl.TrimEnd('/')}/api/v1/accounts/verify_credentials",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return ConnectionValidationResult.Failure(
                    $"Authentication failed ({(int)response.StatusCode}). Check that your access token is valid.");

            var account = await response.Content.ReadFromJsonAsync<MastodonAccountResponse>(
                cancellationToken: cancellationToken);

            return ConnectionValidationResult.Success(
                $"Connected as @{account?.Account ?? account?.Username ?? "unknown"}.");
        }
        catch (HttpRequestException ex)
        {
            return ConnectionValidationResult.Failure($"Could not reach {mastodonSettings.InstanceUrl}: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return ConnectionValidationResult.Failure("Connection timed out.");
        }
    }

    private sealed class MastodonAccountResponse
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("acct")]
        public string? Account { get; set; }
    }
}
