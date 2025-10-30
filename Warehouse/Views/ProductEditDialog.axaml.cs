using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Warehouse.Data;

namespace Warehouse
{
    public partial class ProductEditDialog : Window
    {
        private Product _product;
        private bool _isNew;

        public ProductEditDialog()
        {
            InitializeComponent();
            _isNew = true;
            _product = new Product();
            InitializeControls();
        }

        public ProductEditDialog(Product product) : this()
        {
            _isNew = false;
            _product = product;
            LoadProductData();
        }

        private void InitializeControls()
        {
            var context = new AppDbContext();
            var categories = context.Categories.ToList();
            CmbCategory.ItemsSource = categories;
            context.Dispose();

            Title = _isNew ? "Добавление товара" : "Редактирование товара";
        }

        private void LoadProductData()
        {
            TxtName.Text = _product.Name;
            TxtDescription.Text = _product.Description;
            TxtUnit.Text = _product.Unit;
            TxtPurchasePrice.Text = _product.PurchasePrice.ToString("F2");
            TxtSellingPrice.Text = _product.SellingPrice.ToString("F2");
            ChkIsActive.IsChecked = _product.IsActive;

            if (_product.CategoryId.HasValue)
            {
                var context = new AppDbContext();
                var category = context.Categories.Find(_product.CategoryId.Value);
                CmbCategory.SelectedItem = category;
                context.Dispose();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                ShowError("Введите наименование товара");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtUnit.Text))
            {
                ShowError("Введите единицу измерения");
                return;
            }

            if (!decimal.TryParse(TxtPurchasePrice.Text, out decimal purchasePrice) || purchasePrice < 0)
            {
                ShowError("Некорректная цена закупки");
                return;
            }

            if (!decimal.TryParse(TxtSellingPrice.Text, out decimal sellingPrice) || sellingPrice < 0)
            {
                ShowError("Некорректная цена продажи");
                return;
            }

            var context = new AppDbContext();

            _product.Name = TxtName.Text.Trim();
            _product.Description = TxtDescription.Text?.Trim();
            _product.Unit = TxtUnit.Text.Trim();
            _product.PurchasePrice = purchasePrice;
            _product.SellingPrice = sellingPrice;
            _product.IsActive = ChkIsActive.IsChecked ?? true;

            if (CmbCategory.SelectedItem is Category selectedCategory)
            {
                _product.CategoryId = selectedCategory.Id;
            }
            else
            {
                _product.CategoryId = null;
            }

            if (_isNew)
            {
                context.Products.Add(_product);
            }
            else
            {
                context.Products.Update(_product);
            }

            context.SaveChanges();
            context.Dispose();
            Close(true);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close(false);
        }

        private void ShowError(string message)
        {
            var dialog = new Window
            {
                Title = "Ошибка",
                Content = new TextBlock { Text = message, Margin = new Thickness(20) },
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dialog.ShowDialog(this);
        }
    }
}