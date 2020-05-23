using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Octagon.Converter
{
   public class IntToStringConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (value == null)
            return "0";
         return value.ToString();
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {  
         int ret = 0;
         return int.TryParse((string)value, out ret) ? ret : 0;
      }
   }
}
