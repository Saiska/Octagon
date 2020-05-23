using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Octagon.Models;

namespace Octagon.UserControls
{
   /// <summary>
   /// Interaction logic for RepackShow.xaml
   /// </summary>
   public partial class RepackShow : UserControl
   {


      public static readonly DependencyProperty ModelConfigurationPassBsaProperty = DependencyProperty.Register(
         "ModelConfigurationPassBsa", typeof(ModelConfigurationPassBsa), typeof(RepackShow), new PropertyMetadata(default(ModelConfigurationPassBsa)));

      public ModelConfigurationPassBsa ModelConfigurationPassBsa
      {
         get { return (ModelConfigurationPassBsa)GetValue(ModelConfigurationPassBsaProperty); }
         set { SetValue(ModelConfigurationPassBsaProperty, value); }
      }

      public RepackShow()
      {
         InitializeComponent();
      }


      private void ButtonAddRepack(object sender, RoutedEventArgs e)
      {
         ModelConfigurationPassBsa.AddRepack();
      }

      private void ButtonRemoveRepack(object sender, RoutedEventArgs e)
      {
         ModelConfigurationPassBsa.RemoveRepack();
      }

      private void ButtonDefaultRepack(object sender, RoutedEventArgs e)
      {
         ModelConfigurationPassBsa.DefaultRepack();
      }
   }
}
