using System.Threading.Tasks;

namespace BPControlRoomWebAPI.Infra.Abstractions
{
    public interface IAutomatecExecutable
    {
        public bool ExecutedSuccessfully { get; set; }
        public string ExecutionResult { get; set; }
        public void SetParams(string dbconname, string username, string password, params string[] args);
        public void Execute();
        public Task ExecuteAsync();
    }
}
