using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPControlRoomWebAPI.Infra.Abstractions;

namespace BPControlRoomWebAPI.Infra.Classes
{
    public class BpHttpImmediateStopExecutor : HttpExecutor, IBpHttpExecutable
    {
        private const string _httpResourceMask = @"http://{0}:8181/user%20name%20{1}&password%20{2}&stop%20{3}";
        private const string _successIndicator = "STOPPING";
        
        public string ExecutionResult { get; set; }
        public bool ExecutedSuccessfully { get; set; }

        public void SetParams(string runtimeResourceName, string username, string password, params string[] args)
        {
            this.HttpResource = String.Format(_httpResourceMask, runtimeResourceName, username, password, args[0]);
        }
        public async Task ExecuteAsync()
        {
            ExecutionResult = await SendRequestAsync();
            ExecutedSuccessfully = ExecutionResult.Contains(_successIndicator);
        }
    }
}
