using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WheatlyBot.Entities.Meh;
using WheatlyBot.Settings;

namespace WheatlyBot.Services
{
    public class MehApi
    {
        public ILogger<MehApi> Logger { get; set; }

        private readonly MehSettings _settings;

        public MehApi(IOptions<MehSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<ApiResponse> GetMehDataAsync()
        {
            ApiResponse apiResponse = null;

            try
            {
                var uri = string.Format(_settings.DataUri, _settings.ApiKey);

                using(var client = new HttpClient())
                {
                    var response = await client.GetAsync(uri);
                    response.EnsureSuccessStatusCode();
                    apiResponse = await response.Content.ReadAsAsync<ApiResponse>();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in MehApi.GetMehDataAsync");
            }

            return apiResponse;
        }
    }
}