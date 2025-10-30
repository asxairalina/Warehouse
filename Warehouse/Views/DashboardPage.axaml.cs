using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Warehouse.Data;

namespace Warehouse
{
    public partial class DashboardPage : UserControl
    {
        private User _user;

        public DashboardPage()
        {
            InitializeComponent();
        }

        public DashboardPage(User user) : this()
        {
            _user = user;
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            using var context = new AppDbContext();

            
            // ИЛИ выручка за ВСЕ доставленные заказы (все время) - альтернатива
            var totalRevenue = context.CustomerOrders
                .Where(o => o.Status == "delivered")
                .Sum(o => (decimal?)o.FinalAmount) ?? 0;
            // TxtTodayRevenue.Text = $"{totalRevenue:F2} руб (всего)";

            // Активные заказы
            var activeOrders = context.CustomerOrders
                .Count(o => o.Status == "confirmed" || o.Status == "reserved" || o.Status == "new");
            TxtActiveOrders.Text = activeOrders.ToString();

            // Товары с низким запасом - УПРОЩЕННАЯ ВЕРСИЯ
            var lowStockProducts = context.Products
                .Count(p => p.MinStockLevel.HasValue && p.MinStockLevel > 10); // Простая проверка
            TxtLowStock.Text = lowStockProducts.ToString();

            // Ожидающие поставки
            var pendingSupplies = context.SupplyOrders
                .Count(o => o.Status == "pending" || o.Status == "confirmed");
            TxtPendingSupplies.Text = pendingSupplies.ToString();

            // Остальной код без изменений...
            var topProducts = context.CustomerOrderItems
                .Include(oi => oi.Product)
                .GroupBy(oi => oi.Product.Name)
                .Select(g => new { Name = g.Key, Sold = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(x => x.Sold)
                .Take(5)
                .Select(x => $"{x.Name} × {x.Sold}")
                .ToList();
            TopProductsList.ItemsSource = topProducts;

            // Запасы по категориям
            var stockByCategory = context.Products
                .Include(p => p.Category)
                .GroupBy(p => p.Category.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => $"{x.Category}: {x.Count} товаров")
                .ToList();
            StockByCategoryList.ItemsSource = stockByCategory;

            // Уведомления (упрощенные)
            var alerts = new List<AlertItem>();

            // Простые уведомления
            if (activeOrders > 0)
            {
                alerts.Add(new AlertItem
                {
                    Message = $"Активных заказов: {activeOrders}",
                    BackgroundColor = "#e8f5e8"
                });
            }

            if (lowStockProducts > 0)
            {
                alerts.Add(new AlertItem
                {
                    Message = $"Товаров с низким запасом: {lowStockProducts}",
                    BackgroundColor = "#fff8e1"
                });
            }

            if (pendingSupplies > 0)
            {
                alerts.Add(new AlertItem
                {
                    Message = $"Ожидающих поставок: {pendingSupplies}",
                    BackgroundColor = "#fff2f2"
                });
            }

            if (alerts.Count == 0)
            {
                alerts.Add(new AlertItem
                {
                    Message = "Все системы работают нормально",
                    BackgroundColor = "#e8f5e8"
                });
            }

            AlertsList.ItemsSource = alerts;
        }
        // Вспомогательный метод для получения количества товара на складе
        private int GetProductStockQuantity(int productId, AppDbContext context)
        {
            return context.Stocks
                .Where(s => s.ProductId == productId)
                .Sum(s => (int?)s.Quantity) ?? 0;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
        }

        private Window GetWindow()
        {
            return (Window)VisualRoot;
        }
    }

    public class AlertItem
    {
        public string Message { get; set; }
        public string BackgroundColor { get; set; } = "#ffffff";
    }
}