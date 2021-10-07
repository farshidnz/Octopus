using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Octopus.Deployment.Commands;
using Octopus.Deployment.Models;
using Octopus.Deployment.Models.Requests;

namespace Octopus.Deployment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReleaseController : ControllerBase
    {
        private readonly ISender _sender;

        public ReleaseController(ISender sender)
        {
            _sender = sender;
        }
        /// <summary>
        ///     Given projects and environment, determine what releases should be kept based on deployments
        /// </summary>
        /// <param name="projectId">The Id of the project that we are determining the number of releases</param>
        /// <param name="request"></param>
        /// <remarks>
        ///     Determine what releases should be kept
        /// </remarks>
        /// <response code="400">Bad request</response>
        /// <response code="500">Unknown Error</response>
        /// <response code="Default">Unknown Error</response>
        /// <returns>Returns list of releases to keep</returns>
        [HttpPost("project/{projectId}")]
        [OpenApiOperation("determine-releases")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(List<Release>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Release>>> DetermineReleases([FromRoute] string projectId, [FromBody] ReleaseRequests request)
        {
            var result = await _sender.Send(new MostRecentReleases
            {
                Deployments = request.Deployments,
                Environments = request.Environments,
                Releases = request.Releases,
                ProjectId = projectId
            });
            return result;
        }
    }
}