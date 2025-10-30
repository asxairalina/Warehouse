using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Warehouse.Data;
using Microsoft.EntityFrameworkCore;


namespace Warehouse
{
    public partial class CustomerOrdersPage : UserControl
    {
        private User _user;

        public CustomerOrdersPage()
        {
            InitializeComponent();
        }

        public CustomerOrdersPage(User user) : this()
        {
            _user = user;
            LoadOrders();
        }

        private void LoadOrders()
        {
            var context = new AppDbContext();
            var orders = context.CustomerOrders
                .Include(o => o.Customer)
                .ToList();
            OrdersGrid.ItemsSource = orders;
            context.Dispose();
        }

        private async void BtnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomerOrderEditDialog();
            await dialog.ShowDialog(GetWindow());
            LoadOrders();
        }

        private void BtnViewOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is CustomerOrder selectedOrder)
            {
                ShowError($"Просмотр заказа: {selectedOrder.OrderNumber}");
            }
            else
            {
                ShowError("Выберите заказ для просмотра");
            }
        }

        private void BtnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is CustomerOrder selectedOrder)
            {
                ShowError($"Изменение статуса заказа: {selectedOrder.OrderNumber}");
            }
            else
            {
                ShowError("Выберите заказ для изменения статуса");
            }
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterOrders();
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CmbStatus.SelectedIndex = 0;
            LoadOrders();
        }

        private void FilterOrders()
        {
            var selectedStatus = (CmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            var context = new AppDbContext();
            var query = context.CustomerOrders
                .Include(o => o.Customer)
                .AsQueryable();

            if (selectedStatus == "Новый")
            {
                query = query.Where(o => o.Status == "new");
            }
            else if (selectedStatus == "Подтвержден")
            {
                query = query.Where(o => o.Status == "confirmed");
            }
            else if (selectedStatus == "Зарезервирован")
            {
                query = query.Where(o => o.Status == "reserved");
            }
            else if (selectedStatus == "Отгружен")
            {
                query = query.Where(o => o.Status == "shipped");
            }
            else if (selectedStatus == "Доставлен")
            {
                query = query.Where(o => o.Status == "delivered");
            }
            else if (selectedStatus == "Отменен")
            {
                query = query.Where(o => o.Status == "cancelled");
            }

            OrdersGrid.ItemsSource= query.ToList();
            context.Dispose();
        }

        private Window GetWindow()
        {
            return (Window)VisualRoot;
        }

        private void ShowError(string message)
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