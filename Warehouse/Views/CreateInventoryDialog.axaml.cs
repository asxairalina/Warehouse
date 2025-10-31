using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Linq;
using Warehouse.Data;

namespace Warehouse
{
    public partial class CreateInventoryDialog : Window
    {
        private User _user;

        public CreateInventoryDialog()
        {
            InitializeComponent();
        }

        public CreateInventoryDialog(User user) : this()
        {
            _user = user;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLocation.Text))
            {
                ShowError("Vvedite mesto inventarizacii");
                return;
            }

            using var context = new AppDbContext();
            var inventory = new Inventory
            {
                InventoryNumber = GenerateInventoryNumber(),
                InventoryDate = DateOnly.FromDateTime(DateTime.Now),
                Location = TxtLocation.Text,
                Status = "in_progress",
                CreatedBy = _user.Id,
                Notes = TxtNotes.Text,
                CreatedAt = DateTime.Now
            };

            context.Inventories.Add(inventory);
            context.SaveChanges();

            // Создаем записи расхождений для всех товаров с реальными остатками
            var products = context.Products.ToList();

            foreach (var product in products)
            {
                int expectedQuantity = GetExpectedQuantity(product.Id, TxtLocation.Text);

                var discrepancy = new InventoryDiscrepancy
                {
                    InventoryId = inventory.Id,
                    ProductId = product.Id,
                    ExpectedQuantity = expectedQuantity, 
                    ActualQuantity = 0,                 
                    Difference = -expectedQuantity,     
                    Notes = ""
                };
                context.InventoryDiscrepancies.Add(discrepancy);

            }

            context.SaveChanges();
            Close(true);
        }

        private int GetExpectedQuantity(int productId, string location)
        {
            using var context = new AppDbContext();

            // Суммируем все остатки данного товара на указанном складе
            var totalQuantity = context.Stocks
                .Where(s => s.ProductId == productId && s.Location == location)
                .Sum(s => (int?)s.Quantity) ?? 0;

            return totalQuantity;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close(false);
        }

        private string GenerateInventoryNumber()
        {
            return $"INV-{DateTime.Now:yyyyMMdd-HHmmss}";
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