using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Warehouse.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace Warehouse
{
    public partial class InventoryCountDialog : Window
    {
        private Inventory _inventory;
        private List<InventoryDiscrepancy> _discrepancies;

        public InventoryCountDialog()
        {
            InitializeComponent();
        }

        public InventoryCountDialog(Inventory inventory) : this()
        {
            _inventory = inventory;
            LoadInventoryData();
        }

        private void LoadInventoryData()
        {
            TxtInventoryNumber.Text = _inventory.InventoryNumber;
            TxtLocation.Text = _inventory.Location;
            TxtInventoryDate.Text = _inventory.InventoryDate.ToString("dd.MM.yyyy");

            LoadDiscrepancies();
            UpdateStatistics();
        }

        private int GetExpectedQuantity(int productId, string location)
        {
            using var context = new AppDbContext();

            // Суммируем все остатки данного товара на указанном складе
            var totalQuantity = context.Stocks
                .Where(s => s.ProductId == productId && s.Location == location)
                .Sum(s => (int?)s.Quantity) ?? 0;

            return totalQuantity;
        }

        private void LoadDiscrepancies()
        {
            using var context = new AppDbContext();

           
            var existingDiscrepancies = context.InventoryDiscrepancies
                .Include(d => d.Product)
                .Where(d => d.InventoryId == _inventory.Id)
                .ToList();

            var allProducts = context.Products.ToList();

            _discrepancies = new List<InventoryDiscrepancy>();

            foreach (var product in allProducts)
            {
                var existing = existingDiscrepancies.FirstOrDefault(d => d.ProductId == product.Id);

                
                int expectedQuantity = GetExpectedQuantity(product.Id, _inventory.Location);

                if (existing != null)
                {
                    
                    existing.Difference = existing.ActualQuantity - expectedQuantity;
                    _discrepancies.Add(existing);
                }
                else
                {
                    
                    var newDiscrepancy = new InventoryDiscrepancy
                    {
                        InventoryId = _inventory.Id,
                        ProductId = product.Id,
                        Product = product,
                        ExpectedQuantity = expectedQuantity, 
                        ActualQuantity = 0,                 
                        Difference = -expectedQuantity,     
                        Notes = ""
                    };
                    _discrepancies.Add(newDiscrepancy);
                }
            }

            CountGrid.ItemsSource = _discrepancies;
        }

        private void UpdateStatistics()
        {
            if (_discrepancies == null) return;

            int totalItems = _discrepancies.Count;
            int matchedItems = _discrepancies.Count(i => i.Difference == 0);
            int discrepancyItems = _discrepancies.Count(i => i.Difference != 0);

            TxtTotalItems.Text = $"Vsego tovarov: {totalItems}";
            TxtMatchedItems.Text = $"Sovpalo: {matchedItems}";
            TxtDiscrepancies.Text = $"Raskhojdeniy: {discrepancyItems}";
        }

        private void BtnAutoFill_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (var discrepancy in _discrepancies)
            {
                discrepancy.ActualQuantity = discrepancy.ExpectedQuantity;
                discrepancy.Difference = 0;
                discrepancy.Notes = "Net primechaniy";
            }

            CountGrid.ItemsSource = null;
            CountGrid.ItemsSource = _discrepancies;
            UpdateStatistics();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var context = new AppDbContext();

               
                foreach (var discrepancy in _discrepancies)
                {
                    
                    discrepancy.Difference = discrepancy.ActualQuantity - discrepancy.ExpectedQuantity;

                    var existing = context.InventoryDiscrepancies
                        .FirstOrDefault(d => d.InventoryId == discrepancy.InventoryId && d.ProductId == discrepancy.ProductId);

                    if (existing != null)
                    {
                        
                        existing.ExpectedQuantity = discrepancy.ExpectedQuantity;
                        existing.ActualQuantity = discrepancy.ActualQuantity;
                        existing.Difference = discrepancy.Difference;
                        existing.Notes = discrepancy.Notes;
                    }
                    else
                    {
                        
                        context.InventoryDiscrepancies.Add(discrepancy);
                    }
                }

                context.SaveChanges();
                Close(true);
            }
            catch (Exception ex)
            {
                ShowError($"Oshibka sohraneniya: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
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