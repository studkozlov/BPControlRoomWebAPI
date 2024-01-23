using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace BPControlRoomWebAPI.Models
{
    [Table("cstm_BPVAvailableProcesses")]
    public class BPProcess
    {
        private string _processXml;

        [Column("ProcessId")]
        public Guid Id { get; set; }
        public string ProcessName { get; set; }
        public string ProcessDescription { get; set; }
        public string ProcessInput
        {
            get
            {
                return this._processXml;
            }
            set
            {
                var startStageFirstIndex = value.IndexOf("type=\"Start\"");
                var startStageSecondIndex = value.IndexOf("</stage>", startStageFirstIndex);
                var cutXml = value.Substring(startStageFirstIndex, startStageSecondIndex - startStageFirstIndex);
                var regex = new Regex("<inputs>(.|\n)*?</inputs>");
                var match = regex.Match(cutXml);
                this._processXml = match.Success ? match.Value : string.Empty;
            }
        }
        public string GroupName { get; set; }
    }
}
