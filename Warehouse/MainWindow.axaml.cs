// MainWindow.axaml.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WarehouseApp.Views;

namespace WarehouseApp
{
    public partial class MainWindow : Window
    {
        private MainPage _mainPage;
        private StockPage _stockPage;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            // Создаём страницы один раз — контент сохраняется
            _mainPage = new MainPage();
            _stockPage = new StockPage();

            MainContent.Content = _mainPage;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void BtnMain_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContent.Content = _mainPage;
        }

        private void BtnStock_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainContent.Content = _stockPage;
        }
    }
}