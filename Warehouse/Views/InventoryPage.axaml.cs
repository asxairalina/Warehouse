using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Warehouse.Data;
using Microsoft.EntityFrameworkCore;

namespace Warehouse
{
    public partial class InventoryPage : UserControl
    {
        private User _user;

        public InventoryPage()
        {
            InitializeComponent();
        }

        public InventoryPage(User user) : this()
        {
            _user = user;
            LoadInventory();
        }

        private void LoadInventory()
        {
            var context = new AppDbContext();
            var inventories = context.Inventories.ToList();
            InventoryGrid.ItemsSource = inventories;
            context.Dispose();
        }

        private void BtnStartInventory_Click(object sender, RoutedEventArgs e)
        {
            ShowError("Функция начала инвентаризации в разработке");
        }

        private void BtnCountInventory_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryGrid.SelectedItem is Inventory selectedInventory)
            {
                ShowError($"Подсчет для инвентаризации: {selectedInventory.InventoryNumber}");
            }
            else
            {
                ShowError("Выберите инвентаризацию для подсчета");
            }
        }

        private void BtnFinishInventory_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryGrid.SelectedItem is Inventory selectedInventory)
            {
                ShowError($"Завершение инвентаризации: {selectedInventory.InventoryNumber}");
            }
            else
            {
                ShowError("Выберите инвентаризацию для завершения");
            }
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterInventory();
        }

        private void FilterInventory()
        {
            var selectedStatus = (CmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            var context = new AppDbContext();
            var query = context.Inventories.AsQueryable();

            if (selectedStatus == "В процессе")
            {
                query = query.Where(i => i.Status == "in_progress");
            }
            else if (selectedStatus == "Завершена")
            {
                query = query.Where(i => i.Status == "completed");
            }

            InventoryGrid.ItemsSource = query.ToList();
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