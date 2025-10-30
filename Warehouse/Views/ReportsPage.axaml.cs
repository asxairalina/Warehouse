using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Warehouse.Data;
using System;
using Microsoft.EntityFrameworkCore;

namespace Warehouse
{
    public partial class ReportsPage : UserControl
    {
        private User _user;

        public ReportsPage()
        {
            InitializeComponent();
        }

        public ReportsPage(User user) : this()
        {
            _user = user;
        }

        private void BtnSalesReport_Click(object sender, RoutedEventArgs e)
        {
            var context = new AppDbContext();

            var totalSales = context.CustomerOrders
                .Where(o => o.Status == "delivered")
                .Sum(o => o.FinalAmount);

            var ordersCount = context.CustomerOrders
                .Where(o => o.Status == "delivered")
                .Count();

            TxtReportResult.Text = $"Отчет по продажам:\n" +
                                 $"Всего продаж: {ordersCount}\n" +
                                 $"Общая сумма: {totalSales:F2} руб.\n" +
                                 $"Средний чек: {(ordersCount > 0 ? totalSales / ordersCount : 0):F2} руб.";

            context.Dispose();
        }

        private void BtnStockReport_Click(object sender, RoutedEventArgs e)
        {
            var context = new AppDbContext();

            var totalProducts = context.Products.Count();
            var activeProducts = context.Products.Count(p => p.IsActive == true);
            var lowStockProducts = context.Products
                .Where(p => p.MinStockLevel.HasValue &&
                           p.Stocks.Sum(s => s.Quantity) <= p.MinStockLevel.Value)
                .Count();

            TxtReportResult.Text = $"Отчет по остаткам:\n" +
                                 $"Всего товаров: {totalProducts}\n" +
                                 $"Активных товаров: {activeProducts}\n" +
                                 $"Товаров с низким запасом: {lowStockProducts}";

            context.Dispose();
        }

        private void BtnSupplyReport_Click(object sender, RoutedEventArgs e)
        {
            var context = new AppDbContext();

            var pendingOrders = context.SupplyOrders.Count(o => o.Status == "pending");
            var deliveredOrders = context.SupplyOrders.Count(o => o.Status == "delivered");
            var totalSuppliers = context.Suppliers.Count(s => s.IsActive == true);

            TxtReportResult.Text = $"Отчет по поставкам:\n" +
                                 $"Ожидающих поставок: {pendingOrders}\n" +
                                 $"Доставленных поставок: {deliveredOrders}\n" +
                                 $"Активных поставщиков: {totalSuppliers}";

            context.Dispose();
        }

        private void BtnOrdersReport_Click(object sender, RoutedEventArgs e)
        {
            var context = new AppDbContext();

            var newOrders = context.CustomerOrders.Count(o => o.Status == "new");
            var confirmedOrders = context.CustomerOrders.Count(o => o.Status == "confirmed");
            var deliveredOrders = context.CustomerOrders.Count(o => o.Status == "delivered");

            TxtReportResult.Text = $"Отчет по заказам:\n" +
                                 $"Новых заказов: {newOrders}\n" +
                                 $"Подтвержденных заказов: {confirmedOrders}\n" +
                                 $"Доставленных заказов: {deliveredOrders}";

            context.Dispose();
        }

        private void BtnAnalyticsReport_Click(object sender, RoutedEventArgs e)
        {
            var context = new AppDbContext();

            var totalCustomers = context.Customers.Count(c => c.IsActive == true);
            var wholesaleCustomers = context.Customers.Count(c => c.CustomerType == "wholesale");
            var retailCustomers = context.Customers.Count(c => c.CustomerType == "retail");

            TxtReportResult.Text = $"Аналитика продаж:\n" +
                                 $"Всего клиентов: {totalCustomers}\n" +
                                 $"Оптовых клиентов: {wholesaleCustomers}\n" +
                                 $"Розничных клиентов: {retailCustomers}\n" +
                                 $"Соотношение опт/розница: {(totalCustomers > 0 ? (double)wholesaleCustomers / totalCustomers * 100 : 0):F1}% / {(totalCustomers > 0 ? (double)retailCustomers / totalCustomers * 100 : 0):F1}%";

            context.Dispose();
        }
    }
}