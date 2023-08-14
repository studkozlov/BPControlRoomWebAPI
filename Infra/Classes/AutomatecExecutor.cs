using BPControlRoomWebAPI.Infra.Abstractions;

namespace BPControlRoomWebAPI.Infra.Classes
{
    public class AutomatecExecutor : CliAppExecutor
    {
        private const string _automatecPath =
            @"C:\Program Files\Blue Prism Limited\Blue Prism Automate\automatec.exe";
        public AutomatecExecutor() : base(_automatecPath) { }
        public AutomatecExecutor(string automatecPath) : base(automatecPath) { }

        private string _command;
        private string _inputParams;
        protected string Command
        {
            get
            {
                return this._command;
            }
            set
            {
                this._command = value;
                this.Arguments = $"{this._command} {this._inputParams}";
            }
        }
        protected string InputParams
        {
            get
            {
                return this._inputParams;
            }
            set
            {
                this._inputParams = value;
                this.Arguments = $"{this._command} {this.InputParams}";
            }
        }
        public string ExecutionResult { get; set; }
        public bool ExecutedSuccessfully { get; set; }
    }
}
