using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Data;

namespace Warehouse
{
    public partial class ProductsPage : UserControl
    {
        private User _user;
        private AppDbContext _context;

        public ProductsPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
        }

        public ProductsPage(User user) : this()
        {
            _user = user;
            LoadData();
        }

        private void LoadData()
        {
            var categories = _context.Categories.ToList();
            CmbCategories.ItemsSource = categories;
            CmbCategories.SelectedItem = null;

            LoadProducts();
        }

        private void LoadProducts()
        {
            var products = _context.Products.ToList();
            var categories = _context.Categories.ToList();

            foreach (var product in products)
            {
                product.Category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }

            if (CmbCategories.SelectedItem is Category selectedCategory)
            {
                products = products.Where(p => p.CategoryId == selectedCategory.Id).ToList();
            }

            var activeProducts = products.Where(p => p.IsActive == true).ToList();
            ProductsGrid.ItemsSource = activeProducts;
        }

        private async void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProductEditDialog();
            await dialog.ShowDialog(GetWindow());
            LoadData(); // Всегда обновляем данные
        }

        private async void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is Product selectedProduct)
            {
                var dialog = new ProductEditDialog(selectedProduct);
                await dialog.ShowDialog(GetWindow());
                LoadData(); 
            }
            else
            {
                await ShowMessage("Выберите товар для редактирования", "Ошибка");
            }
        }

        private async void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is Product selectedProduct)
            {
                if (await ConfirmDelete($"Вы уверены, что хотите удалить товар '{selectedProduct.Name}'?"))
                {
                    var productToDelete = _context.Products.Find(selectedProduct.Id);
                    if (productToDelete != null)
                    {
                        _context.Products.Remove(productToDelete);
                        _context.SaveChanges();
                    }
                    LoadData();
                }
            }
            else
            {
                await ShowMessage("Выберите товар для удаления", "Ошибка");
            }
        }

        private void CmbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadProducts();
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CmbCategories.SelectedItem = null;
            LoadProducts();
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