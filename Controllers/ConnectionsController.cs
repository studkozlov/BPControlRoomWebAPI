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

namespace BPControlRoomWebAPI.Controllers
{
    [ApiController]
    [AllowAnonymous]
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

        [HttpGet]
        [Route("[controller]")]
        public IEnumerable<string> Get()
        {
            return _configuration.GetSection($"{APP_CONNECTIONS_SECTION}:{APP_CONNECTIONS_LIST_KEY}")
                                 .Get<string[]>();
        }

        [HttpPut]
        [Route("[controller]/login")]
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
    }
}
