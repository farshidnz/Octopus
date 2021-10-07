using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Octopus.Deployment.Models;
using Octopus.Deployment.Models.Requests;
using Xunit;
using Environment = Octopus.Deployment.Models.Environment;

namespace Octopus.Deployment.Tests.Scenarios
{
    public class OneEnvironmentOneDeployment : IDisposable
    {
        private readonly HttpResponseMessage _result;
        private readonly WebHost<Startup> _webHost;

        private readonly string _projectId = "Project-1";
        private readonly Release _expectedRelease;

        public OneEnvironmentOneDeployment()
        {
            _webHost = new WebHost<Startup>(services => { });

            var request = CreateRequest();
            _expectedRelease = request.Releases.First();

            var body = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                "application/json");

            _result = AsyncHelper.RunNow(() => _webHost.Client.PostAsync($"/Release/project/{_projectId}", body));
        }

        [Fact]
        public void Should_BeSuccessful_Response()
        {
            _result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_Be_Correct_Release()
        {
            var json = AsyncHelper.RunNow(() => _result.Content.ReadAsStringAsync());
            var response =JsonConvert.DeserializeObject<List<Release>>(json);

            response.Count.Should().Be(1);
            response.First().Id.Should().Be(_expectedRelease.Id);
            response.First().Version.Should().Be(_expectedRelease.Version);
            response.First().ProjectId.Should().Be(_expectedRelease.ProjectId);
            response.First().Created.Should().Be(_expectedRelease.Created);
        }

        private static ReleaseRequests CreateRequest()
        {
            return new ReleaseRequests()
            {
                Releases = new List<Release>
                {
                    new()
                    {
                        Created = new DateTime(2000, 01, 01, 08, 0, 0),
                        Id = "Release-1",
                        ProjectId = "project-1",
                        Version = "1.0.0"
                    }
                },
                Deployments = new List<Models.Deployment>
                {
                    new()
                    {
                        DeployedAt = new DateTime(2000, 01, 01, 10, 00, 00),
                        EnvironmentId = "Environment-1",
                        Id = "Deployment-1",
                        ReleaseId = "Release-1"
                    }
                },
                Environments = new List<Environment>
                {
                    new()
                    {
                        Id = "Environment-1",
                        Name = "Staging"
                    }
                }
            };
        }

        public void Dispose()
        {
            _result?.Dispose();
            _webHost?.Dispose();
        }
    }
}