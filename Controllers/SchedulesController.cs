using BPControlRoomWebAPI.Infra.Abstractions;
using BPControlRoomWebAPI.Infra.Classes;
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
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SchedulesController : ControllerBase
    {
        const string APP_ENVIRONMENT_SECTION = "Environment";
        const string APP_ENVIRONMENT_BPPATH = "AutomateCPath";

        public class Schedule
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string InitialTask { get; set; }
            public List<BPTask> Tasks { get; set; }
        }

        private readonly ApplicationContext _db;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ResourcesController> _logger;

        public SchedulesController(ILogger<ResourcesController> logger, IConfiguration configuration, ApplicationContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _db = context;
        }

        /// <summary>
        /// Returns list of active schedules with their tasks.
        /// Requires authentication token.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Schedule>> Get()
        {
            var tasks = await _db.BPTasks.AsNoTracking()
                                                 .ToListAsync();
            return tasks.GroupBy(t => t.ScheduleId)
                            .Select(g => new Schedule
                            {
                                Id = g.Key,
                                Name = g.ToList().FirstOrDefault().ScheduleName,
                                Description = g.ToList().FirstOrDefault().ScheduleDescription,
                                InitialTask = g.ToList().FirstOrDefault().InitialTask,
                                Tasks = g.ToList()
                            })
                            .ToArray();
        }

        /// <summary>
        /// Runs selected schedule within 30 seconds.
        /// Requires authentication token.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Post([FromBody] ScheduleArgs args)
        {
            var authParams = new AuthenticationParams(User);
            var executor = GetExecutor<AutomatecRunScheduleExecutor>();
            executor.SetParams(authParams.ConnectionName, authParams.Username, authParams.GetPasswordAsString(), args.ScheduleName);
            await executor.ExecuteAsync();

            return executor.ExecutedSuccessfully ? "Success " + executor.ExecutionResult : executor.ExecutionResult;
        }

        private IAutomatecExecutable GetExecutor<T>() where T : IAutomatecExecutable, new()
        {
            var appPath = _configuration.GetSection($"{APP_ENVIRONMENT_SECTION}:{APP_ENVIRONMENT_BPPATH}")
                                        .Get<string>();
            return String.IsNullOrEmpty(appPath)
                   ? AutomatecExecutorsFactory.CreateExecutor<T>()
                   : AutomatecExecutorsFactory.CreateExecutor<T>(appPath);
        }
    }
}
