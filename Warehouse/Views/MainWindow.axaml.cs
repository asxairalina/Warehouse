using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Linq;
using Warehouse.Data;

namespace Warehouse
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = TxtUsername.Text;
            var password = TxtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Введите логин и пароль");
                return;
            }

            try
            {
                var context = new AppDbContext();
                var user = context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

                if (user != null && user.IsActive == true)
                {
                    var mainView = new MainMenuPage(user);
                    mainView.Show();
                    this.Close();
                }
                else
                {
                    ShowError("Неверный логин или пароль");
                }
                context.Dispose();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
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