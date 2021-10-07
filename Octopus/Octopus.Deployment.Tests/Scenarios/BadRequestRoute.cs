using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Octopus.Deployment.Models;
using Xunit;

namespace Octopus.Deployment.Tests.Scenarios
{
    public class BadRequestRoute
    {
        private readonly HttpResponseMessage _result;
        private readonly WebHost<Startup> _webHost;

        public readonly string _projectId = "Project-1";
        private readonly Release _expectedRelease;

        public BadRequestRoute()
        {
            _webHost = new WebHost<Startup>(services => { });

            var body = new StringContent(JsonConvert.SerializeObject(null), Encoding.UTF8,
                "application/json");

            _result = AsyncHelper.RunNow(() => _webHost.Client.PostAsync($"/Release/project/{1}", body));
        }

        [Fact]
        public void Should_BeBadRequest_Response()
        {
            _result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void Dispose()
        {
            _result?.Dispose();
            _webHost?.Dispose();
        }
    }
}