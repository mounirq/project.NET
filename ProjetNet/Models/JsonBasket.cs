using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.Models
{
    
    class JsonBasket
    {
        #region Private Fields
        private string name;
        private string[] underlyingShareIds;
        private double[] weights;
        private DateTime maturity;
        private double strike;
        #endregion Private Fields

        #region Public Properties
        public string Name { get => name; }
        public string[] UnderlyingShareIds { get => underlyingShareIds; }
        public double[] Weights { get => weights; }
        public DateTime Maturity { get => maturity; }
        public double Strike { get => strike; }
        #endregion Public Properties

        public JsonBasket(string name, string[] UnderlyingShareIds , double[] weights, DateTime maturity, double strike)
        {
            this.name = name;
            this.underlyingShareIds = UnderlyingShareIds;
            this.weights = weights;
            this.maturity = maturity;
            this.strike = strike;
        }
        

    }
}
