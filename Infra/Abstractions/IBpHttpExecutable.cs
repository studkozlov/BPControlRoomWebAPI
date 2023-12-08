using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPControlRoomWebAPI.Infra.Abstractions
{
    public interface IBpHttpExecutable
    {
        public bool ExecutedSuccessfully { get; set; }
        public string ExecutionResult { get; set; }
        public void SetParams(string runtimeResourceName, string username, string password, params string[] args);
        public Task ExecuteAsync();
    }
}
