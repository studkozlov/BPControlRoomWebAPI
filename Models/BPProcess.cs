using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPControlRoomWebAPI.Models
{
    [Table("cstm_BPVAvailableProcesses")]
    public class BPProcess
    {
        [Column("ProcessId")]
        public Guid Id { get; set; }
        public string ProcessName { get; set; }
        public string ProcessDescription { get; set; }
        public string GroupName { get; set; }
    }
}
