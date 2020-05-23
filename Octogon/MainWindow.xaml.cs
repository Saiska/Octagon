using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Navigation;
using Octagon.Models;
using Octagon.Tools;
using Octagon.Windows;
using OctagonCommon;
using OctagonCommon.Args;
using Application = System.Windows.Forms.Application;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;

namespace Octagon
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public ModelConfiguration ModelConfiguration { get; set; }

      public MainWindow()
      {
         InitializeComponent();
         this.Height = (SystemParameters.PrimaryScreenHeight * 0.75);
         this.Width = (SystemParameters.PrimaryScreenWidth * 0.75);
         ModelConfiguration = new ModelConfiguration();
         DataContext = ModelConfiguration;
         this.Closing += OnClosing;
      }

      private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
      {
         ModelConfiguration.Save();
      }

      private void SetSourceDir(object sender, RoutedEventArgs e)
      {
         using (var dialog = new FolderBrowserDialog())
         {
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
               ModelConfiguration.Main.PathSource = dialog.SelectedPath;
            }
         }
      }

      private void SetMergeDir(object sender, RoutedEventArgs e)
      {
         using (var dialog = new FolderBrowserDialog())
         {
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
               ModelConfiguration.Main.PathMergeDirectory = dialog.SelectedPath;
            }
         }
      }

      private void SetBackupDir(object sender, RoutedEventArgs e)
      {
         using (var dialog = new FolderBrowserDialog())
         {
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
               ModelConfiguration.Main.PathBackup = dialog.SelectedPath;
            }
         }
      }

      private void ButtonAddFormatPass(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Main.AddFormatPass();
      }

      private void ButtonAddDividePass(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Main.AddDividePass();
      }

      private void ButtonAddFixedPass(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Main.AddFixedPass();
      }

      private void ButtonAddUpscaleFactor(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Main.AddUpscaleFactor();
      }

      private void ButtonAddUpscaleFixed(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Main.AddUpscaleFixed();
      }

      private void ButtonAddCorrectSize(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Main.AddCorrectSize();
      }

      private void ButtonAddGmic(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Main.AddGmic();
      }

      private void ButtonAddForce(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Main.AddForce();
      }

      private void ButtonAddCustom(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Main.AddCustom();
      }

      private void ButtonAddForceMipmap(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Main.AddForceMipmap();
      }

      private void ButtonAddCorrectMipmap(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Main.AddCorrectMipmap();
      }                     

      private void ShowSourcePathFilter(object sender, RoutedEventArgs e)
      {
         var w = new WindowFilter { ModelConfigurationSelection = ModelConfiguration.Main.ModelSelection };
         w.DataContext = w.ModelConfigurationSelection;
         w.ShowDialog();
      }

      private void ShowBsaFilter(object sender, RoutedEventArgs e)
      {
         var w = new WindowFilter { ModelConfigurationSelection = ModelConfiguration.Main.ModelPassBsa.ModelSelection };
         w.DataContext = w.ModelConfigurationSelection;
         w.ShowDialog();

      }             

      private void ButtonExit(object sender, RoutedEventArgs e)
      {
         ModelConfiguration.Save();
         System.Windows.Application.Current.Shutdown();
      }

      private void ButtonCredits(object sender, RoutedEventArgs e)
      {
         var w = new WindowCredits();
         w.ShowDialog();
      }

      private void ButtonSave(object sender, RoutedEventArgs e)
      {
         try
         {
            var file = FileTools.GetFileConfiguration(@"Please select a file to save the current configuration", true, null);
            if (file != null)
               ModelConfiguration.Save(file);
         }
         catch (Exception ex)
         {
            MessageTools.ShowError(ex.Message);
         }
      }
      private void ButtonOpen(object sender, RoutedEventArgs e)
      {
         try
         {
            var file = FileTools.GetFileConfiguration(@"Please select a configuration file to open", false, null);
            if (file != null)
               ModelConfiguration.Open(file);
         }
         catch (Exception ex)
         {
            MessageTools.ShowError(ex.Message);
         }
      }

      private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
      {
         Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
         e.Handled = true;
      }

      private void SearchTexDiag(object sender, RoutedEventArgs e)
      {
         var path = FileTools.GetExe("texdiag.exe", ModelConfiguration.Main.ModelPath.PathTexdiag);
         if (path != null)
            ModelConfiguration.Main.ModelPath.PathTexdiag = path;
      }

      private void SearchTexConv(object sender, RoutedEventArgs e)
      {
         var path = FileTools.GetExe("texconv.exe", ModelConfiguration.Main.ModelPath.PathTexconv);
         if (path != null)
            ModelConfiguration.Main.ModelPath.PathTexconv = path;
      }

      private void SearchBsarch(object sender, RoutedEventArgs e)
      {
         var path = FileTools.GetExe("bsarch.exe", ModelConfiguration.Main.ModelPath.PathBsarch);
         if (path != null)
            ModelConfiguration.Main.ModelPath.PathBsarch = path;
      }

      private void SearchGmic(object sender, RoutedEventArgs e)
      {
         var path = FileTools.GetExe("gmic.exe", ModelConfiguration.Main.ModelPath.PathGmic);
         if (path != null)
            ModelConfiguration.Main.ModelPath.PathGmic = path;
      }

      private void SearchCustomTool(object sender, RoutedEventArgs e)
      {
         var path = FileTools.GetExe("*.exe", ModelConfiguration.Main.ModelPath.PathCustomTool);
         if (path != null)
            ModelConfiguration.Main.ModelPath.PathCustomTool = path;
      }

      private void SetModPriorityFile(object sender, RoutedEventArgs e)
      {
         var path = FileTools.GetFileAny("Choose MO2 priority file", false, ModelConfiguration.Main.PathMergePriorityFile);
         if (path != null)
            ModelConfiguration.Main.PathMergePriorityFile = path;
      }


      private void ShowFilters(object sender, RoutedEventArgs e)
      {
         Button button = sender as Button;
         if (button != null)
         {
            ModelConfigurationPass pass = button.DataContext as ModelConfigurationPass;
            var selection = pass.ModelSelection;
            //
            var w = new WindowFilter { ModelConfigurationSelection = selection };
            w.DataContext = w.ModelConfigurationSelection;
            w.ShowDialog();
         }
      }

      private void ButtonStart(object sender, RoutedEventArgs e)
      {
         var w = new WindowProcessor();
         w.ModelProcessor = new ModelProcessor(new Processor(new ProcessorArgs(ModelConfiguration.Main.Main)), w.ListViewLog);
         w.DataContext = w.ModelProcessor;
         w.ShowDialog();
      }
   }
}
