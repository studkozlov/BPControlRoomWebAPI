using BPControlRoomWebAPI.Infra.Abstractions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BPControlRoomWebAPI.Infra.Classes
{
    public class AutomatecRunProcessExecutor : AutomatecExecutor, IAutomatecExecutable
    {
        private const string _successResponseRegex =
            @"Session:[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}";


        public AutomatecRunProcessExecutor() : base()
        {
            Command = "/run";
        }
        public AutomatecRunProcessExecutor(string automatecPath) : base(automatecPath)
        {
            Command = "/run";
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
        /// <param name="args">args[0] - process name, args[1] - resource name</param>
        public void SetParams(string dbconname, string username, string password, params string[] args)
        {
            InputParams = $"\"{args[0]}\" /resource {args[1]} /user {username} {password} /dbconname \"{dbconname}\"";
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
