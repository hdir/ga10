using Bare10.Resources;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bare10.Utils
{
    public static partial class Accessibility
    {
        public static event Action<bool> AccessibilityEnabledChanged;

        private static bool accessibilityEnabled;
        public static bool AccessibilityEnabled
        {
            get => accessibilityEnabled;
            set
            {
                if(accessibilityEnabled != value)
                {
                    accessibilityEnabled = value;
                    AccessibilityEnabledChanged?.Invoke(value);
                }
            }
        }

        public static void Unused(BindableObject bindable)
        {
            AutomationProperties.SetIsInAccessibleTree(bindable, false);
        }

        public static void InUse(BindableObject bindable)
        {
            AutomationProperties.SetIsInAccessibleTree(bindable, true);
        }

        public static void InUse(BindableObject bindable, string name, string helpText = "")
        {
            AutomationProperties.SetIsInAccessibleTree(bindable, true);
            AutomationProperties.SetName(bindable, name);
            AutomationProperties.SetHelpText(bindable, helpText);
        }

        public static void InUse(BindableObject bindable, string name, BindingBase helpTextBinding = null)
        {
            AutomationProperties.SetIsInAccessibleTree(bindable, true);
            AutomationProperties.SetName(bindable, name);
            bindable.SetBinding(AutomationProperties.HelpTextProperty, helpTextBinding);
        }

        public static void InUse(BindableObject bindable, BindingBase nameBinding, BindingBase helpTextBinding = null)
        {
            AutomationProperties.SetIsInAccessibleTree(bindable, true);
            bindable.SetBinding(AutomationProperties.NameProperty, nameBinding);
            if(helpTextBinding != null)
            {
                bindable.SetBinding(AutomationProperties.HelpTextProperty, helpTextBinding);
            }
        }

        public static void LabeledBy(VisualElement element, VisualElement labeledBy)
        {
            AutomationProperties.SetLabeledBy(element, labeledBy);
        }

        public static void NotTabbedTo(VisualElement element)
        {
            element.IsTabStop = false;
        }

        public static void SetTabOrder(VisualElement element, int tabIndex)
        {
            element.IsTabStop = true;
            element.TabIndex = tabIndex;
        }

        public static void SetTabOrderByIndex(List<VisualElement> elements)
        {
            for(int i = 0; i < elements.Count; ++i)
            {
                elements[i].IsTabStop = true;
                elements[i].TabIndex = i;
            }
        }

        public static Label CreateLabel(string text)
        {
            var label = new Label
            {
                FontSize = Sizes.TextMedium,
                Text = text,
            };
            InUse(label);
            return label;
        }

        public static Label CreateLabelWithoutName(string textPropertyBindingName)
        {
            var label = new Label
            {
                FontSize = Sizes.TextMedium,
            };
            label.SetBinding(Label.TextProperty, textPropertyBindingName);
            InUse(label);

            return label;
        }

        public static Label CreateLabel(string name, string textPropertyBindingName)
        {
            var label = new Label
            {
                FontSize = Sizes.TextMedium,
            };
            label.SetBinding(Label.TextProperty, textPropertyBindingName);
            InUse(label, name, new Binding(textPropertyBindingName));

            return label;
        }

        public static Button CreateButton(string name, string clickPropertyBindingName, string textPropertyBindingName = null)
        {
            var button = new Button();
            if(textPropertyBindingName != null)
            {
                button.SetBinding(Button.TextProperty, textPropertyBindingName);
            }
            else
            {
                button.Text = name;
            }
            button.SetBinding(Button.CommandProperty, clickPropertyBindingName);
            InUse(button, name, "");
            return button;
        }

        public static Button CreateButton(string name, ICommand clickCommand, string textPropertyBindingName = null)
        {
            var button = new Button
            {
                Command = clickCommand,
            };
            if(textPropertyBindingName != null)
            {
                button.SetBinding(Button.TextProperty, textPropertyBindingName);
            }
            else
            {
                button.Text = name;
            }
            InUse(button, name, "");
            return button;
        }

        /// <summary>
        /// Creates an accessible-ready list
        /// </summary>
        /// <param name="name">The spoken name of the list</param>
        /// <param name="listPropertyName">the nameof the property to bind for ItemsSource</param>
        /// <param name="itemTemplateType">The ItemTemplate type. <b>Defaults to TextCell if null</b></param>
        /// <param name="hasUnevenRows">Whether or the list should allow cells to fill vertically</param>
        /// <returns></returns>
        public static ListView CreateList(string name, string listPropertyName, Type itemTemplateType = null, bool hasUnevenRows = true)
        {
            var list = new ListView
            {
                VerticalOptions = LayoutOptions.Start,
                HasUnevenRows = hasUnevenRows,
                BackgroundColor = Color.Transparent,
            };
            if(itemTemplateType != null)
            {
                list.ItemTemplate = new DataTemplate(itemTemplateType);
            }
            list.SetBinding(ListView.ItemsSourceProperty, listPropertyName);
            InUse(list, name, "");

            return list;
        }

        public static ListView CreateListWithHeader(string name, string listPropertyName, Type itemTemplateType = null, bool hasUnevenRows = true)
        {
            ListView list = CreateList(name, listPropertyName, itemTemplateType, hasUnevenRows);
            list.Header = name;
            return list;
        }

        public static Entry CreateEntry(string name, string textPropertyName)
        {
            var entry = new Entry
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                FontSize = Sizes.Title,
            };
            entry.SetBinding(Entry.TextProperty, textPropertyName, BindingMode.TwoWay);
            InUse(entry, name, "");

            return entry;
        }

        public static View CreateLabelledSwitch(string name, string togglePropertyName)
        {
            var label = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontSize = Sizes.TextMedium,
                Text = name,
            };

            var aSwitch = new Switch();
            aSwitch.SetBinding(Switch.IsToggledProperty, togglePropertyName, BindingMode.TwoWay);
            AutomationProperties.SetIsInAccessibleTree(aSwitch, true);
            AutomationProperties.SetLabeledBy(aSwitch, label);

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    label,
                    aSwitch,
                },
            };

            return layout;
        }

        public static Switch CreateSwitch(string name, string togglePropertyName)
        {
            var aSwitch = new Switch();
            aSwitch.SetBinding(Switch.IsToggledProperty, togglePropertyName, BindingMode.TwoWay);
            AutomationProperties.SetIsInAccessibleTree(aSwitch, true);
            //InUse(aSwitch, name, "");
            return aSwitch;
        }
    }
}
