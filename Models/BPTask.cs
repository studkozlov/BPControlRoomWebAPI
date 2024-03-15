using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPControlRoomWebAPI.Models
{
    [Table("cstm_BPVTasks")]
    public class BPTask
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public string ScheduleDescription { get; set; }
        public string InitialTask { get; set; }
        public string TaskOnComplete { get; set; }
        public string TaskOnException { get; set; }
    }
}
