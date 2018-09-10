using Newtonsoft.Json;
using PricingLibrary.FinancialProducts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.Models
{
    class JsonHandler
    {
        #region Private Fields
        private List<VanillaCall> listVanillaCalls;
        private List<BasketOption> listBasketOptions;
        #endregion Private Fields

        #region Public Properties
        public List<VanillaCall> ListVanillaCalls { get => listVanillaCalls;}
        public List<BasketOption> ListBasketOptions { get => listBasketOptions; }
        #endregion Public Properties

        public JsonHandler()
        {
            this.listVanillaCalls = new List<VanillaCall>();
            this.listBasketOptions = new List<BasketOption>();
        }

        public void LoadOptions()
        {
            string pathVanilla = Directory.GetCurrentDirectory() + "\\" + "VanillaCalls.json";
            string pathBasket = Directory.GetCurrentDirectory() + "\\" + "BasketOptions.json";
            if (File.Exists(pathVanilla))
            {
                using (StreamReader r = new StreamReader("VanillaCalls.json"))
                {
                    string jsonRead = r.ReadToEnd();
                    List<VanillaCall> items = JsonConvert.DeserializeObject<List<VanillaCall>>(jsonRead);
                    this.listVanillaCalls = items;
                }
            }
            
            if (File.Exists(pathBasket))
            {
                using (StreamReader r = new StreamReader("BasketOptions.json"))
                {
                    string jsonRead = r.ReadToEnd();
                    List<BasketOption> listBasket = new List<BasketOption>();
                    List<JsonBasket> items = JsonConvert.DeserializeObject<List<JsonBasket>>(jsonRead);
                    int size = 0;
                    foreach (JsonBasket jsonBasket in items)
                    {
                        size = jsonBasket.UnderlyingShareIds.Length;
                        Share[] listShares = new Share[size];
                        for (int i = 0; i < size; i++)
                        {
                            listShares[i] = new Share(jsonBasket.UnderlyingShareIds[i], jsonBasket.UnderlyingShareIds[i]);
                        }
                        BasketOption basketOption = new BasketOption(jsonBasket.Name, listShares, jsonBasket.Weights, jsonBasket.Maturity, jsonBasket.Strike);
                        listBasket.Add(basketOption);
                    }
                    this.listBasketOptions = listBasket;
                }
            }    
        }
        

        public void SaveOptionBasket(BasketOption option)
        {
            this.listBasketOptions.Add(option);
            this.listBasketOptions = listBasketOptions.Distinct().ToList();
            string json = JsonConvert.SerializeObject(this.listBasketOptions.ToArray());
            System.IO.File.WriteAllText("BasketOptions.json", json);
        }

        public void SaveOptionVanille(VanillaCall option)
        {           
            this.listVanillaCalls.Add(option);
            this.listVanillaCalls = this.listVanillaCalls.Distinct().ToList();
            string json = JsonConvert.SerializeObject(this.listVanillaCalls.ToArray());
            System.IO.File.WriteAllText("VanillaCalls.json", json);
        }

        public void DeleteOption(VanillaCall option)
        {
            bool deleted = this.listVanillaCalls.Remove(option);
            string json = JsonConvert.SerializeObject(this.listVanillaCalls.ToArray());
            System.IO.File.WriteAllText("VanillaCalls.json", json);
        }

        public void DeleteOption(BasketOption option)
        {
            bool deleted = this.listBasketOptions.Remove(option);
            string json = JsonConvert.SerializeObject(this.listBasketOptions.ToArray());
            System.IO.File.WriteAllText("BasketOptions.json", json);
        }
         
    }
}
