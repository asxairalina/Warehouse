using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Warehouse.Data;
using Microsoft.EntityFrameworkCore;

namespace Warehouse
{
    public partial class CustomersPage : UserControl
    {
        private User _user;

        public CustomersPage()
        {
            InitializeComponent();
        }

        public CustomersPage(User user) : this()
        {
            _user = user;
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            var context = new AppDbContext();
            var customers = context.Customers.ToList();
            CustomersGrid.ItemsSource = customers;
            context.Dispose();
        }

        

        private void BtnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersGrid.SelectedItem is Customer selectedCustomer)
            {
                if (ConfirmDelete($"Вы уверены, что хотите удалить клиента '{selectedCustomer.Name}'?"))
                {
                    var context = new AppDbContext();
                    context.Customers.Remove(selectedCustomer);
                    context.SaveChanges();
                    context.Dispose();
                    LoadCustomers();
                }
            }
            else
            {
                ShowError("Выберите клиента для удаления");
            }
        }

        private async void BtnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomerEditDialog();
            await dialog.ShowDialog(GetWindow());
            LoadCustomers();
        }

        private async void BtnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersGrid.SelectedItem is Customer selectedCustomer)
            {
                var dialog = new CustomerEditDialog(selectedCustomer);
                await dialog.ShowDialog(GetWindow());
                LoadCustomers();
            }
            else
            {
                ShowError("Выберите клиента для редактирования"); // Используйте существующий метод
            }
        }

        private void CmbCustomerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterCustomers();
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CmbCustomerType.SelectedIndex = 0;
            FilterCustomers();
        }

        private void FilterCustomers()
        {
            var selectedType = (CmbCustomerType.SelectedItem as ComboBoxItem)?.Content.ToString();

            using var context = new AppDbContext();
            var query = context.Customers.AsQueryable();

            if (selectedType == "Retail")
            {
                query = query.Where(c => c.CustomerType == "retail");
            }
            else if (selectedType == "Wholesale")
            {
                query = query.Where(c => c.CustomerType == "wholesale");
            }

            CustomersGrid.ItemsSource = query.ToList();
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

        private bool ConfirmDelete(string message)
        {
            var dialog = new Window
            {
                Title = "Подтверждение",
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { Text = message, Margin = new Thickness(20) },
                        new StackPanel
                        {
                            Orientation = Avalonia.Layout.Orientation.Horizontal,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Margin = new Thickness(10),
                            Children =
                            {
                                new Button { Content = "Да", Tag = true, Margin = new Thickness(5) },
                                new Button { Content = "Нет", Tag = false, Margin = new Thickness(5) }
                            }
                        }
                    }
                },
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            bool result = false;
            foreach (Button btn in ((StackPanel)((StackPanel)dialog.Content).Children[1]).Children)
            {
                btn.Click += (s, e) =>
                {
                    result = (bool)((Button)s).Tag;
                    dialog.Close();
                };
            }

            dialog.ShowDialog(GetWindow());
            return result;
        }
    }
}