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

        private async void BtnViewOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is CustomerOrder selectedOrder)
            {
                var dialog = new CustomerOrderViewDialog(selectedOrder);
                await dialog.ShowDialog(GetWindow());
            }
            else
            {
                ShowError("Vyberite zakaz dlya prosmotra");
            }
        }

        private async void BtnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is CustomerOrder selectedOrder)
            {
                var dialog = new ChangeOrderStatusDialog(selectedOrder);
                var result = await dialog.ShowDialog<bool>(GetWindow());

                if (result)
                {
                    LoadOrders(); // Obnovit spisok posle izmeneniya statusa
                    ShowError("Status zakaza uspeshno izmenen");
                }
            }
            else
            {
                ShowError("Vyberite zakaz dlya izmeneniya statusa");
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
                .Include(o => o.CustomerOrderItems)
                .AsQueryable();

            if (selectedStatus == "New")
            {
                query = query.Where(o => o.Status == "new");
            }
            else if (selectedStatus == "Confirmed")
            {
                query = query.Where(o => o.Status == "confirmed");
            }
            else if (selectedStatus == "Reserved")
            {
                query = query.Where(o => o.Status == "reserved");
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