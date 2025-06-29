using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace PTDesign.Library
{
    public class TextBoxUpdateOnEnterOrLostFocusBehavior : DependencyObject
    {
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached(
                "Enable",
                typeof(bool),
                typeof(TextBoxUpdateOnEnterOrLostFocusBehavior),
                new UIPropertyMetadata(false, OnEnableChanged));

        public static bool GetEnable(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableProperty);
        }

        public static void SetEnable(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableProperty, value);
        }

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox)
                return;

            if ((bool)e.NewValue)
            {
                textBox.LostFocus += TextBox_LostFocus;
                textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            }
            else
            {
                textBox.LostFocus -= TextBox_LostFocus;
                textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            }
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
                binding?.UpdateSource();
            }
        }
    }
}
