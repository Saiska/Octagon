using System.Windows;
using System.Windows.Controls;
using Octagon.Models;

namespace Octagon.UserControls
{
   /// <summary>
   /// Interaction logic for FilterShow.xaml
   /// </summary>
   public partial class FilterShow : UserControl
   {
      public FilterShow()
      {
         InitializeComponent();
      }

      public static readonly DependencyProperty ModelConfigurationSelectionProperty = DependencyProperty.Register(
         "ModelConfigurationSelection", typeof(ModelConfigurationSelection), typeof(FilterShow), new PropertyMetadata(default(ModelConfigurationSelection)));

      public ModelConfigurationSelection ModelConfigurationSelection
      {
         get { return (ModelConfigurationSelection) GetValue(ModelConfigurationSelectionProperty); }
         set { SetValue(ModelConfigurationSelectionProperty, value); }
      }


   }
}
