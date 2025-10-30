using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Warehouse.Data;
using System.Linq;


namespace Warehouse
{
    public partial class StockPage : UserControl
    {
        private User _user;

        public StockPage()
        {
            InitializeComponent();
        }

        public StockPage(User user) : this()
        {
            _user = user;
            LoadStock();
        }

        private void LoadStock()
        {
            var context = new AppDbContext();

            // Вместо Include используем явную загрузку
            var stock = context.Stocks.ToList();
            var products = context.Products.ToList();

            // Связываем данные вручную
            foreach (var stockItem in stock)
            {
                stockItem.Product = products.FirstOrDefault(p => p.Id == stockItem.ProductId);
            }

            StockGrid.ItemsSource = stock;
            context.Dispose();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadStock();
        }

        private void BtnAddStock_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Функция добавления на склад в разработке");
        }

        private void BtnRemoveStock_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Функция списания со склада в разработке");
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = TxtSearch.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadStock();
                return;
            }

            var context = new AppDbContext();
            var allStock = context.Stocks.ToList();
            var products = context.Products.ToList();

            foreach (var stockItem in allStock)
            {
                stockItem.Product = products.FirstOrDefault(p => p.Id == stockItem.ProductId);
            }

            var filteredStock = allStock.Where(s =>
                (s.Product?.Name?.ToLower().Contains(searchText) == true) ||
                (s.Location?.ToLower().Contains(searchText) == true) ||
                (s.BatchNumber?.ToLower().Contains(searchText) == true)
            ).ToList();

            StockGrid.ItemsSource = filteredStock;
            context.Dispose();
        }

        private Window GetWindow()
        {
            return (Window)VisualRoot;
        }

        private void ShowMessage(string message)
        {
            var dialog = new Window
            {
                Title = "Информация",
                Content = new TextBlock { Text = message, Margin = new Thickness(20) },
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dialog.ShowDialog(GetWindow());
        }
    }
}