using Umbraco.Automate.Core.Settings;

namespace OC.Automate.Mastodon;

public class MastodonPostSettings
{
    [Field(Label = "Content", Description = "The content of the post. Supports data bindings using ${ binding } syntax.", SupportsBindings = true)]
    public string Content { get; set; } = string.Empty;

    [Field(Label = "Visibility", Description = "Who can see this post: public, unlisted, private, or direct.", SortOrder = 1)]
    public string Visibility { get; set; } = "public";

    [Field(Label = "Post URL", Description = "Optional URL to append to the post.", SortOrder = 2, SupportsBindings = true)]
    public string? PostUrl { get; set; }

    [Field(Label = "Sensitive", Description = "Mark this post as containing sensitive content.", SortOrder = 3)]
    public bool IsSensitive { get; set; }

    [Field(Label = "Spoiler Text", Description = "Optional content warning text shown before the post.", SortOrder = 4, SupportsBindings = true)]
    public string? SpoilerText { get; set; }
}
