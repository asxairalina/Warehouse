using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Warehouse.Data;
using Microsoft.EntityFrameworkCore;

namespace Warehouse
{
    public partial class ExecuteTransferDialog : Window
    {
        private Transfer _transfer;

        public ExecuteTransferDialog()
        {
            InitializeComponent();
        }

        public ExecuteTransferDialog(Transfer transfer) : this()
        {
            _transfer = transfer;
            LoadTransferData();
        }

        private void LoadTransferData()
        {
            TxtTransferNumber.Text = _transfer.TransferNumber;
            TxtStatus.Text = _transfer.Status;
            TxtFromLocation.Text = _transfer.FromLocation;
            TxtToLocation.Text = _transfer.ToLocation;
            TxtTransferDate.Text = _transfer.TransferDate.ToString("dd.MM.yyyy");

            using var context = new AppDbContext();
            var transferItems = context.TransferItems
                .Include(ti => ti.Product)
                .Where(ti => ti.TransferId == _transfer.Id)
                .ToList();

            // ƒобавл€ем информацию о доступном количестве
            var itemsWithAvailability = transferItems.Select(item => new
            {
                item.Product,
                item.Quantity,
                AvailableQuantity = GetAvailableQuantity(item.ProductId, _transfer.FromLocation),
                IsConfirmed = false
            }).ToList();

            TransferItemsGrid.ItemsSource = itemsWithAvailability;
        }

        private int GetAvailableQuantity(int productId, string location)
        {
            // «десь должна быть логика получени€ доступного количества товара на указанном складе
            // ѕока возвращаем заглушку
            return 100;
        }

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            using var context = new AppDbContext();
            var transfer = context.Transfers.Find(_transfer.Id);
            if (transfer != null)
            {
                transfer.Status = "completed";
                context.SaveChanges();
            }

            Close(true);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}