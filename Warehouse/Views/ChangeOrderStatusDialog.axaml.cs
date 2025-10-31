using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Warehouse.Data;

namespace Warehouse
{
    public partial class ChangeOrderStatusDialog : Window
    {
        private CustomerOrder _order;

        // Конструктор без параметров
        public ChangeOrderStatusDialog()
        {
            InitializeComponent();
        }

        // Конструктор с параметром
        public ChangeOrderStatusDialog(CustomerOrder order) : this()
        {
            _order = order;
            LoadOrderInfo();
        }

        private void LoadOrderInfo()
        {
            TxtOrderInfo.Text = $"Zakaz №{_order.OrderNumber} ot {_order.OrderDate:dd.MM.yyyy}";

            // Устанавливаем текущий статус
            if (CmbStatus.Items != null)
            {
                foreach (ComboBoxItem item in CmbStatus.Items)
                {
                    if (item.Tag?.ToString() == _order.Status)
                    {
                        CmbStatus.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = CmbStatus.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
            {
                // Показываем ошибку, если статус не выбран
                var errorDialog = new Window
                {
                    Title = "Oshibka",
                    Content = new TextBlock { Text = "Vyberite status", Margin = new Thickness(20) },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                errorDialog.ShowDialog(this);
                return;
            }

            var newStatus = selectedItem.Tag.ToString();

            using var context = new AppDbContext();
            var order = context.CustomerOrders.Find(_order.Id);
            if (order != null)
            {
                order.Status = newStatus;
                context.SaveChanges();
            }

            Close(true);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}