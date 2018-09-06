using PricingLibrary.FinancialProducts;
using ProjetNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.ViewModels
{
    internal class UserInputViewModel
    {
        #region Private Fields

        private String _optionType;
        private DateTime _maturity;
        private double _strike;
        private String[] _undelyingsIds;
        private Double[] _weights;
        private DateTime _startDateTest;
        private AbstractDataProviderViewModel _dataType;
        private int _estimationStep;

        #endregion Private Fields

        #region Public Constructors

        public UserInputViewModel() { }

        public UserInputViewModel(string optionType, DateTime maturity, double strike, string[] undelyingsIds, double[] weights, DateTime startDateTest, AbstractDataProviderViewModel dataType, int estimationStep)
        {
            _optionType = optionType;
            _maturity = maturity;
            _strike = strike;
            _undelyingsIds = undelyingsIds;
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

        public String[] UnderlyingsIds
        {
            get { return _undelyingsIds; }
            set { _undelyingsIds = value; }
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

        public AbstractDataProviderViewModel DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public int EstimationStep
        {
            get { return _estimationStep; }
            set
            {
                _estimationStep = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public IOption constructOption()
        {
            IOption option = null;

            if (!(Weights == null) && Weights.Sum() != 1)
            {
                throw new ArgumentException("The sum of the weights must equal");
            }
            List<Share> underlyingsShares = new List<Share>();
            foreach (string underlyingId in UnderlyingsIds)
            {
                if (DataType.Name == "Historical")
                {
                    String underlyingName = ShareName.GetShareName(underlyingId);
                    underlyingsShares.Add(new Share(underlyingName, underlyingId));
                }                    
                else
                {
                    underlyingsShares.Add(new Share(underlyingId, underlyingId));
                }
            }
            if (OptionType.Equals("VanillaCall"))
            {
                option = new VanillaCall(OptionType, underlyingsShares[0], Maturity, Strike);
            }
            else if (OptionType.Equals("BasketOption"))
            {
                option = new BasketOption(OptionType, underlyingsShares.ToArray(), Weights, Maturity, Strike);
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
