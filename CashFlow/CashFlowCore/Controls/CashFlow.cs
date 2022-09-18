using CashFlowDataBaseLibrary.BLL;
using CashFlowDataBaseLibrary.DML;
using CashFlowDataBaseLibrary.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CashFlowCore.Controls
{
    public class CashFlow : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region [Variables]

        private double _currentCash;
        private readonly BoCashFlow _boCashFlowDb;

        #endregion

        #region [Properties]

        private double _previousCashReleased;
        public double PreviousCashReleased
        {
            get { return _previousCashReleased; }
            set
            {
                _previousCashReleased = value;
                OnPropertyChanged();
            }
        }

        private double _currentBalance;
        public double CurrentBalance
        {
            get { return _currentBalance; }
            set
            {
                _currentBalance = value;
                OnPropertyChanged();
            }
        }

        private DateTime _selectedInitialDate;
        public DateTime SelectedInitialDate
        {
            get { return _selectedInitialDate; }
            set
            {
                _selectedInitialDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _selectedFinalDate;
        public DateTime SelectedFinalDate
        {
            get { return _selectedFinalDate; }
            set
            {
                _selectedFinalDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _initialDate;
        public DateTime InitialDate
        {
            get { return _initialDate; }
            set
            {
                _initialDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _rangeDate;
        public DateTime RangeDate
        {
            get { return _rangeDate; }
            set
            {
                _rangeDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _finalDate;
        public DateTime FinalDate
        {
            get { return _finalDate; }
            set
            {
                _finalDate = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public CashFlow()
        {
            _boCashFlowDb = new BoCashFlow();

            StartParamethers();
        }

        #region [Public Methods]

        public void ChangeSelectedDate()
        {
            RangeDate = SelectedInitialDate;
        }

        public async void ChangeCurrentBalance(TypesEnum.CashReleaseType cashReleaseType)
        {
            try
            {
                if (_currentCash > double.MinValue)
                {
                    PreviousCashReleased = _currentCash;
                    if (cashReleaseType == TypesEnum.CashReleaseType.Credit)
                    {
                        CurrentBalance += _currentCash;
                    }
                    else
                    {
                        PreviousCashReleased *= -1;
                        CurrentBalance -= _currentCash;
                    }

                    await _boCashFlowDb.InsertCashReleased(_currentCash, cashReleaseType, PreviousCashReleased, CurrentBalance);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public double VerifyCashValue(string textCash)
        {
            if (double.TryParse(textCash.Replace('.', ':').Replace(',', '.').Replace(':', ','), out double newCash) && newCash < 99999999999)
            {
                string[] decimals = textCash.Replace('.', ':').Replace(',', '.').Replace(':', ',').Split('.');

                if (decimals.Length > 1 && decimals[1].Length != 2)
                    _currentCash = decimals[1].Length == 3 ? newCash * 10 : newCash / 10;
                else
                    _currentCash = newCash;
            }

            return _currentCash;
        }

        public void GenerateReportByDate()
        {
            try
            {
                string savedFolder = _boCashFlowDb.GetBalanceByDate(SelectedInitialDate, SelectedFinalDate);
                _ = MessageBox.Show($"The report was saved on \"{savedFolder}\"", "Message", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}");
            }
        }

        #endregion

        #region [Private Methods]

        private void StartParamethers()
        {
            try
            {
                CashFlowDataSet lastBalance = _boCashFlowDb.LoadBalanceInformations();
                lastBalance ??= new CashFlowDataSet();

                PreviousCashReleased = lastBalance.PreviousValue;
                CurrentBalance = lastBalance.BalanceValue;
                SelectedInitialDate = DateTime.Now;
                SelectedFinalDate = DateTime.Now;
                InitialDate = DateTime.Now.AddDays(-90d);
                RangeDate = SelectedInitialDate;
                FinalDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}");
            }
        }

        #endregion
    }
}
