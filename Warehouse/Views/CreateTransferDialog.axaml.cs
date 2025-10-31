using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Warehouse.Data;
using System;

namespace Warehouse
{
    public partial class CreateTransferDialog : Window
    {
        private User _user;

        public CreateTransferDialog()
        {
            InitializeComponent();
            DpTransferDate.SelectedDate = DateTime.Today;
        }

        public CreateTransferDialog(User user) : this()
        {
            _user = user;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (CmbFromLocation.SelectedItem == null || CmbToLocation.SelectedItem == null)
            {
                ShowError("Vyberite lokacii dlya peremeshcheniya");
                return;
            }

            var fromLocation = (CmbFromLocation.SelectedItem as ComboBoxItem)?.Content.ToString();
            var toLocation = (CmbToLocation.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (fromLocation == toLocation)
            {
                ShowError("Lokacii ne mogut byt odinakovymi");
                return;
            }

            using var context = new AppDbContext();
            var transfer = new Transfer
            {
                TransferNumber = GenerateTransferNumber(),
                FromLocation = fromLocation,
                ToLocation = toLocation,
                TransferDate = DateOnly.FromDateTime(DateTime.Today),
                Status = "draft",
                CreatedBy = _user.Id,
                Notes = TxtNotes.Text,
                CreatedAt = DateTime.Now
            };

            try
            {
                context.Transfers.Add(transfer);
                context.SaveChanges();
                Close(true);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                ShowError($"Oshibka sohraneniya: {ex.InnerException?.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close(false);
        }

        private string GenerateTransferNumber()
        {
            return $"TRF-{DateTime.Now:yyyyMMdd-HHmmss}";
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