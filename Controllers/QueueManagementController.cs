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
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class QueueManagementController : Controller
    {
        public class GroupOfWorkQueues
        {
            public string Group { get; set; }
            public List<BPWorkQueue> WorkQueues { get; set; }
        }

        ApplicationContext db;

        private readonly ILogger<ResourcesController> _logger;

        public QueueManagementController(ILogger<ResourcesController> logger, ApplicationContext context)
        {
            _logger = logger;
            db = context;
        }

        [HttpGet]
        public async Task<IEnumerable<GroupOfWorkQueues>> Get()
        {
            var workQueues = await db.BPWorkQueues.ToListAsync();
            return workQueues.GroupBy(wq => wq.WorkQueueGroupName)
                             .Select(g => new GroupOfWorkQueues { Group = g.Key, WorkQueues = g.ToList() })
                             .OrderByDescending(g => g.Group)
                             .ToArray();
        }
    }
}
