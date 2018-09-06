using PricingLibrary.FinancialProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.Models
{
    internal class HedgingTool
    {
        #region Private Fields

        private IOption _option;
        private DateTime _startDateTest;
        private IDataProvider _dataType;
        private int _estimationStep;

        #endregion Private Fields

        #region Public Constructors

        public HedgingTool(IOption option, DateTime startDateTest, IDataProvider dataType, int estimationStep)
        {
            _option = option;
            _startDateTest = startDateTest;
            _dataType = dataType;
            _estimationStep = estimationStep;
        }

        #endregion Public Constructors

        #region Public Properties

        public IOption Option
        {
            get { return _option; }
            set { _option = value; }
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
            set { _estimationStep = value; }
        }

        #endregion Public Properties
    }
}
