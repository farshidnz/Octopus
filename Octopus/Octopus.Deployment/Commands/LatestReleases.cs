using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Octopus.Deployment.Models;
using Environment = Octopus.Deployment.Models.Environment;

namespace Octopus.Deployment.Commands
{
    public record LatestReleases : IRequest<List<Release>>
    {
        public string ProjectId { get; init; }
        public List<Release> Releases { get; init; }
        public List<Models.Deployment> Deployments { get; init; }
        public List<Environment> Environments { get; init; }

        public sealed class LatestReleasesHandler : IRequestHandler<LatestReleases, List<Release>>
        {
            private readonly ILogger<LatestReleasesHandler> _logger;

            public LatestReleasesHandler(ILogger<LatestReleasesHandler> logger)
            {
                _logger = logger;
            }

            public async Task<List<Release>> Handle(LatestReleases request, CancellationToken cancellationToken)
            {
                List<Release> keptReleases = new();
                var projectReleases = request.Releases.Where(r =>
                    string.Equals(r.ProjectId, request.ProjectId, StringComparison.CurrentCultureIgnoreCase));

                foreach (var environment in request.Environments)
                {
                    foreach (var release in projectReleases)
                    {
                        var recentDeployment = request.Deployments
                            .OrderByDescending(d => d.DeployedAt)
                            .FirstOrDefault(d =>
                                string.Equals(d.ReleaseId, release.Id, StringComparison.CurrentCultureIgnoreCase)
                                && string.Equals(d.EnvironmentId, environment.Id,
                                    StringComparison.CurrentCultureIgnoreCase));

                        if (recentDeployment != null)
                        {
                            _logger.LogInformation(
                                $"Most recent release with id {release.Id} with a deployment Id {recentDeployment!.Id} for project id {request.ProjectId}");
                            keptReleases.Add(release);
                            break;
                        }
                    }
                }

                return keptReleases;
            }

            public sealed class LatestReleasesHandlerValidator : AbstractValidator<LatestReleases>
            {
                public LatestReleasesHandlerValidator()
                {
                    RuleFor(request => request)
                        .NotNull();
                    
                    RuleFor(request => request.ProjectId)
                        .NotNull();
                }
            }
        }
    }
}