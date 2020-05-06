using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Podcastomatik.Shared.Models;

namespace Podcastomatik.Shared.Services
{
    public class DbConnector
    {
        private readonly string baseUrl = "http://10.51.189.122:45455";

        public async Task<IEnumerable<T>> GetAsync<T>(string apiRouteNoStartingSlash)
        {
            using (var webClient = new WebClient())
            {
                var webResult = await webClient.DownloadStringTaskAsync($"{baseUrl}/api/{apiRouteNoStartingSlash}");

                var converted = JsonConvert.DeserializeObject<IEnumerable<T>>(webResult);

                return converted;
            }
        }

        public async Task<IEnumerable<PodcastSearchResult>> Search(string searchText)
        {
            using (var webClient = new WebClient())
            {
                webClient.BaseAddress = baseUrl;
                webClient.QueryString.Add("searchText", searchText);

                var webResult = await webClient.DownloadStringTaskAsync("api/podcasts/search");

                var converted = JsonConvert.DeserializeObject<IEnumerable<PodcastSearchResult>>(webResult);

                return converted;
            }
        }
    }
}
