using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPControlRoomWebAPI.Models
{
    [Table("cstm_BPVQueueContents")]
    public class BPQueueItem
    {
        [Column("QueueItemId")]
        public Guid Id { get; set; }
        public string State { get; set; }
        public Guid QueueId { get; set; }
        public string ItemKey { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public string Tag { get; set; }
        public string Resource { get; set; }
        public int Attempt { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? NextReview { get; set; }
        public DateTime? Completed { get; set; }
        public int TotalWorkTime { get; set; }
        public DateTime? ExceptionDate { get; set; }
        public string ExceptionReason { get; set; }
    }
}
