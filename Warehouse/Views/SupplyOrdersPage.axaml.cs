using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Warehouse.Data;
using Microsoft.EntityFrameworkCore;

namespace Warehouse
{
    public partial class SupplyOrdersPage : UserControl
    {
        private User _user;

        public SupplyOrdersPage()
        {
            InitializeComponent();
        }

        public SupplyOrdersPage(User user) : this()
        {
            _user = user;
            LoadOrders();
        }

        private void LoadOrders()
        {
            var context = new AppDbContext();
            var orders = context.SupplyOrders
                .Include(o => o.Supplier)
                .ToList();
            OrdersGrid.ItemsSource = orders;
            context.Dispose();
        }

        private void BtnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            ShowError("Функция создания заказа поставки в разработке");
        }

        private void BtnViewOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is SupplyOrder selectedOrder)
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
            if (OrdersGrid.SelectedItem is SupplyOrder selectedOrder)
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
            var query = context.SupplyOrders
                .Include(o => o.Supplier)
                .AsQueryable();

            if (selectedStatus == "Ожидание")
            {
                query = query.Where(o => o.Status == "pending");
            }
            else if (selectedStatus == "Подтвержден")
            {
                query = query.Where(o => o.Status == "confirmed");
            }
            else if (selectedStatus == "Доставлен")
            {
                query = query.Where(o => o.Status == "delivered");
            }
            else if (selectedStatus == "Отменен")
            {
                query = query.Where(o => o.Status == "cancelled");
            }

            OrdersGrid.ItemsSource = query.ToList();
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