using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Umbraco.Automate.Core.Actions;

namespace OC.Automate.Mastodon;

[Action("mastodonSendPost", "Send Mastodon Post", ConnectionTypeAlias = "mastodon")]
public class SendMastodonPostAction : ActionBase<MastodonPostSettings>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SendMastodonPostAction> _logger;

    public SendMastodonPostAction(
        ActionInfrastructure infrastructure,
        IHttpClientFactory httpClientFactory,
        ILogger<SendMastodonPostAction> logger)
        : base(infrastructure)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var connectionSettings = context.Connection?.GetSettings<MastodonConnectionSettings>();

        if (connectionSettings is null
            || string.IsNullOrWhiteSpace(connectionSettings.InstanceUrl)
            || string.IsNullOrWhiteSpace(connectionSettings.AccessToken))
        {
            return ActionResult.Failed(
                new InvalidOperationException("No Mastodon connection configured."),
                StepRunErrorCategory.ConfigurationError);
        }

        var settings = context.GetSettings<MastodonPostSettings>();

        if (string.IsNullOrWhiteSpace(settings.Content))
            return ActionResult.Failed(
                new InvalidOperationException("Post content is required."),
                StepRunErrorCategory.Validation);

        var status = settings.Content.Trim();
        if (!string.IsNullOrWhiteSpace(settings.PostUrl))
            status = $"{status}\n\n{settings.PostUrl}";

        var payload = new
        {
            status,
            visibility = string.IsNullOrWhiteSpace(settings.Visibility) ? "public" : settings.Visibility,
            sensitive = settings.IsSensitive,
            spoiler_text = settings.SpoilerText
        };

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", connectionSettings.AccessToken);

        _logger.LogInformation(
            "Posting to Mastodon instance {InstanceUrl}: {StatusLength} characters",
            connectionSettings.InstanceUrl, status.Length);

        var response = await client.PostAsJsonAsync(
            $"{connectionSettings.InstanceUrl.TrimEnd('/')}/api/v1/statuses",
            payload,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            return ActionResult.Failed(
                new InvalidOperationException($"Mastodon API returned {response.StatusCode}: {error}"),
                StepRunErrorCategory.InvalidResponse);
        }

        return Success();
    }
}
