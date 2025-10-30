using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Xml.Linq;
using Warehouse.Data;

namespace Warehouse
{
    public partial class SupplierEditDialog : Window
    {
        private Supplier _supplier;
        private bool _isNew;

        public SupplierEditDialog()
        {
            InitializeComponent();
            _isNew = true;
            _supplier = new Supplier();
            Title = "Добавление поставщика";
        }

        public SupplierEditDialog(Supplier supplier) : this()
        {
            _isNew = false;
            _supplier = supplier;
            LoadSupplierData();
            Title = "Редактирование поставщика";
        }

        private void LoadSupplierData()
        {
            TxtName.Text = _supplier.Name;
            TxtContactPerson.Text = _supplier.ContactPerson;
            TxtPhone.Text = _supplier.Phone;
            TxtEmail.Text = _supplier.Email;
            TxtAddress.Text = _supplier.Address;
            TxtInn.Text = _supplier.Inn;
            TxtBankDetails.Text = _supplier.BankDetails;
            ChkIsActive.IsChecked = _supplier.IsActive;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                ShowError("Введите наименование поставщика");
                return;
            }

            var context = new AppDbContext();

            _supplier.Name = TxtName.Text.Trim();
            _supplier.ContactPerson = TxtContactPerson.Text?.Trim();
            _supplier.Phone = TxtPhone.Text?.Trim();
            _supplier.Email = TxtEmail.Text?.Trim();
            _supplier.Address = TxtAddress.Text?.Trim();
            _supplier.Inn = TxtInn.Text?.Trim();
            _supplier.BankDetails = TxtBankDetails.Text?.Trim();
            _supplier.IsActive = ChkIsActive.IsChecked ?? true;

            if (_isNew)
            {
                context.Suppliers.Add(_supplier);
            }
            else
            {
                context.Suppliers.Update(_supplier);
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
                Title = "Ошибка",
                Content = new TextBlock { Text = message, Margin = new Thickness(20) },
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dialog.ShowDialog(this);
        }
    }
}