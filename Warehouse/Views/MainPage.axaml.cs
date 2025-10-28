// Views/MainPage.axaml.cs
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using WarehouseApp;

namespace WarehouseApp.Views
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void LoadProducts()
        {
            try
            {
                var products = await Database.LoadProductsAsync();
                ProductsPanel.ItemsSource = products;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}