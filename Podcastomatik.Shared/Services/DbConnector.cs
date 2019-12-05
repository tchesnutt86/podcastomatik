using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace Podcastomatik.Shared.Services
{
    public class DbConnector
    {
        private readonly string baseUrl = "http://10.51.189.122:45455/api";

        public async Task<IEnumerable<T>> GetAsync<T>(string apiRouteNoStartingSlash)
        {
            using (var webClient = new WebClient())
            {
                var webResult = await webClient.DownloadStringTaskAsync($"{baseUrl}/{apiRouteNoStartingSlash}");

                var converted = JsonConvert.DeserializeObject<IEnumerable<T>>(webResult);

                return converted;
            }
        }
    }
}
