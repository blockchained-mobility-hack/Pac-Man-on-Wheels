namespace Pac_Man_on_wheels.Services
{
  using System;
  using System.Globalization;

  using Xamarin.Forms;

  public class StringToColorConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string valueAsString = value.ToString();
      switch (valueAsString)
      {
        case (""):
          {
            return Color.Default;
          }
        case ("Accent"):
          {
            return Color.Accent;
          }
        default:
          {
            return Color.FromHex(value.ToString());
          }
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}
