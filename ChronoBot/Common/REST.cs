using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChronoBot.Common
{
    public static class REST
    {
        public static async Task<T> Get<T>(string baseURI, string requestURI = "")
        {
            T ret = default(T);

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(baseURI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(requestURI);

            if (response.IsSuccessStatusCode)
                ret = await response.Content.ReadAsAsync<T>();

            return ret;
        }
    }
}
