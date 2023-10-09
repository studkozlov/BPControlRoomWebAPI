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
    public class ResourcesController : ControllerBase
    {
        public class GroupOfResources
        {
            public string Group { get; set; }
            public List<BPResource> Resources { get; set; }
        }

        ApplicationContext _db;

        private readonly ILogger<ResourcesController> _logger;

        public ResourcesController(ILogger<ResourcesController> logger, ApplicationContext context)
        {
            _logger = logger;
            _db = context;
        }

        /// <summary>
        /// Returns list of all runtime resources divided by group (folder) name.
        /// Requires authentication token.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<GroupOfResources>> Get()
        {
            var resources = await _db.BPResources.AsNoTracking()
                                                 .ToListAsync();
            return resources.GroupBy(resource => resource.ResourceGroupName)
                            .Select(g => new GroupOfResources { Group = g.Key, Resources=g.ToList()})
                            .ToArray();
        }
    }
}
