using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Umbraco.Automate.Core.Connections;
using Umbraco.Automate.Core.Settings;

namespace OC.Automate.Mastodon;

public class MastodonConnectionSettings
{
    [Field(Label = "Instance URL", Description = "The base URL of your Mastodon instance (e.g. https://mastodon.social).")]
    public string InstanceUrl { get; set; } = string.Empty;

    [Field(Label = "Connection Name", Description = "The key used to look up the access token in appsettings (OwainCodes:Automate:Mastodon:AccessTokens).", SortOrder = 1)]
    public string ConnectionName { get; set; } = string.Empty;
}

[ConnectionType("mastodon", "Mastodon")]
public class MastodonConnectionType : ConnectionTypeBase<MastodonConnectionSettings>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<MastodonSettings> _mastodonSettings;

    public MastodonConnectionType(
        ConnectionTypeInfrastructure infrastructure,
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<MastodonSettings> mastodonSettings)
        : base(infrastructure)
    {
        _httpClientFactory = httpClientFactory;
        _mastodonSettings = mastodonSettings;
    }

    public override async Task<ConnectionValidationResult> ValidateAsync(object? settings, CancellationToken cancellationToken)
    {
        var mastodonSettings = settings as MastodonConnectionSettings;

        if (string.IsNullOrWhiteSpace(mastodonSettings?.InstanceUrl))
            return ConnectionValidationResult.Failure("Instance URL is required.");

        if (string.IsNullOrWhiteSpace(mastodonSettings.ConnectionName))
            return ConnectionValidationResult.Failure("Connection name is required.");

        if (!_mastodonSettings.CurrentValue.AccessTokens.TryGetValue(mastodonSettings.ConnectionName, out var accessToken)
            || string.IsNullOrWhiteSpace(accessToken))
        {
            return ConnectionValidationResult.Failure(
                $"No access token found for connection name '{mastodonSettings.ConnectionName}' in appsettings (OwainCodes:Automate:Mastodon:AccessTokens).");
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

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
        public string? Username { get; set; }
        public string? Account { get; set; }
    }
}
