using System;
using System.Windows;
using Octagon.Models;
using Octagon.Tools;

namespace Octagon.Windows
{
   /// <summary>
   /// Interaction logic for WindowFilter.xaml
   /// </summary>
   public partial class WindowFilter : Window
   {
      public ModelConfigurationSelection ModelConfigurationSelection { get; set; }

      public WindowFilter()
      {
         InitializeComponent();
         this.Height = (SystemParameters.PrimaryScreenHeight * 0.75);
         this.Width = (SystemParameters.PrimaryScreenWidth * 0.75);                
      }

      private void ButtonAddFilter(object sender, RoutedEventArgs e)
      {
         ModelConfigurationSelection.AddFilter();
      }

      private void ButtonInvert(object sender, RoutedEventArgs e)
      {
         ModelConfigurationSelection.Invert();
      }                   

      private void ButtonExportFilters(object sender, RoutedEventArgs e)
      {
         try
         {
            var file = FileTools.GetFileFilters(@"Please select a file to save the current filters", true, null);
            if (file != null)
               ModelConfigurationSelection.Save(file);
         }
         catch (Exception ex)
         {
            MessageTools.ShowError(ex.Message);
         }                                  
      }

      private void ButtonImportFilters(object sender, RoutedEventArgs e)
      {
         try
         {
            var file = FileTools.GetFileFilters(@"Please select a filters file to open", false, null);
            if (file != null)
               ModelConfigurationSelection.Open(file);
         }
         catch (Exception ex)
         {
            MessageTools.ShowError(ex.Message);
         }                                 
      }

      private void ButtonShowInfo(object sender, RoutedEventArgs e)
      {
         ModelConfigurationSelection.ChangeShowInfo();
      }                                 

      private void ButtonClose(object sender, RoutedEventArgs e)
      {
         Close();
      }
   }
}
