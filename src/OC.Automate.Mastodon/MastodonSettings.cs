namespace OC.Automate.Mastodon;

public class MastodonSettings
{
    public const string SectionName = "OwainCodes:Automate:Mastodon";

    /// <summary>
    /// Access tokens keyed by the connection name set in the backoffice.
    /// </summary>
    public Dictionary<string, string> AccessTokens { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
