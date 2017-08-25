namespace ForeignExchange2.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;
    using ForeignExchange2.Helpers;
    using GalaSoft.MvvmLight.Command;
    using Models;
    using Xamarin.Forms;

    public class MainViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Services
        ApiService apiService;
        #endregion

        #region Attributes
        bool _isRunning;
        bool _isEnabled;
        string _result;
        ObservableCollection<Rate> _rates;
		Rate _sourceRate;
		Rate _targetRate;
        string _status;
		#endregion

		#region Properties
        public string Status
        {
			get
			{
				return _status;
			}
			set
			{
				if (_status != value)
				{
					_status = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(Status)));
				}
			}
		}

		public string Amount
        {
            get;
            set;
        }

        public ObservableCollection<Rate> Rates
        {
			get
			{
                return _rates;
			}
			set
			{
				if (_rates != value)
				{
					_rates = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(Rates)));
				}
			}
		}

        public Rate SourceRate
        {
			get
			{
				return _sourceRate;
			}
			set
			{
				if (_sourceRate != value)
				{
					_sourceRate = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(SourceRate)));
				}
			}
		}

        public Rate TargetRate
        {
			get
			{
				return _targetRate;
			}
			set
			{
				if (_targetRate != value)
				{
					_targetRate = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(TargetRate)));
				}
			}
		}

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRunning)));
                }
            }
        }

        public bool IsEnabled
        {
			get
			{
				return _isEnabled;
			}
			set
			{
				if (_isEnabled != value)
				{
					_isEnabled = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(IsEnabled)));
				}
			}
		}

        public string Result
        {
			get
			{
				return _result;
			}
			set
			{
				if (_result != value)
				{
					_result = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(Result)));
				}
			}
		}
        #endregion

        #region Constructors
        public MainViewModel()
        {
            apiService = new ApiService();
            LoadRates();
        }
        #endregion

        #region Methods
        async void LoadRates()
        {
            IsRunning = true;
            Result = "Loading rates...";

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
				IsRunning = false;
				Result = connection.Message;
				return;
			}

            var response = await apiService.GetList<Rate>(
                "http://apiexchangerates.azurewebsites.net", 
                "api/Rates");

            if (!response.IsSuccess)
            {
                IsRunning = false;
                Result = response.Message;
                return;
            }

            Rates = new ObservableCollection<Rate>((List<Rate>)response.Result);
            IsRunning = false;
            IsEnabled = true;
			Result = "Ready to convert!";
            Status = "Rates loaded from internet.";
		}
        #endregion

        #region Commands
        public ICommand SwitchCommand
        {
			get
			{
				return new RelayCommand(Switch);
			}
		}

        void Switch()
        {
            var aux = SourceRate;
            SourceRate = TargetRate;
            TargetRate = aux;
            Convert();
        }

        public ICommand ConvertCommmand
        {
            get
            {
                return new RelayCommand(Convert);
            }
        }

        async void Convert()
        {
            if (string.IsNullOrEmpty(Amount))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Lenguages.Error, 
                    Lenguages.AmountValidation, 
                    Lenguages.Accept);
                return;
            }

            decimal amount = 0;
            if (!decimal.TryParse(Amount, out amount))
            {
				await Application.Current.MainPage.DisplayAlert(
					"Error",
					"You must enter a numeric value in amount.",
					"Accept");
				return;
			}

			if (SourceRate == null)
			{
				await Application.Current.MainPage.DisplayAlert(
					"Error",
					"You must select a source rate.",
					"Accept");
				return;
			}

			if (TargetRate == null)
			{
				await Application.Current.MainPage.DisplayAlert(
					"Error",
					"You must select a target rate.",
					"Accept");
				return;
			}

            var amountConverted = amount / 
                                  (decimal)SourceRate.TaxRate * 
                                  (decimal)TargetRate.TaxRate;

            Result = string.Format(
                "{0} {1:C2} = {2} {3:C2}", 
                SourceRate.Code, 
                amount, 
                TargetRate.Code, 
                amountConverted);
		}
        #endregion
    }
}
