using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Octagon.Tools
{
   public class FileTools
   {
      public static string GetFileFilters(string title, bool save, string defaultPath)
      {
         return GetFile(title, save, @".ofilters", @"Octagon filters files (*.ofilters)|*.ofilters|json files (*.json)|*.json", defaultPath);
      }

      public static string GetFileConfiguration(string title, bool save, string defaultPath)
      {
         return GetFile(title, save, @".oconfig", @"Octagon configuration files (*.oconfig)|*.oconfig|json files (*.json)|*.json", defaultPath);
      }

      public static string GetFileAny(string title, bool save, string defaultPath)
      {
         return GetFile(title, save, @".*", @"Any files (*.*)|*.*", defaultPath);
      }

      public static string GetExe(string fileName, string defaultPath)
      {                                               
         return GetFile("Get file", false, null, string.Format(@"{0}|{0}", fileName), defaultPath);
      }

      private static string GetFile(string title, bool save, string ext, string filter, string defaultPath)
      {
         FileDialog fileDialog;
         //
         if (save)
         {
            fileDialog = new SaveFileDialog();
         }
         else
         {
            fileDialog = new OpenFileDialog();
         }
         //
         fileDialog.DefaultExt = ext;
         fileDialog.Filter = filter;
         fileDialog.AddExtension = true;
         fileDialog.Title = title;
         //
         string initialDirectory = null;
         if (!string.IsNullOrEmpty(defaultPath))
         {
              var file = new FileInfo(defaultPath);
            if (file.Exists && file.Directory != null)
            {
               initialDirectory = file.Directory.FullName;
            }
            else
            {
               var dir = new DirectoryInfo(defaultPath);
               if (dir.Exists)
               {
                  initialDirectory = dir.FullName;
               }
            }
         }
         //
         if (string.IsNullOrEmpty(initialDirectory))
         {
            DirectoryInfo dir = new FileInfo(Application.ExecutablePath).Directory;
            if (dir != null)
            {
               initialDirectory = dir.FullName;
            }
            else
            {
               initialDirectory = Directory.GetDirectoryRoot(Application.ExecutablePath);
            }
         }
         //
         fileDialog.InitialDirectory = initialDirectory;
         //
         if (fileDialog.ShowDialog() == DialogResult.OK)
         {
            return fileDialog.FileName;
         }
         return null;
      }
   }
}
