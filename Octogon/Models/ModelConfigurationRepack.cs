using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Octagon.Tools;
using OctagonCommon.Configurations;

namespace Octagon.Models
{
   public class ModelConfigurationRepack : AModel
   {
      public ConfigurationRepack Repack { get; private set; }
      public ObservableCollection<ModelConfigurationString> SourceNames { get; private set; }

      public ModelConfigurationRepack(ConfigurationRepack repack)
      {
         Repack = repack;
         SourceNames = new ObservableCollection<ModelConfigurationString>(repack.SourceNames.Select(e => new ModelConfigurationString(e)));
         DeleteItemCommand = new DelegateCommand(DeleteString);
         AddItemCommand = new DelegateCommand(AddString);
      }

      private void DeleteString(object obj)
      {
         var repack = obj as ModelConfigurationString;
         if (repack != null)
         {
            RemoveString(repack);
         }
      }

      public void AddString(object obj)
      {
         var newElement = Repack.AddString();
         SourceNames.Add(new ModelConfigurationString(newElement));
      }

      public void RemoveString()
      {
         if (StringSelected != null)
         {
            RemoveString(StringSelected);
            StringSelected = null;
         }
      }

      public void RemoveString(ModelConfigurationString  modelConfigurationString)
      {
         Repack.RemoveString(modelConfigurationString.ConfigurationString);
         SourceNames.Remove(modelConfigurationString);
      }

      private ModelConfigurationString _stringSelected;
      public ModelConfigurationString StringSelected
      {
         get { return _stringSelected; }
         set
         {
            if (_stringSelected != value)
            {
               _stringSelected = value;
               OnPropertyChanged("StringSelected");
            }
         }
      }

      public string BsaName
      {
         get { return Repack.BsaName; }
         set
         {
            if (Repack.BsaName != value)
            {
               Repack.BsaName = value;
               OnPropertyChanged("BsaName");
            }
         }
      }

      public bool IsCompressed
      {
         get { return Repack.IsCompressed; }
         set
         {
            if (Repack.IsCompressed != value)
            {
               Repack.IsCompressed = value;
               OnPropertyChanged("IsCompressed");
            }
         }
      }

      public long Size
      {
         get { return Repack.Size; }
         set
         {
            if (Repack.Size != value)
            {
               Repack.Size = value;
               OnPropertyChanged("Size");
               OnPropertyChanged("SizeText");
            }
         }
      }

      public string SizeText
      {
         get { return string.Format("{0} Mo", (int) (Size / 1000000d)); }
      }

      public ICommand DeleteItemCommand { get; set; }
      public ICommand AddItemCommand { get; set; }
   }
}