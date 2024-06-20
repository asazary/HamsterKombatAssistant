using System.Windows;

namespace HamsterKombatAssistant
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow(Window mainWindow)
        {
            InitializeComponent();

            Owner = mainWindow;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
