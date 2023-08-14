using Microsoft.EntityFrameworkCore;

namespace BPControlRoomWebAPI.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<BPResource> BPResources { get; set; } = null;
        public DbSet<BPWorkQueue> BPWorkQueues { get; set; } = null;
        public DbSet<BPQueueItem> BPQueueItems { get; set; } = null;
        public DbSet<BPSession> BPSessions { get; set; } = null;
        public DbSet<BPProcess> BPProcesses { get; set; } = null;
        public ApplicationContext (DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
    }
}
