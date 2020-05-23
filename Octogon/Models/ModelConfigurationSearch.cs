using System.Windows;
using OctagonCommon.Configurations;

namespace Octagon.Models
{
   public class ModelConfigurationSearch   :AModel
   {
      public ConfigurationSearch Search { get; private set; }

      public ModelConfigurationSearch(ConfigurationSearch search)
      {
         Search = search;
      }

      public bool IsSearchEnabled
      {
         get { return Search.IsSearchEnabled; }
         set
         {
            if (Search.IsSearchEnabled != value)
            {
               Search.IsSearchEnabled = value;
               OnPropertyChanged("IsSearchEnabled");
               OnPropertyChanged("VisibilitySearch");
            }
         }
      }

      public Visibility VisibilitySearch { get { return IsSearchEnabled ? Visibility.Visible : Visibility.Collapsed; } }

      public bool IsApplySearchToProcess
      {
         get { return Search.IsApplySearchToProcess; }
         set
         {
            if (Search.IsApplySearchToProcess != value)
            {
               Search.IsApplySearchToProcess = value;
               OnPropertyChanged("IsApplySearchToProcess");
            }
         }
      }

      public bool IsSearchNameEnabled
      {
         get { return Search.IsSearchNameEnabled; }
         set
         {
            if (Search.IsSearchNameEnabled != value)
            {
               Search.IsSearchNameEnabled = value;
               OnPropertyChanged("IsSearchNameEnabled");
            }
         }
      }

      public bool IsSearchMinSizeEnabled
      {
         get { return Search.IsSearchMinSizeEnabled; }
         set
         {
            if (Search.IsSearchMinSizeEnabled != value)
            {
               Search.IsSearchMinSizeEnabled = value;
               OnPropertyChanged("IsSearchMinSizeEnabled");
            }
         }
      }

      public bool IsSearchMaxSizeEnabled
      {
         get { return Search.IsSearchMaxSizeEnabled; }
         set
         {
            if (Search.IsSearchMaxSizeEnabled != value)
            {
               Search.IsSearchMaxSizeEnabled = value;
               OnPropertyChanged("IsSearchMaxSizeEnabled");
            }
         }
      }

      public bool IsSearchFormatEnabled
      {
         get { return Search.IsSearchFormatEnabled; }
         set
         {
            if (Search.IsSearchFormatEnabled != value)
            {
               Search.IsSearchFormatEnabled = value;
               OnPropertyChanged("IsSearchFormatEnabled");
            }
         }
      }

      public bool IsSearchMipmapsEnabled
      {
         get { return Search.IsSearchMipmapsEnabled; }
         set
         {
            if (Search.IsSearchMipmapsEnabled != value)
            {
               Search.IsSearchMipmapsEnabled = value;
               OnPropertyChanged("IsSearchMipmapsEnabled");
            }
         }
      }

      public bool IsSearchPowerOf2
      {
         get { return Search.IsSearchPowerOf2; }
         set
         {
            if (Search.IsSearchPowerOf2 != value)
            {
               Search.IsSearchPowerOf2 = value;
               OnPropertyChanged("IsSearchPowerOf2");
            }
         }
      }

      public bool IsMipmaps
      {
         get { return Search.IsMipmaps; }
         set
         {
            if (Search.IsMipmaps != value)
            {
               Search.IsMipmaps = value;
               OnPropertyChanged("IsMipmaps");
            }
         }
      }

      public bool IsPowerOf2
      {
         get { return Search.IsPowerOf2; }
         set
         {
            if (Search.IsPowerOf2 != value)
            {
               Search.IsPowerOf2 = value;
               OnPropertyChanged("IsPowerOf2");
            }
         }
      }

      public int MinSize
      {
         get { return Search.MinSize; }
         set
         {
            if (Search.MinSize != value)
            {
               Search.MinSize = value;
               OnPropertyChanged("MinSize");
            }
         }
      }

      public int MaxSize
      {
         get { return Search.MaxSize; }
         set
         {
            if (Search.MaxSize != value)
            {
               Search.MaxSize = value;
               OnPropertyChanged("MaxSize");
            }
         }
      }

      public string Name
      {
         get { return Search.Name; }
         set
         {
            if (Search.Name != value)
            {
               Search.Name = value;
               OnPropertyChanged("Name");
            }
         }
      }

      public string Format
      {
         get { return Search.Format; }
         set
         {
            if (Search.Format != value)
            {
               Search.Format = value;
               OnPropertyChanged("Format");
            }
         }
      }
          
   }
}