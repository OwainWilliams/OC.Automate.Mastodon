using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace OC.Automate.Mastodon;

/// <summary>
/// Resolves Mastodon access tokens and builds authenticated <see cref="HttpClient"/> instances.
/// Shared by the connection validator and the send-post action so token lookup, the bearer
/// header, the request timeout and the "no token" message live in one place.
/// </summary>
public sealed class MastodonClientFactory
{
    /// <summary>Request timeout applied to every call made to a Mastodon instance.</summary>
    public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(15);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<MastodonSettings> _settings;

    public MastodonClientFactory(
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<MastodonSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings;
    }

    /// <summary>
    /// Looks up the access token for <paramref name="connectionName"/> and, when found, returns an
    /// <see cref="HttpClient"/> pre-configured with the bearer token and a request timeout.
    /// </summary>
    /// <returns>
    /// <c>true</c> if a token was found and a client created; otherwise <c>false</c> with
    /// <paramref name="error"/> describing the missing configuration.
    /// </returns>
    public bool TryCreateClient(string connectionName, out HttpClient client, out string error)
    {
        client = null!;
        error = string.Empty;

        if (!_settings.CurrentValue.AccessTokens.TryGetValue(connectionName, out var accessToken)
            || string.IsNullOrWhiteSpace(accessToken))
        {
            error = $"No access token found for connection name '{connectionName}' in appsettings ({MastodonSettings.SectionName}:AccessTokens).";
            return false;
        }

        client = _httpClientFactory.CreateClient();
        client.Timeout = RequestTimeout;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return true;
    }
}
