using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Warehouse.Data;

namespace Warehouse
{
    public partial class CustomerEditDialog : Window
    {
        private Customer _customer;
        private bool _isNew;

        public CustomerEditDialog()
        {
            InitializeComponent();
            _isNew = true;
            _customer = new Customer();
            Title = "Dobavlenie klienta";
        }

        public CustomerEditDialog(Customer customer) : this()
        {
            _isNew = false;
            _customer = customer;
            LoadCustomerData();
            Title = "Redaktirovanie klienta";
        }

        private void LoadCustomerData()
        {
            TxtName.Text = _customer.Name;
            TxtContactPerson.Text = _customer.ContactPerson;
            TxtPhone.Text = _customer.Phone;
            TxtEmail.Text = _customer.Email;
            TxtAddress.Text = _customer.Address;
            TxtInn.Text = _customer.Inn;
            TxtDiscountRate.Text = _customer.DiscountRate?.ToString("F2") ?? "0";
            ChkIsActive.IsChecked = _customer.IsActive;

            // Устанавливаем тип клиента
            if (_customer.CustomerType == "wholesale")
                CmbCustomerType.SelectedIndex = 1;
            else
                CmbCustomerType.SelectedIndex = 0;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                ShowError("Vvedite naimenovanie klienta");
                return;
            }

            var context = new AppDbContext();

            _customer.Name = TxtName.Text.Trim();
            _customer.ContactPerson = TxtContactPerson.Text?.Trim();
            _customer.Phone = TxtPhone.Text?.Trim();
            _customer.Email = TxtEmail.Text?.Trim();
            _customer.Address = TxtAddress.Text?.Trim();
            _customer.Inn = TxtInn.Text?.Trim();
            _customer.IsActive = ChkIsActive.IsChecked ?? true;

            if (decimal.TryParse(TxtDiscountRate.Text, out decimal discountRate))
                _customer.DiscountRate = discountRate;

            // Устанавливаем тип клиента
            if (CmbCustomerType.SelectedIndex == 1)
                _customer.CustomerType = "wholesale";
            else
                _customer.CustomerType = "retail";

            if (_isNew)
            {
                context.Customers.Add(_customer);
            }
            else
            {
                context.Customers.Update(_customer);
            }

            context.SaveChanges();
            context.Dispose();
            Close(true);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close(false);
        }

        private void ShowError(string message)
        {
            var dialog = new Window
            {
                Title = "Oshibka",
                Content = new TextBlock { Text = message, Margin = new Thickness(20) },
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dialog.ShowDialog(this);
        }
    }
}