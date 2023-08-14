using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPControlRoomWebAPI.Models
{
    [Table("cstm_BPVEnvironment")]
    public class BPSession
    {
        [Column("SessionId")]
        public Guid Id { get; set; }
        [Column("ID")]
        public int Number { get; set; }
        public string Process { get; set; }
        public string Resource { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string LatestStage { get; set; }
    }
}
