using System;
using System.Windows;
using System.Windows.Controls;

namespace CommonCode
{
    public class ColumnDefinitionExtended : ColumnDefinition
    {
        // Variables
        public static DependencyProperty VisibleProperty;

        // Properties
        public Boolean Visible
        {
            get { return (Boolean)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        // Constructors
        static ColumnDefinitionExtended()
        {
            VisibleProperty = DependencyProperty.Register("Visible",
                typeof(Boolean),
                typeof(ColumnDefinitionExtended),
                new PropertyMetadata(true, OnVisibleChanged));

            WidthProperty.OverrideMetadata(typeof(ColumnDefinitionExtended),
                new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), null,
                    CoerceWidth));

            MinWidthProperty.OverrideMetadata(typeof(ColumnDefinitionExtended),
                new FrameworkPropertyMetadata((Double)0, null,
                    CoerceMinWidth));
        }

        // Get/Set
        public static void SetVisible(DependencyObject obj, Boolean nVisible)
        {
            obj.SetValue(VisibleProperty, nVisible);
        }
        public static Boolean GetVisible(DependencyObject obj)
        {
            return (Boolean)obj.GetValue(VisibleProperty);
        }

        static void OnVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.CoerceValue(WidthProperty);
            obj.CoerceValue(MinWidthProperty);
        }
        static Object CoerceWidth(DependencyObject obj, Object nValue)
        {
            return (((ColumnDefinitionExtended)obj).Visible) ? nValue : new GridLength(0);
        }
        static Object CoerceMinWidth(DependencyObject obj, Object nValue)
        {
            return (((ColumnDefinitionExtended)obj).Visible) ? nValue : (Double)0;
        }
    }
}