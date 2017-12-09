using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChronoBot.Common;
using ChronoBot.Entities.ChronoGG;
using Microsoft.Extensions.Logging;

namespace ChronoBot.Services
{
    public class ChronoGGAPI
    {
        public ILogger<ChronoGGAPI> Logger { get; set; }

        private const string SaleURI = @"https://api.chrono.gg/sale";

        private const string ShopURI = @"https://api.chrono.gg/shop";

        public async Task<Sale> GetCurrentSaleAsync()
        {
            Sale sale = null;

            try
            {
                sale = await REST.Get<Sale>(SaleURI);
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
                IEnumerable<ShopItem> items = await REST.Get<IEnumerable<ShopItem>>(ShopURI);
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
