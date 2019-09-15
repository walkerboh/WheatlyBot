using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WheatlyBot.Entities.ChronoGG;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WheatlyBot.Settings;

namespace WheatlyBot.Services
{
    public class ChronoGgApi
    {
        public ILogger<ChronoGgApi> Logger { get; set; }

        private readonly ChronoGgSettings _settings;

        public ChronoGgApi(IOptions<ChronoGgSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<Sale> GetCurrentSaleAsync()
        {
            Sale sale = null;

            try
            {
                using(var client = new HttpClient())
                {
                    var response = await client.GetAsync(_settings.SaleUri);
                    response.EnsureSuccessStatusCode();
                    sale = await response.Content.ReadAsAsync<Sale>();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in ChronoGgApi.GetCurrentSaleAsync");
            }

            return sale;
        }

        public async Task<Shop> GetShopAsync()
        {
            Shop shop = null;

            try
            {
                IEnumerable<ShopItem> items;

                using(var client = new HttpClient())
                {
                    var response = await client.GetAsync(_settings.ShopUri);
                    response.EnsureSuccessStatusCode();
                    items = await response.Content.ReadAsAsync<IEnumerable<ShopItem>>();
                }

                shop = new Shop(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in ChronoGgApi.GetShopAsync");
            }

            return shop;
        }
    }
}
