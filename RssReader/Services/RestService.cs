using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RssReader.Services
{
    public static class RestService
    {
        public static async Task<string> GetRssDetails(string uri)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage();

                var response = await client.GetAsync(uri);

                var resString = await response.Content.ReadAsStringAsync();

                return resString;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
