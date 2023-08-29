using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace BPControlRoomWebAPI.Models
{
    [Table("cstm_BPVLogs")]
    public class BPLog
    {
        [Column("LogId")]
        public Int64 Id { get; set; }
        public Int32 SessionNumber { get; set; }
        public string StageName { get; set; }
        public string StageType { get; set; }
        public string Process { get; set; }
        public string Page { get; set; }
        public string Object { get; set; }
        public string Action { get; set; }
        public string Result { get; set; }
        public DateTime ResourceStart { get; set; }
        public DateTime? ResourceEnd { get; set; }
        public string Parameters { get; set; }
    }
}
