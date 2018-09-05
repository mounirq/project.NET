using PricingLibrary.FinancialProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.Models
{
    internal class UserInput
    {
        #region Private Fields

        private String _optionType;
        private DateTime _maturity;
        private double _strike;
        private String[] _undelyingsNames;
        private Double[] _weights;
        private DateTime _startDateTest;
        private IDataProvider _dataType;
        private int _estimationStep;

        #endregion Private Fields

        #region Public Constructors

        public UserInput(){ }

        public UserInput(string optionType, DateTime maturity, double strike, string[] undelyingsNames, double[] weights, DateTime startDateTest, IDataProvider dataType, int estimationStep)
        {
            _optionType = optionType;
            _maturity = maturity;
            _strike = strike;
            _undelyingsNames = undelyingsNames;
            _weights = weights;
            _startDateTest = startDateTest;
            _dataType = dataType;
            _estimationStep = estimationStep;
        }

        #endregion Public Constructors

        #region Public Properties

        public String OptionType
        {
            get { return _optionType; }
            set { _optionType = value; }
        }

        public DateTime Maturity
        {
            get { return _maturity; }
            set { _maturity = value; }
        }

        public Double Strike
        {
            get { return _strike; }
            set { _strike = value; }
        }

        public String[] UnderlyingsNames
        {
            get { return _undelyingsNames; }
            set { _undelyingsNames = value; }
        }

        public Double[] Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        public DateTime StartDateTest
        {
            get { return _startDateTest; }
            set { _startDateTest = value; }
        }

        public IDataProvider DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public int EstimationStep
        {
            get { return _estimationStep; }
            set {
                _estimationStep = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public IOption constructOption() 
        {
            IOption option = null;

            if (!(Weights==null) && Weights.Sum() != 1)
            {
                throw new ArgumentException("The sum of the weights must equal");
            }
            List<Share> underlyingsShares = new List<Share>();
            foreach (string underlying in UnderlyingsNames)
            {
                underlyingsShares.Add(new Share(underlying, "id_" + underlying));
            }
            if (OptionType.Equals("VanillaCall"))
            {
                option = new VanillaCall(OptionType, underlyingsShares[0], Maturity, Strike);
            }
            else if (OptionType.Equals("BasketOption"))
            {
                option = new BasketOption(OptionType, underlyingsShares.ToArray(), Weights,  Maturity, Strike);
            }
            else
            {
                throw new ArgumentException("Unkown OptionType " + OptionType);
            }
            return option;
        }

        #endregion Public Methods

    }
}
