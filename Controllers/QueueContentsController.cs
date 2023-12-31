﻿using BPControlRoomWebAPI.Models;
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
    public class QueueContentsController : ControllerBase
    {
        ApplicationContext db;

        private readonly ILogger<ResourcesController> _logger;

        public QueueContentsController(ILogger<ResourcesController> logger, ApplicationContext context)
        {
            _logger = logger;
            db = context;
        }

        /// <summary>
        /// Returns list of work queue items for a selected work queue.
        /// Requires authentication token.
        /// </summary>
        /// <param name="queueId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/{queueId}")]
        public async Task<IEnumerable<BPQueueItem>> Get(Guid queueId)
        {
            return await db.BPQueueItems.AsNoTracking()
                                        .Where(item => item.QueueId == queueId)
                                        .OrderByDescending(item => item.Created)
                                        .ToArrayAsync();
        }
    }
}
