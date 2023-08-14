using BPControlRoomWebAPI.Infra.Abstractions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BPControlRoomWebAPI.Infra.Classes
{
    public class AutomatecGetTokenExecutor : AutomatecExecutor, IAutomatecExecutable
    {
        private const string _successResponseRegex =
            @"[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}_[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}";

        public AutomatecGetTokenExecutor() : base()
        {
            Command = "/getauthtoken";
        }
        public AutomatecGetTokenExecutor(string automatecPath) : base(automatecPath)
        {
            Command = "/getauthtoken";
        }

        private void HandleResponse()
        {
            ExecutionResult = ExecutionResult.Trim();
            Regex regex = new Regex(_successResponseRegex);
            ExecutedSuccessfully = regex.IsMatch(ExecutionResult);
        }
        public void SetParams(string dbconname, string username, string password, params string[] args)
        {
            InputParams = $"/user {username} {password} /dbconname \"{dbconname}\"";
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
