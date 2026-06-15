using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Connections;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace OC.Automate.Mastodon;

public class MastodonComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.WithCollectionBuilder<ConnectionTypeCollectionBuilder>()
            .Add<MastodonConnectionType>();

        builder.WithCollectionBuilder<ActionCollectionBuilder>()
            .Add<SendMastodonPostAction>();
    }
}
