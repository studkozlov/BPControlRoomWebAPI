using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPControlRoomWebAPI.Models
{
    [Table("cstm_BPVQueueManagement")]
    public class BPWorkQueue
    {
        [Column("WorkQueueId")]
        public Guid Id { get; set; }
        public string WorkQueueName { get; set; }
        public string WorkQueueGroupName { get; set; }
        public string Status { get; set; }
        public int WorkedItems { get; set; }
        public int PendingItems { get; set; }
        public int ReferredItems { get; set; }
        public int TotalItems { get; set; }
        public int? AverageCaseDuration { get; set; }

    }
}
