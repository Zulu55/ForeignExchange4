namespace ForeignExchange4.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Models;

    public class MainViewModel
    {
        #region Propperties
        public string Amount
        {
            get;
            set;
        }

        public ObservableCollection<Rate> Rates
        {
            get;
            set;
        }

        public Rate SourceRate
        {
            get;
            set;
        }

        public Rate TargetRate
        {
            get;
            set;
        }

        public bool IsRunning
        {
            get;
            set;
        }

        public string Result
        {
            get;
            set;
        }
        #endregion    

        #region Constructors
        public MainViewModel()
        {
            Result = "Loading rates...";
            IsRunning = true;
        }
        #endregion


        #region Commands
        public ICommand ConvertCommand
        {
            get
            {
                return new RelayCommand(Convert);
            }
        }

        void Convert()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
