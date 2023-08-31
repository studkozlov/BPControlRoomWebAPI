using BPControlRoomWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPControlRoomWebAPI.Controllers
{
    [ApiController]
    [Authorize]
    public class LogsController : ControllerBase
    {
        ApplicationContext _db;

        private readonly ILogger<ResourcesController> _logger;

        public LogsController(ILogger<ResourcesController> logger, ApplicationContext context)
        {
            _logger = logger;
            _db = context;
        }

        [HttpGet]
        [Route("[controller]/{sessionNum}")]
        public async Task<IEnumerable<BPLog>> Get(Int64 sessionNum)
        {
            return  await _db.BPLogs.Where(l => l.SessionNumber == sessionNum)
                                    .OrderBy(l => l.ResourceStart)
                                    .ThenBy(l => l.Id)
                                    .ToArrayAsync();
        }
    }
}
