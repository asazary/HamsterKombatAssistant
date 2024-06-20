using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace HamsterKombatAssistant
{
    /// <summary>
    /// Interaction logic for EditItemWindow.xaml
    /// </summary>
    public partial class EditItemWindow : Window
    {
        private HkItem _item;
        private Logic _logic;
        public static event Action<HkItem>? ItemEditedEvent;

        public EditItemWindow(HkItem hkItem, Logic logic, Window mainWindow)
        {
            InitializeComponent();
            _item = hkItem;
            _logic = logic;

            ItemName.Text = hkItem.Name;
            GroupName.Text = hkItem.GroupName;
            ItemLevelValue.Text = hkItem.Level.ToString();
            ItemValue.Text = hkItem.Value.ToString();
            ItemIncValue.Text = hkItem.Inc.ToString();
            ItemIncCostValue.Text = hkItem.IncCost.ToString();
            Owner = mainWindow;
        }
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = CheckNumberRegex();
            e.Handled = regex.IsMatch(e.Text);
        }

        private void EditItemSaveButton_Click(object sender, RoutedEventArgs e)
        {
            _item.Level = int.Parse(ItemLevelValue.Text);
            _item.Value = int.Parse(ItemValue.Text);
            _item.Inc = int.Parse(ItemIncValue.Text);
            _item.IncCost = int.Parse(ItemIncCostValue.Text);

            ItemEditedEvent?.Invoke(_item);
            Close();
        }

        private void EditItemCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        [GeneratedRegex("[^0-9]+")]
        private static partial Regex CheckNumberRegex();
    }
}
