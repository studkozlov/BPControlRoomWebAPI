using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPControlRoomWebAPI.Models
{
    [Table("cstm_BPVResources")]
    public class BPResource
    {
        [Column("ResourceId")]
        public Guid Id { get; set; }
        public string ResourceName { get; set; }
        public string ResourceGroupName { get; set; }
        public string ResourceStatus { get; set; }
    }
}
