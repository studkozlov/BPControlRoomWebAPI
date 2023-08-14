using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPControlRoomWebAPI.Models.ViewModels
{
    public class AuthenticationArgs
    {
        public string ConnectionName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
