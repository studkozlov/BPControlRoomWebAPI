using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace BPControlRoomWebAPI.Infra.Abstractions
{
    public abstract class HttpExecutor
    {
        private readonly HttpClient client = new HttpClient();
        protected string HttpResource { get; set; }

        protected async Task<string> SendRequestAsync()
        {
            HttpResponseMessage response = await client.GetAsync(HttpResource);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
