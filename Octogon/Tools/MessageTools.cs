using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Octagon.Tools
{
   public class MessageTools
   {
      public static void ShowError(string msg)
      {
         MessageBox.Show(msg, "Octagon Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }

   }
}
