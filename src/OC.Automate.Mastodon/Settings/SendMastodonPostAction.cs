using Microsoft.Extensions.Logging;
using OC.Automate.Mastodon.Factory;
using System.Net.Http.Json;
using Umbraco.Automate.Core.Actions;

namespace OC.Automate.Mastodon.Settings;

[Action("mastodonSendPost", "Send Mastodon Post", ConnectionTypeAlias = "mastodon")]
public class SendMastodonPostAction : ActionBase<MastodonPostSettings>
{
    private readonly MastodonClientFactory _clientFactory;
    private readonly ILogger<SendMastodonPostAction> _logger;

    public SendMastodonPostAction(
        ActionInfrastructure infrastructure,
        MastodonClientFactory clientFactory,
        ILogger<SendMastodonPostAction> logger)
        : base(infrastructure)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var connectionSettings = context.Connection?.GetSettings<MastodonConnectionSettings>();

        if (connectionSettings is null
            || string.IsNullOrWhiteSpace(connectionSettings.InstanceUrl)
            || string.IsNullOrWhiteSpace(connectionSettings.ConnectionName))
        {
            return ActionResult.Failed(
                new InvalidOperationException("No Mastodon connection configured."),
                StepRunErrorCategory.ConfigurationError);
        }

        if (!_clientFactory.TryCreateClient(connectionSettings.ConnectionName, out var client, out var tokenError))
        {
            return ActionResult.Failed(
                new InvalidOperationException(tokenError),
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

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{connectionSettings.InstanceUrl.TrimEnd('/')}/api/v1/statuses")
        {
            Content = JsonContent.Create(payload)
        };

        // Guard against duplicate toots if this step is retried: Mastodon returns the original
        // status for a repeated Idempotency-Key rather than creating a new one. RunId + StepId is
        // stable across retries of the same step but unique per step execution.
        request.Headers.Add("Idempotency-Key", $"{context.RunId}:{context.StepId}");

        _logger.LogInformation(
            "Posting to Mastodon instance {InstanceUrl}: {StatusLength} characters",
            connectionSettings.InstanceUrl, status.Length);

        var response = await client.SendAsync(request, cancellationToken);

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
