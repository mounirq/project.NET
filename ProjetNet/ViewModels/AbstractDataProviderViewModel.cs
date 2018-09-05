using ProjetNet.Models;

namespace ProjetNet.ViewModels
{
    internal abstract class AbstractDataProviderViewModel
    {
        #region Private Fields

        private IDataProvider _data;

        #endregion Private Fields

        #region Public Constructors

        public AbstractDataProviderViewModel(IDataProvider data)
        {
            _data = data;
        }

        #endregion Public Constructors

        #region Public Properties

        public IDataProvider Data
        {
            get { return _data; }
        }

        public abstract string Name { get; }

        #endregion Public Properties

    }
}
