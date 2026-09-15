using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF_HomeTool.Helpers
{
    /// <summary>
    /// DataGridHelper 类提供了一个附加属性，用于在 WPF DataGrid 中启用自动滚动到选定项的功能。
    /// </summary>
    public static class DataGridHelper
    {
        // 定义附加属性：AutoScrollToSelectedItem
        public static readonly DependencyProperty AutoScrollToSelectedItemProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollToSelectedItem",
                typeof(bool),
                typeof(DataGridHelper),
                new PropertyMetadata(false, OnAutoScrollToSelectedItemChanged));

        public static bool GetAutoScrollToSelectedItem(DependencyObject obj)
            => (bool)obj.GetValue(AutoScrollToSelectedItemProperty);

        public static void SetAutoScrollToSelectedItem(DependencyObject obj, bool value)
            => obj.SetValue(AutoScrollToSelectedItemProperty, value);

        // Added: Property changed callback
        private static void OnAutoScrollToSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                if ((bool)e.NewValue)
                    dataGrid.SelectionChanged += DataGrid_SelectionChanged;
                else
                    dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
            }
        }

        // Added: SelectionChanged handler that scrolls the selected item into view
        private static void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dg && dg.SelectedItem != null)
            {
                // Ensure layout is updated before scrolling
                dg.Dispatcher.BeginInvoke((Action)(() =>
                {
                    dg.UpdateLayout();
                    dg.ScrollIntoView(dg.SelectedItem);
                }));
            }
        }
    }
}
