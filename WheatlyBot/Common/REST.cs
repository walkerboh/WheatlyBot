using NLog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WheatlyBot.Common
{
    public static class REST
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public static async Task<T> Get<T>(string baseURI, string requestURI = "")
        {
            T ret = default(T);

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(baseURI)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(requestURI);

                if (response.IsSuccessStatusCode)
                    ret = await response.Content.ReadAsAsync<T>();

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in REST.Get<T>");
            }

            return ret;
        }
    }
}
