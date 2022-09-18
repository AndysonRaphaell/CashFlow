using System.Windows;
using System.Windows.Controls;

namespace CashFlowCore.Controls
{
    /// <summary>
    /// Interaction logic for CashFlowControl.xaml
    /// </summary>
    public partial class CashFlowControl : UserControl
    {
        private bool _isCashChanged = false;
        private CashFlow _dataContext;

        public CashFlowControl()
        {
            InitializeComponent();
            
            _dataContext = (CashFlow)this.DataContext;
            txtCash.Text = "0,00";
        }

        private void txtCash_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isCashChanged)
            {
                _isCashChanged = true;
                if (sender is TextBox textCash)
                {
                    double newCash = _dataContext.VerifyCashValue(textCash.Text);
                    txtCash.Text = newCash.ToString("N2").Replace('.', ':').Replace(',', '.').Replace(':', ',');
                    txtCash.Select(txtCash.Text.Length, 0);
                }

                _isCashChanged = false;
            }
        }

        private void btnCredit_Click(object sender, RoutedEventArgs e)
        {
            _dataContext?.ChangeCurrentBalance(CashFlowDataBaseLibrary.Utils.TypesEnum.CashReleaseType.Credit);
        }

        private void btnDebit_Click(object sender, RoutedEventArgs e)
        {
            _dataContext?.ChangeCurrentBalance(CashFlowDataBaseLibrary.Utils.TypesEnum.CashReleaseType.Debt);
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext?.ChangeSelectedDate();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            _dataContext?.GenerateReportByDate();
        }
    }
}
