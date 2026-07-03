namespace OC.Automate.Mastodon.Settings;

public class MastodonSettings
{
    public const string SectionName = "Umbraco:Automate:Providers:OCAutomateMastodon";

    /// <summary>
    /// Access tokens keyed by the connection name set in the backoffice.
    /// </summary>
    public Dictionary<string, string> AccessTokens { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
