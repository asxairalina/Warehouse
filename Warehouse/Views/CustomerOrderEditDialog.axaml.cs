using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using System.Collections.ObjectModel;
using Warehouse.Data;
using System;

namespace Warehouse
{
    public partial class CustomerOrderEditDialog : Window
    {
        private CustomerOrder _order;
        private bool _isNew;
        private ObservableCollection<CustomerOrderItem> _orderItems;
        private AppDbContext _context;

        public CustomerOrderEditDialog()
        {
            InitializeComponent();
            _context = new AppDbContext();
            _isNew = true;
            _order = new CustomerOrder();
            _orderItems = new ObservableCollection<CustomerOrderItem>();
            InitializeControls();
        }

        public CustomerOrderEditDialog(CustomerOrder order) : this()
        {
            _isNew = false;
            _order = order;
            LoadOrderData();
            Title = "Redaktirovanie zakaza";
        }

        private void InitializeControls()
        {
            // Загрузка клиентов
            var customers = _context.Customers.Where(c => c.IsActive == true).ToList();
            CmbCustomer.ItemsSource = customers;

            // Настройка DataGrid
            OrderItemsGrid.ItemsSource = _orderItems;

            Title = _isNew ? "Oformlenie zakaza" : "Redaktirovanie zakaza";
        }

        private void LoadOrderData()
        {
            // Загрузка данных заказа
            if (_order.CustomerId.HasValue)
            {
                var customer = _context.Customers.Find(_order.CustomerId.Value);
                CmbCustomer.SelectedItem = customer;
            }

            TxtNotes.Text = _order.Notes;

            // Загрузка позиций заказа
            var orderItems = _context.CustomerOrderItems
                .Where(oi => oi.CustomerOrderId == _order.Id)
                .ToList();

            var products = _context.Products.ToList();
            foreach (var item in orderItems)
            {
                item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
            }

            _orderItems.Clear();
            foreach (var item in orderItems)
            {
                _orderItems.Add(item);
            }

            CalculateTotal();
        }

        private async void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var products = _context.Products.Where(p => p.IsActive == true).ToList();

            var dialog = new Window
            {
                Title = "Vybor tovara",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20), Spacing = 10 };

            var productCombo = new ComboBox
            {
                ItemsSource = products,
                DisplayMemberBinding = new Avalonia.Data.Binding("Name")
            };

            var quantityBox = new TextBox { Text = "1" };

            var addButton = new Button { Content = "Dobavit" };

            stackPanel.Children.Add(new TextBlock { Text = "Tovar:" });
            stackPanel.Children.Add(productCombo);
            stackPanel.Children.Add(new TextBlock { Text = "Kolichestvo:" });
            stackPanel.Children.Add(quantityBox);
            stackPanel.Children.Add(addButton);

            dialog.Content = stackPanel;

            bool result = false;
            Product selectedProduct = null;
            int quantity = 1;

            addButton.Click += (s, e) =>
            {
                if (productCombo.SelectedItem is Product product && int.TryParse(quantityBox.Text, out int qty) && qty > 0)
                {
                    selectedProduct = product;
                    quantity = qty;
                    result = true;
                    dialog.Close();
                }
            };

            await dialog.ShowDialog(this);

            if (result && selectedProduct != null)
            {
                var orderItem = new CustomerOrderItem
                {
                    ProductId = selectedProduct.Id,
                    Product = selectedProduct,
                    Quantity = quantity,
                    UnitPrice = selectedProduct.SellingPrice
                };

                _orderItems.Add(orderItem);
                CalculateTotal();
            }
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CustomerOrderItem item)
            {
                _orderItems.Remove(item);
                CalculateTotal();
            }
        }

        private void CalculateTotal()
        {
            decimal totalAmount = _orderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
            TxtTotalAmount.Text = $"{totalAmount:F2} rub.";
            _order.TotalAmount = totalAmount;
            _order.FinalAmount = totalAmount;
        }

        private void BtnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            if (CmbCustomer.SelectedItem is not Customer selectedCustomer)
            {
                ShowError("Vyberite klienta");
                return;
            }

            if (_orderItems.Count == 0)
            {
                ShowError("Dobavte tovary v zakaz");
                return;
            }

            try
            {
                _order.CustomerId = selectedCustomer.Id;
                _order.Notes = TxtNotes.Text?.Trim();
                _order.OrderDate = DateOnly.FromDateTime(DateTime.Now);
                _order.Status = "new";

                if (_isNew)
                {
                    _order.OrderNumber = GenerateOrderNumber();
                    _context.CustomerOrders.Add(_order);
                    _context.SaveChanges();

                    foreach (var item in _orderItems)
                    {
                        item.CustomerOrderId = _order.Id;
                        _context.CustomerOrderItems.Add(item);
                    }
                }
                else
                {
                    _context.CustomerOrders.Update(_order);

                    var existingItems = _context.CustomerOrderItems
                        .Where(oi => oi.CustomerOrderId == _order.Id);
                    _context.CustomerOrderItems.RemoveRange(existingItems);

                    foreach (var item in _orderItems)
                    {
                        item.CustomerOrderId = _order.Id;
                        _context.CustomerOrderItems.Add(item);
                    }
                }

                _context.SaveChanges();
                _context.Dispose();
                Close(true);
            }
            catch (Exception ex)
            {
                ShowError($"Oshibka sohraneniya: {ex.Message}");
            }
        }

        private string GenerateOrderNumber()
        {
            var now = DateTime.Now.Date;
            return $"ORD-{now:yyyyMMddHHmmss}";
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
            Close(false);
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