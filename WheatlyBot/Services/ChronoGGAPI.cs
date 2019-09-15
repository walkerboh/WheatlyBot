using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WheatlyBot.Common;
using WheatlyBot.Entities.ChronoGG;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WheatlyBot.Settings;

namespace WheatlyBot.Services
{
    public class ChronoGGAPI
    {
        public ILogger<ChronoGGAPI> Logger { get; set; }

        private readonly ChronoGgSettings _settings;

        public ChronoGGAPI(IOptions<ChronoGgSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<Sale> GetCurrentSaleAsync()
        {
            Sale sale = null;

            try
            {
                sale = await REST.Get<Sale>(_settings.SaleUri);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in ChronoGGAPI.GetCurrentSaleAsync");
            }

            return sale;
        }

        public async Task<Shop> GetShopAsync()
        {
            Shop shop = null;

            try
            {
                IEnumerable<ShopItem> items = await REST.Get<IEnumerable<ShopItem>>(_settings.ShopUri);
                shop = new Shop(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in ChronoGGAPI.GetShopAsync");
            }

            return shop;
        }
    }
}
