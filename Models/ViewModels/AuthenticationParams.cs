using System.Net;
using System.Security;
using System.Security.Claims;

namespace BPControlRoomWebAPI.Models.ViewModels
{
    public class AuthenticationParams
    {
        public string ConnectionName { get; set; }
        public string Username { get; set; }
        public SecureString Password { get; set; }
        public string PasswordAsString
        {
            get
            {
                return new NetworkCredential("", Password).Password;
            }
        }

        public AuthenticationParams(ClaimsPrincipal user)
        {
            ConnectionName = user.FindFirst("dbconname")?.Value;
            Username = user.Identity.Name;
            Password = new NetworkCredential("", user.FindFirst("password")?.Value).SecurePassword;
        }
    }
}
