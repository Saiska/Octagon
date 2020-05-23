using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Octagon.Windows
{
   /// <summary>
   /// Interaction logic for WindowCredits.xaml
   /// </summary>
   public partial class WindowCredits : Window
   {
      public WindowCredits()
      {
         InitializeComponent();
      }
      private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
      {
         Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
         e.Handled = true;
      }
   }
}
