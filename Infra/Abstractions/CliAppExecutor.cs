using BPControlRoomWebAPI.Infra.Extensions;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BPControlRoomWebAPI.Infra.Abstractions
{
    public abstract class CliAppExecutor
    {
        private readonly string _appPath;
        protected string Arguments { get; set; }

        protected CliAppExecutor(string appPath)
        {
            _appPath = appPath;
        }

        protected string ExecuteCommand()
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _appPath,
                    Arguments = Arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            process.Start();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }
        protected async Task<string> ExecuteCommandAsync()
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _appPath,
                    Arguments = Arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            process.Start();
            await process.WaitForExitAsync();
            return process.StandardOutput.ReadToEnd();
        }
    }
}
