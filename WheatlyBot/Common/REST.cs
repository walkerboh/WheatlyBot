using NLog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WheatlyBot.Common
{
    public static class REST
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static async Task<T> Get<T>(string baseUri, string requestUri = "")
        {
            T ret = default;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    var response = await client.GetAsync(requestUri);

                    if (response.IsSuccessStatusCode)
                        ret = await response.Content.ReadAsAsync<T>();
                    else
                    {
                        Logger.Warn(
                            $"Error sending GET to {baseUri}/{requestUri}. Response was {response.StatusCode}: {response.ReasonPhrase}.");
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error in REST.Get<T>");
                }
            }

            return ret;
        }
    }
}
