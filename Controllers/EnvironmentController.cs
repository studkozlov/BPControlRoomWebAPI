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

        [HttpGet]
        [Route("[controller]")]
        public async Task<IEnumerable<BPSession>> Get()
        {
            return await _db.BPSessions.OrderByDescending(session => session.StartTime)
                                      .ToArrayAsync();
        }

        [HttpPost]
        [Route("[controller]")]
        public async Task<string> Post([FromBody]EnvironmentArgs args)
        {
            var authParams = new AuthenticationParams(User);
            var executor = GetExecutor<AutomatecRunProcessExecutor>();
            executor.SetParams(authParams.ConnectionName, authParams.Username, authParams.GetPasswordAsString(), args.ProcessName, args.ResourceName);
            await executor.ExecuteAsync();

            return executor.ExecutedSuccessfully ? "Success " + executor.ExecutionResult : executor.ExecutionResult;
        }

        [HttpDelete]
        [Route("[controller]/{sessionId}")]
        public async Task<string> Delete(string sessionId)
        {
            var authParams = new AuthenticationParams(User);
            var executor = GetExecutor<AutomatecRequestStopExecutor>();
            executor.SetParams(authParams.ConnectionName, authParams.Username, authParams.GetPasswordAsString(), sessionId);
            await executor.ExecuteAsync();

            return executor.ExecutedSuccessfully ? "Successful" : executor.ExecutionResult;
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
