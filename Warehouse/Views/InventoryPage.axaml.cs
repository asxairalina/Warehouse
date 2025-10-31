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
            using var context = new AppDbContext();
            var inventories = context.Inventories
                .Include(i => i.CreatedByNavigation)
                .ToList();
            InventoryGrid.ItemsSource = inventories;
        }

        private async void BtnStartInventory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateInventoryDialog(_user);
            var result = await dialog.ShowDialog<bool>(GetWindow());
            if (result)
            {
                LoadInventory();
                ShowError("Inventarizaciya nachata");
            }
        }

        private async void BtnCountInventory_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryGrid.SelectedItem is Inventory selectedInventory)
            {
                var dialog = new InventoryCountDialog(selectedInventory);
                var result = await dialog.ShowDialog<bool>(GetWindow());
                if (result)
                {
                    LoadInventory();
                    ShowError("Podschet sohranen");
                }
            }
            else
            {
                ShowError("Vyberite inventarizaciyu dlya podsmeta");
            }
        }

        private async void BtnFinishInventory_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryGrid.SelectedItem is Inventory selectedInventory)
            {
                using var context = new AppDbContext();
                var inventory = context.Inventories.Find(selectedInventory.Id);
                if (inventory != null)
                {
                    inventory.Status = "completed";
                    context.SaveChanges();
                    LoadInventory();
                    ShowError("Inventarizaciya zavershena");
                }
            }
            else
            {
                ShowError("Vyberite inventarizaciyu dlya zaversheniya");
            }
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterInventory();
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CmbStatus.SelectedIndex = 0;
            LoadInventory();
        }

        private void FilterInventory()
        {
            var selectedStatus = (CmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            using var context = new AppDbContext();
            var query = context.Inventories
                .Include(i => i.CreatedByNavigation)
                .AsQueryable();

            if (selectedStatus == "V processe")
            {
                query = query.Where(i => i.Status == "in_progress");
            }
            else if (selectedStatus == "Zavershen")
            {
                query = query.Where(i => i.Status == "completed");
            }

            InventoryGrid.ItemsSource = query.ToList();
        }

        private Window GetWindow()
        {
            return (Window)VisualRoot;
        }

        private void ShowError(string message)
        {
            var dialog = new Window
            {
                Title = "Informaciya",
                Content = new TextBlock { Text = message, Margin = new Thickness(20) },
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dialog.ShowDialog(GetWindow());
        }
    }
}