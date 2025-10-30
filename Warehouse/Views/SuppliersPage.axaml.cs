using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Data;

namespace Warehouse
{
    public partial class SuppliersPage : UserControl
    {
        private User _user;

        public SuppliersPage()
        {
            InitializeComponent();
        }

        public SuppliersPage(User user) : this()
        {
            _user = user;
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            var context = new AppDbContext();
            var suppliers = context.Suppliers.ToList();
            SuppliersGrid.ItemsSource = suppliers;
            context.Dispose();
        }

        private async void BtnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SupplierEditDialog();
            await dialog.ShowDialog(GetWindow());
            LoadSuppliers(); // Обновляем список поставщиков
        }

        private async void BtnEditSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (SuppliersGrid.SelectedItem is Supplier selectedSupplier)
            {
                var dialog = new SupplierEditDialog(selectedSupplier);
                await dialog.ShowDialog(GetWindow());
                LoadSuppliers(); // Обновляем список поставщиков
            }
            else
            {
                await ShowMessage("Выберите поставщика для редактирования", "Ошибка");
            }
        }

        private async void BtnDeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (SuppliersGrid.SelectedItem is Supplier selectedSupplier)
            {
                if (await ConfirmDelete($"Вы уверены, что хотите удалить поставщика '{selectedSupplier.Name}'?"))
                {
                    var context = new AppDbContext();
                    var supplierToDelete = context.Suppliers.Find(selectedSupplier.Id);
                    if (supplierToDelete != null)
                    {
                        context.Suppliers.Remove(supplierToDelete);
                        context.SaveChanges();
                    }
                    context.Dispose();
                    LoadSuppliers();
                }
            }
            else
            {
                await ShowMessage("Выберите поставщика для удаления", "Ошибка");
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = TxtSearch.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadSuppliers();
                return;
            }

            var context = new AppDbContext();
            var filteredSuppliers = context.Suppliers
                .Where(s => s.Name.ToLower().Contains(searchText) ||
                           (s.ContactPerson != null && s.ContactPerson.ToLower().Contains(searchText)) ||
                           (s.Phone != null && s.Phone.Contains(searchText)) ||
                           (s.Email != null && s.Email.ToLower().Contains(searchText)))
                .ToList();
            SuppliersGrid.ItemsSource = filteredSuppliers;
            context.Dispose();
        }

        private Window GetWindow()
        {
            return (Window)VisualRoot;
        }

        private async Task ShowMessage(string message, string title = "Информация")
        {
            var dialog = new Window
            {
                Title = title,
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var panel = new StackPanel
            {
                Margin = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var textBlock = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            };

            var button = new Button
            {
                Content = "OK",
                Width = 80,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            button.Click += (s, e) => dialog.Close();

            panel.Children.Add(textBlock);
            panel.Children.Add(button);
            dialog.Content = panel;

            await dialog.ShowDialog(GetWindow());
        }

        private async Task<bool> ConfirmDelete(string message)
        {
            var dialog = new Window
            {
                Title = "Подтверждение",
                Width = 350,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var result = false;

            var panel = new StackPanel
            {
                Margin = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var textBlock = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Spacing = 10,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var yesButton = new Button { Content = "Да", Width = 80 };
            var noButton = new Button { Content = "Нет", Width = 80 };

            yesButton.Click += (s, e) => { result = true; dialog.Close(); };
            noButton.Click += (s, e) => { result = false; dialog.Close(); };

            buttonPanel.Children.Add(yesButton);
            buttonPanel.Children.Add(noButton);

            panel.Children.Add(textBlock);
            panel.Children.Add(buttonPanel);
            dialog.Content = panel;

            await dialog.ShowDialog(GetWindow());
            return result;
        }
    }
}