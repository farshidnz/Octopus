using System;

namespace Octopus.Deployment.Models
{
    public class Deployment
    {
        public string Id { get; init; }
        public string ReleaseId { get; init; }
        public string EnvironmentId { get; init; }
        public DateTime DeployedAt { get; init; }
    }
}