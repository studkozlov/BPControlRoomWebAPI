using BPControlRoomWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public class Schedule
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string InitialTask { get; set; }
            public List<BPTask> Tasks { get; set; }
        }

        private readonly ApplicationContext _db;

        private readonly ILogger<ResourcesController> _logger;

        public SchedulesController(ILogger<ResourcesController> logger, ApplicationContext context)
        {
            _logger = logger;
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
    }
}
