using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Warehouse.Data;
using Microsoft.EntityFrameworkCore;

namespace Warehouse
{
    public partial class TransfersPage : UserControl
    {
        private User _user;

        public TransfersPage()
        {
            InitializeComponent();
        }

        public TransfersPage(User user) : this()
        {
            _user = user;
            LoadTransfers();
        }

        private void LoadTransfers()
        {
            using var context = new AppDbContext();
            var transfers = context.Transfers
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.TransferItems)
                .ToList();
            TransfersGrid.ItemsSource = transfers;
        }

        private async void BtnCreateTransfer_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateTransferDialog(_user);
            var result = await dialog.ShowDialog<bool>(GetWindow());
            if (result)
            {
                LoadTransfers();
                ShowError("Peremeshchenie sozdano");
            }
        }

        private async void BtnExecuteTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (TransfersGrid.SelectedItem is Transfer selectedTransfer)
            {
                var dialog = new ExecuteTransferDialog(selectedTransfer);
                var result = await dialog.ShowDialog<bool>(GetWindow());
                if (result)
                {
                    LoadTransfers();
                    ShowError("Peremeshchenie vypolneno");
                }
            }
            else
            {
                ShowError("Vyberite peremeshchenie dlya vypolneniya");
            }
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterTransfers();
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CmbStatus.SelectedIndex = 0;
            LoadTransfers();
        }

        private void FilterTransfers()
        {
            var selectedStatus = (CmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            using var context = new AppDbContext();
            var query = context.Transfers
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.TransferItems)
                .AsQueryable();

            if (selectedStatus == "Chernovik")
            {
                query = query.Where(t => t.Status == "draft");
            }
            else if (selectedStatus == "V processe")
            {
                query = query.Where(t => t.Status == "in_progress");
            }
            else if (selectedStatus == "Zaversheno")
            {
                query = query.Where(t => t.Status == "completed");
            }

            TransfersGrid.ItemsSource = query.ToList();
        }

        private Window GetWindow()
        {
            return (Window)VisualRoot;
        }

        private void ShowError(string message)
        {
            var dialog = new Window
            {
                Title = "Informaciya",
                Content = new TextBlock { Text = message, Margin = new Thickness(20) },
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dialog.ShowDialog(GetWindow());
        }
    }
}