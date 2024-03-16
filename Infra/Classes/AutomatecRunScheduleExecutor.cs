using BPControlRoomWebAPI.Infra.Abstractions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BPControlRoomWebAPI.Infra.Classes
{
    public class AutomatecRunScheduleExecutor : AutomatecExecutor, IAutomatecExecutable
    {
        private const string _successResponseRegex =
            @"Schedule.{3,}set to run at";


        public AutomatecRunScheduleExecutor() : base()
        {
            Command = "/startschedule";
        }
        public AutomatecRunScheduleExecutor(string automatecPath) : base(automatecPath)
        {
            Command = "/startschedule";
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
        /// <param name="args">args[0] - schedule name</param>
        public void SetParams(string dbconname, string username, string password, params string[] args)
        {
            InputParams = $"/schedule \"{args[0]}\" /user {username} {password} /dbconname \"{dbconname}\"";
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
