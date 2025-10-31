using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Warehouse.Data;
using Microsoft.EntityFrameworkCore;

namespace Warehouse
{
    public partial class CustomerOrderViewDialog : Window
    {
        private CustomerOrder _order;

        public CustomerOrderViewDialog()
        {
            InitializeComponent();
        }

        public CustomerOrderViewDialog(CustomerOrder order) : this()
        {
            _order = order;
            LoadOrderData();
        }

        private void LoadOrderData()
        {
            using var context = new AppDbContext();

            // Загружаем полные данные заказа
            var order = context.CustomerOrders
                .Include(o => o.Customer)
                .Include(o => o.CustomerOrderItems)  // Исправлено на CustomerOrderItems
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == _order.Id);

            if (order != null)
            {
                // Устанавливаем контекст данных для привязки
                DataContext = order;

                // Загружаем товары заказа
                OrderItemsGrid.ItemsSource = order.CustomerOrderItems?.ToList();  // Исправлено на CustomerOrderItems

                // Устанавливаем итоговую сумму
                TotalAmountText.Text = order.FinalAmount?.ToString("F2") + " rub.";  // Исправлено на FinalAmount
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}