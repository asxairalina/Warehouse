using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Warehouse.Data;

namespace Warehouse
{
    public partial class MainMenuPage : Window
    {
        private User _user;

        public MainMenuPage()
        {
            InitializeComponent();
        }

        public MainMenuPage(User user) : this()
        {
            _user = user;

            TxtWelcome.Text = $"Добро пожаловать, {_user.FullName}! ({_user.Role})";

            // Настройка видимости кнопок по ролям
            bool isAdmin = _user.Role == "admin";
            bool isManager = _user.Role == "manager";
            bool isWarehouse = _user.Role == "warehouse_worker";
            bool isSales = _user.Role == "sales";

            BtnProducts.IsVisible = isAdmin;
            BtnSuppliers.IsVisible = isAdmin;
            BtnReports.IsVisible = isAdmin;

            BtnCustomers.IsVisible = isManager;
            BtnCustomerOrders.IsVisible = isManager;

            BtnStock.IsVisible = isWarehouse;
            BtnSupplyOrders.IsVisible = isWarehouse;
            BtnInventory.IsVisible = isWarehouse;
            BtnTransfers.IsVisible = isWarehouse;

            BtnSalesOrders.IsVisible = isSales;
            BtnSalesCustomers.IsVisible = isSales;

            ShowProductsPage();
        }

        private void BtnProducts_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowProductsPage();
        }

        private void BtnStock_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowStockPage();
        }

        private void BtnCustomers_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowCustomersPage();
        }

        private void BtnSuppliers_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowSuppliersPage();
        }

        private void BtnSupplyOrders_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowSupplyOrdersPage();
        }

        private void BtnCustomerOrders_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowCustomerOrdersPage();
        }

        private void BtnInventory_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowInventoryPage();
        }

        private void BtnSalesOrders_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowCustomerOrdersPage();
        }

        private void BtnSalesCustomers_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowCustomersPage();
        }

        private void BtnReports_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowReportsPage();
        }

        private void BtnLogout_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void ShowProductsPage()
        {
            MainContent.Content = new ProductsPage(_user);
        }

        private void ShowStockPage()
        {
            MainContent.Content = new StockPage(_user);
        }

        private void ShowCustomersPage()
        {
            MainContent.Content = new CustomersPage(_user);
        }

        private void ShowSuppliersPage()
        {
            MainContent.Content = new SuppliersPage(_user);
        }

        private void ShowSupplyOrdersPage()
        {
            MainContent.Content = new SupplyOrdersPage(_user);
        }

        private void ShowCustomerOrdersPage()
        {
            MainContent.Content = new CustomerOrdersPage(_user);
        }

        private void ShowInventoryPage()
        {
            MainContent.Content = new InventoryPage(_user);
        }

        private void ShowReportsPage()
        {
            MainContent.Content = new ReportsPage(_user);
        }

        private void BtnTransfers_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TransfersPage(_user);
        }
    }
}