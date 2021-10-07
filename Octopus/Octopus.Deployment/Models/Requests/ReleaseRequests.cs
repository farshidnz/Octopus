using System.Collections.Generic;

namespace Octopus.Deployment.Models.Requests
{
    public record ReleaseRequests
    {
        public List<Release> Releases { get; init; }
        public List<Deployment> Deployments { get; init; }
        public List<Environment> Environments { get; init; }
    }
}