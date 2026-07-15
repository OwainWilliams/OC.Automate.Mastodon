using OC.Automate.Mastodon.Settings;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Umbraco.Automate.Core.Connections;

namespace OC.Automate.Mastodon.ConnectionTypes
{

    [ConnectionType("mastodon", "Mastodon",
        Description = "Connect to Mastodon",
        Group = "Custom",
        Icon = "icon-plug")]
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
}
