using System;
using System.ComponentModel;
using System.Windows;
using Octagon.Models;

namespace Octagon.Windows
{
   /// <summary>
   /// Interaction logic for WindowProcessor.xaml
   /// </summary>
   public partial class WindowProcessor : Window
   {
      public WindowProcessor()
      {
         InitializeComponent();
         this.Height = (SystemParameters.PrimaryScreenHeight * 0.75);
         this.Width = (SystemParameters.PrimaryScreenWidth * 0.75);
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         e.Cancel = ModelProcessor.TypeVisibility != ModelProcessor.TypeBottomVisibility.Close && !_forceClose;
         base.OnClosing(e);
      }

      public ModelProcessor ModelProcessor { get; set; }

      bool _shown;
      protected override void OnContentRendered(EventArgs e)
      {
         base.OnContentRendered(e);

         if (_shown)
            return;

         _shown = true;
         ModelProcessor.Start();
      }

      bool _forceClose;
      private void ButtonClose(object sender, RoutedEventArgs e)
      {
         _forceClose = true;
         Close();
      }

      private void ButtonContinue(object sender, RoutedEventArgs e)
      {
         ModelProcessor.Continue();
      }
   }
}
