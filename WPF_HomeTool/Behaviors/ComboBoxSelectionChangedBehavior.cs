using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Behaviors
{
    public static class ComboBoxSelectionChangedBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(ComboBoxSelectionChangedBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(DependencyObject obj, ICommand value)
            => obj.SetValue(CommandProperty, value);

        public static ICommand GetCommand(DependencyObject obj)
            => (ICommand)obj.GetValue(CommandProperty);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboBox cb)
            {
                cb.SelectionChanged -= Cb_SelectionChanged;
                cb.SelectionChanged += Cb_SelectionChanged;
            }
        }

        private static void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            var cmd = GetCommand(cb);
            if (cmd?.CanExecute(cb.SelectedItem) == true)
                cmd.Execute(cb.SelectedItem);
        }
    }
}
