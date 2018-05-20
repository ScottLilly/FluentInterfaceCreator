using System.Windows;

namespace FluentInterfaceCreator.Converters
{
    public class ConfigurableBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public ConfigurableBooleanToVisibilityConverter() 
            : base(Visibility.Visible, Visibility.Collapsed)
        {
        }
    }
}