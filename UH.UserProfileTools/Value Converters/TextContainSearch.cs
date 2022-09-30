using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UH.UserProfileTools
{
    public sealed class TextContainSearch : DependencyObject
    {
        public static void SetText(DependencyObject element, string text)
        {
            var controlSearch = element as Control;
            if (controlSearch != null)
                controlSearch.KeyUp += (sender, e) =>
                {
                    if (sender is ComboBox)
                    {
                        var control = sender as ComboBox;
                        control.IsDropDownOpen = true;
                        var oldText = control.Text;
                        foreach (var itemFromSource in control.ItemsSource)
                        {
                            if (itemFromSource != null)
                            {
                                Object simpleType = itemFromSource.GetType().GetProperty(text).GetValue(itemFromSource, null);
                                String propertOfList = simpleType as string;
                                if (!string.IsNullOrEmpty(propertOfList) && propertOfList.Contains(control.Text))
                                {
                                    control.SelectedItem = itemFromSource;
                                    control.Items.MoveCurrentTo(itemFromSource);
                                    break;
                                }
                            }
                        }
                        control.Text = oldText;
                        TextBox txt = control.Template.FindName("PART_EditableTextBox", control) as TextBox;
                        if (txt != null)
                        {
                            txt.Select(txt.Text.Length, 0);
                        }
                    }
                };
        }

    }
}
