using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Behaviors
{
    public static class TextBoxScrollBehavior
    {
        // The name of the attached property
        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollToEnd",
                typeof(bool),
                typeof(TextBoxScrollBehavior),
                new PropertyMetadata(false, OnAutoScrollToEndChanged));

        public static bool GetAutoScrollToEnd(TextBox obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        public static void SetAutoScrollToEnd(TextBox obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        private static void OnAutoScrollToEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox && (bool)e.NewValue)
            {
                // Subscribe to the TextChanged event when the property is set to true
                textBox.TextChanged += TextBox_TextChanged;
            }
            else if (d is TextBox textBox2)
            {
                // Unsubscribe when the property is set to false (good practice)
                textBox2.TextChanged -= TextBox_TextChanged;
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.ScrollToEnd();
            }
        }
    }
}
