using System;

namespace Octopus.Deployment.Models
{
    public sealed record Release
    {
        public string Id { get; init; }
        public string ProjectId { get; init; }
        public string Version { get; init; }
        public DateTime Created { get; init; }
    }
}