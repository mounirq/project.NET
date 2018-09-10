using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjetNet.Models;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;

namespace ProjetNet.Test
{
    class JsonHandlerTest
    {
        public static void Main(string[] args)
        {
            Share share1 = new Share("vodName", "vod.l");
            Share share2 = new Share("ftse", "ftse");
            string[] shareNames = new string[2] { "vodName", "ftseName" };
            string nameBasket = "Basket";
            double strikeBasket = 9;
            Share[] sharesBasket = { share1, share2 };
            Double[] weights = { 0.3, 0.7 };
            DateTime maturityBasket = new DateTime(2019, 09, 04);
            BasketOption optionBasket = new BasketOption(nameBasket, sharesBasket, weights, maturityBasket, strikeBasket);
            BasketOption optionBasket2 = new BasketOption("BASKETTT", sharesBasket, weights, maturityBasket, strikeBasket);

            VanillaCall optionVanille = new VanillaCall("Vanille", sharesBasket[0], maturityBasket, strikeBasket);
            VanillaCall optionVanille2 = new VanillaCall("Vanille2", sharesBasket[0], maturityBasket, strikeBasket);



            JsonHandler jsonHandle = new JsonHandler();
            jsonHandle.SaveOption(optionVanille);
            jsonHandle.SaveOption(optionVanille);
            jsonHandle.SaveOption(optionVanille2);
            jsonHandle.SaveOption(optionBasket);
            jsonHandle.SaveOption(optionBasket2);
            jsonHandle.SaveOption(optionBasket);

            jsonHandle.SaveOption(optionBasket);
            jsonHandle.LoadOptions();
            List<VanillaCall> listOptions = jsonHandle.ListVanillaCalls;
            List<BasketOption> listBasket = jsonHandle.ListBasketOptions;
        }
    }
}
