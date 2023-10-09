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
    public class AvailableProcessesController : ControllerBase
    {
        public class GroupOfProcesses
        {
            public string Group { get; set; }
            public List<BPProcess> Processes { get; set; }
        }

        private readonly ApplicationContext _db;

        private readonly ILogger<ResourcesController> _logger;

        public AvailableProcessesController(ILogger<ResourcesController> logger, ApplicationContext context)
        {
            _logger = logger;
            _db = context;
        }

        /// <summary>
        /// Returns list of published processes divided by a group (folder).
        /// Requires authentication token.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<GroupOfProcesses>> Get()
        {
            var processes = await _db.BPProcesses.AsNoTracking()
                                                 .ToListAsync();
            return processes.GroupBy(p => p.GroupName)
                            .Select(g => new GroupOfProcesses { Group = g.Key, Processes = g.ToList() })
                            .ToArray();
        }
    }
}
