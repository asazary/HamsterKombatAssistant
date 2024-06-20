using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HamsterKombatAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string DefaultFileName = "hka_data.csv";
        private static readonly string _appPath = AppDomain.CurrentDomain.BaseDirectory; //Assembly.GetExecutingAssembly().Location;
        private readonly string _defaultFilePath = Path.Combine(_appPath, DefaultFileName);
        public readonly Logic _logic;
        private string FilePath;
        private CancellationTokenSource? _cancellationTokenSource = null;


        public MainWindow()
        {
            InitializeComponent();
            MenuAlignmentFix();

            _logic = new Logic();
            ItemsList.ItemsSource = _logic.Items;
            ItemsOfVariantList.ItemsSource = _logic.ItemsOfVariant;
            VariantsList.ItemsSource = _logic.Variants;
            _logic.RecalcDoneEvent += RefreshCurrentInc;
            _logic.VariantsCountChanged += UpdateVariantsCount;
            _logic.VariantsStackCountChanged += UpdateVariantsStackCount;
            ItemsList.SizeChanged += ItemsListView_SizeChanged;

            FilePath = _defaultFilePath;

            if (File.Exists(FilePath))
            {
                _logic.LoadDataFromFile(FilePath);
            }

            Height = 500;

            //MessageBox.Show(_logic.Test());
        }

        private void ImportFromCsvButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = "csv",
                Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            var dlgResult = dlg.ShowDialog();
            if (dlgResult == true)
            {
                _logic.ImportFromCsv(dlg.FileName);                
            }
        }

        private void OpenButton_Click(Object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory,
                DefaultExt = "csv",
                Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            var dlgResult = dlg.ShowDialog();
            if (dlgResult == true)
            {
                FilePath = dlg.FileName;
                _logic.LoadDataFromFile(dlg.FileName);
            }
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e) {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory,
                Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "csv",
                OverwritePrompt = true,
                FileName = DefaultFileName
            };

            var dlgResult = dlg.ShowDialog();
            
            if (dlgResult == true)
            {
                File.WriteAllLines(dlg.FileName, _logic.GetItemsAsCsvList());               
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            if (File.Exists(FilePath)) File.Delete(FilePath);
            File.WriteAllLines(FilePath, _logic.GetItemsAsCsvList());
        } 

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void GoButton_Click(object sender, RoutedEventArgs e)
        {
            GoButton.IsEnabled = false;
            VariantsList.IsEnabled = false;
            ItemsOfVariantList.IsEnabled = false;

            _cancellationTokenSource = new CancellationTokenSource();

            var money = int.Parse(MoneyField.Text);
            var task = Task.Run(() => _logic
            .CalculateVariants(money, _cancellationTokenSource.Token),
                _cancellationTokenSource.Token);

            await task;
            var variants = task.Result;

            int maxShow = int.Parse(MaxShowField.Text);

            _logic.Variants.Clear();
            _logic.ItemsOfVariant.Clear();
            foreach (var variant in variants!.ToList()
                .OrderByDescending(x => x.IncSum).Take(maxShow))
                _logic.Variants.Add(variant);

            GoButton.IsEnabled = true;
            VariantsList.IsEnabled = true;
            ItemsOfVariantList.IsEnabled = true;

            ResizeVariantsList();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
            GoButton.IsEnabled = true;
            VariantsList.IsEnabled = true;
            ItemsOfVariantList.IsEnabled = true;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow(this);
            aboutWindow.ShowDialog();
        }

        private void MenuAlignmentFix()
        {
            var ifLeft = SystemParameters.MenuDropAlignment;
            if (ifLeft)
            {
                // change to false
                var t = typeof(SystemParameters);
                var field = t.GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
                field?.SetValue(null, false);
                _ = SystemParameters.MenuDropAlignment;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = CheckNumberRegex();
            e.Handled = regex.IsMatch(e.Text);
        }

        private void RefreshCurrentInc(int sumInc) =>
            CurrentIncText.Text = $"Current Inc: {sumInc}";
        [GeneratedRegex("[^0-9]+")]
        private static partial Regex CheckNumberRegex();

        private void UpdateVariantsCount(int count) =>
            Dispatcher.Invoke(() => VariantsCountText.Text = $"variants found: {count}");

        private void UpdateVariantsStackCount(int count) =>
            Dispatcher.Invoke(() => VariantsStackCountText.Text = $"stack: {count}");


        private void VariantsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListView)sender;
            if (list != null && list?.SelectedItem is Variant variant)
            {                
                _logic.ItemsOfVariant.Clear();
                foreach (var item in variant.Items.OrderBy(x => x.Id))
                    _logic.ItemsOfVariant.Add(item);

                ResizeVariantsItemsList();
            }            
        }        
    }
}