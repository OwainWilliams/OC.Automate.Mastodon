using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

namespace OC.Automate.Mastodon.Composers
{
    public class MastodonPackageManifestReader : IPackageManifestReader
    {
        public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
        {
            var version = typeof(MastodonPackageManifestReader).Assembly.GetName().Version?.ToString() ?? "1.0.0";
            return Task.FromResult<IEnumerable<PackageManifest>>(new[]
            {
                new PackageManifest
                {
                    Id = "OC.Automate.Mastodon",
                    Name = "OC Automate Mastodon",
                    Version = version,
                    AllowTelemetry = true,
                    Extensions = []

                }
            });
        }
    }
}
