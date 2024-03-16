using BPControlRoomWebAPI.Infra.Classes;
using BPControlRoomWebAPI.Infra.Abstractions;
using BPControlRoomWebAPI.Models;
using BPControlRoomWebAPI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPControlRoomWebAPI.Controllers
{
    [ApiController]
    [Authorize]
    public class EnvironmentController : ControllerBase
    {
        const string APP_ENVIRONMENT_SECTION = "Environment";
        const string APP_ENVIRONMENT_BPPATH = "AutomateCPath";

        private ApplicationContext _db;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ResourcesController> _logger;

        public EnvironmentController(ILogger<ResourcesController> logger, IConfiguration configuration, ApplicationContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _db = context;
        }

        /// <summary>
        /// Returns list of all sessions within last month.
        /// Requires authentication token.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]")]
        public async Task<IEnumerable<BPSession>> Get()
        {
            return await _db.BPSessions.AsNoTracking()
                                       .OrderByDescending(session => session.StartTime)
                                       .Where(s => s.Status != "Debugging" && s.StartTime > DateTime.Now.AddMonths(-1))
                                       .ToArrayAsync();
        }

        /// <summary>
        /// Creates a new session by starting selected process on a selected runtime resource with startup parameters (optionally).
        /// Requires authentication token.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[controller]")]
        public async Task<string> Post([FromBody]EnvironmentArgs args)
        {
            var authParams = new AuthenticationParams(User);
            var executor = GetExecutor<AutomatecRunProcessExecutor>();
            executor.SetParams(authParams.ConnectionName, authParams.Username, authParams.GetPasswordAsString(), args.ProcessName, args.ResourceName, args.InputParameters);
            await executor.ExecuteAsync();

            return executor.ExecutedSuccessfully ? "Success " + executor.ExecutionResult : executor.ExecutionResult;
        }

        /// <summary>
        /// Requests stop of a selected session.
        /// Requires authentication token.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[controller]/requeststop/{sessionId}")]
        public async Task<string> RequestStop(string sessionId)
        {
            var authParams = new AuthenticationParams(User);
            var executor = GetExecutor<AutomatecRequestStopExecutor>();
            executor.SetParams(authParams.ConnectionName, authParams.Username, authParams.GetPasswordAsString(), sessionId);
            await executor.ExecuteAsync();

            return executor.ExecutedSuccessfully ? "Success " : executor.ExecutionResult;
        }

        /// <summary>
        /// Immediately stops selected session.
        /// Requires authentication token.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[controller]/immediatestop/{sessionId}")]
        public async Task<string> ImmediateStop(string sessionId)
        {
            var session = await _db.BPSessions.FindAsync(new Guid(sessionId));
            if (session == null)
                return "Session not found.";

            var runtimeResourceName = session.Resource;
            var authParams = new AuthenticationParams(User);
            var executor = BpHttpExecutorsFactory.CreateExecutor<BpHttpImmediateStopExecutor>();
            executor.SetParams(runtimeResourceName, authParams.Username, authParams.GetPasswordAsString(), sessionId);
            await executor.ExecuteAsync();

            return executor.ExecutedSuccessfully ? "Success " : executor.ExecutionResult;
        }

        private IAutomatecExecutable GetExecutor<T>() where T: IAutomatecExecutable, new()
        {
            var appPath = _configuration.GetSection($"{APP_ENVIRONMENT_SECTION}:{APP_ENVIRONMENT_BPPATH}")
                                        .Get<string>();
            return String.IsNullOrEmpty(appPath)
                   ? AutomatecExecutorsFactory.CreateExecutor<T>()
                   : AutomatecExecutorsFactory.CreateExecutor<T>(appPath);
        }
    }
}
