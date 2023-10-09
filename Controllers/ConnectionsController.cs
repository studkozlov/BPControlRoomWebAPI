using BPControlRoomWebAPI.Infra.Classes;
using BPControlRoomWebAPI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace BPControlRoomWebAPI.Controllers
{
    [ApiController]
    public class ConnectionsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        const string APP_CONNECTIONS_SECTION = "Connections";
        const string APP_CONNECTIONS_LIST_KEY = "Names";
        const string APP_ENVIRONMENT_SECTION = "Environment";
        const string APP_ENVIRONMENT_BPPATH = "AutomateCPath";

        public ConnectionsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Returns list of available Blue Prism connections.
        /// Authentication is not needed.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]")]
        [AllowAnonymous]
        public IEnumerable<string> Get()
        {
            return _configuration.GetSection($"{APP_CONNECTIONS_SECTION}:{APP_CONNECTIONS_LIST_KEY}")
                                 .Get<string[]>();
        }

        /// <summary>
        /// Authorizes an user by username and password.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[controller]/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticationArgs args)
        {
            var appPath = _configuration.GetSection($"{APP_ENVIRONMENT_SECTION}:{APP_ENVIRONMENT_BPPATH}")
                                        .Get<string>();
            var executor = String.IsNullOrEmpty(appPath)
                                            ? AutomatecExecutorsFactory.CreateExecutor<AutomatecGetTokenExecutor>()
                                            : AutomatecExecutorsFactory.CreateExecutor<AutomatecGetTokenExecutor>(appPath);
            executor.SetParams(args.ConnectionName, args.Username, args.Password);
            await executor.ExecuteAsync();

            if (!executor.ExecutedSuccessfully)
                return Unauthorized(executor.ExecutionResult);

            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.Name, args.Username),
                new Claim("password", args.Password),
                new Claim("dbconname", args.ConnectionName)
            };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            
                return Content(new JwtSecurityTokenHandler().WriteToken(jwt));
        }

        /// <summary>
        /// Authorizes an user by AzureAD ID token.
        /// The method accepts requests only from users authenticated through Azure AD (by idToken).
        /// The method checks user's AD groups, compares with BP users table and either provides regular auth token or rejects auth request.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[controller]/loginsso")]
        [Authorize]
        public IActionResult AuthenticateSso([FromBody] AuthenticationArgs args)
        {
            //Check User's AD groups and compare with BP DB. If match, assign proper role and return generated token

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, User.FindFirst("preferred_username")?.Value),//get user's email from User object
                new Claim(ClaimTypes.Name, "bpsacc"),//service account
                new Claim("password", "bpsacc_pass"),//service account password
                new Claim("dbconname", args.ConnectionName)
            };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return Content(new JwtSecurityTokenHandler().WriteToken(jwt));
        }
    }
}
