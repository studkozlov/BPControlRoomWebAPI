using BPControlRoomWebAPI.Infra.Abstractions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BPControlRoomWebAPI.Infra.Classes
{
    public class AutomatecRequestStopExecutor : AutomatecExecutor, IAutomatecExecutable
    {
        private const string _successResponseRegex = "Stop requested for session";

        public AutomatecRequestStopExecutor() : base()
        {
            Command = "/requeststop";
        }
        public AutomatecRequestStopExecutor(string automatecPath) : base(automatecPath)
        {
            Command = "/requeststop";
        }

        private void HandleResponse()
        {
            ExecutionResult = ExecutionResult.Trim();
            Regex regex = new Regex(_successResponseRegex);
            ExecutedSuccessfully = regex.IsMatch(ExecutionResult);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbconname"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="args">args[0] - session ID</param>
        public void SetParams(string dbconname, string username, string password, params string[] args)
        {
            InputParams = $"{args[0]} /user {username} {password} /dbconname \"{dbconname}\"";
        }
        public void Execute()
        {
            ExecutionResult = ExecuteCommand();
            HandleResponse();
        }
        public async Task ExecuteAsync()
        {
            ExecutionResult = await ExecuteCommandAsync();
            HandleResponse();
        }
    }
}
