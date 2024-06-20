using System.Windows;
using System.Windows.Controls;

namespace HamsterKombatAssistant
{
    public partial class MainWindow
    {
        private bool _itemslistViewResized = false;

        public void ItemsEditButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ((MenuItem)sender).Tag as HkItem;
            if (item == null) return;

            var editItemWindow = new EditItemWindow(item, _logic, this);
            editItemWindow.ShowDialog();
        }

        internal void ResizeItemList(ListView listView, double[] columnsWidth)
        {   
            if (listView == null) return;

            GridView gView = (GridView)listView.View;
            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;

            for (var i = 0; i < gView.Columns.Count; i++)
                gView.Columns[i].Width = workingWidth * columnsWidth[i];
        }

        internal void ItemsListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_itemslistViewResized || sender is not ListView) return;

            var listView = (ListView)sender;
            ResizeItemList(listView, [0.15, 0.05, 0.30, 0.1, 0.15, 0.1, 0.15]);
            _itemslistViewResized = true;
        }

        internal void ResizeVariantsItemsList() => 
            ResizeItemList(ItemsOfVariantList, [0.15, 0.05, 0.30, 0.1, 0.15, 0.1, 0.15]);

        internal void ResizeVariantsList() =>
            ResizeItemList(VariantsList, [0.3, 0.45, 0.25]);
    }
}
