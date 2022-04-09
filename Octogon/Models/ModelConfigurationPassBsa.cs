using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using OctagonCommon.Configurations;
using Octagon.Tools;

namespace Octagon.Models
{
   public class ModelConfigurationPassBsa : AModel, IModelConfigurationSelection
   {
      public ModelConfigurationSelection ModelSelection { get; set; }

      public ObservableCollection<ModelConfigurationRepack> ModelRepacks { get; private set; }

      public ConfigurationPassBsa PassBsa { get; private set; }

      public ModelConfigurationPassBsa(ConfigurationPassBsa passBsa)
      {
         PassBsa = passBsa;
         ModelSelection = new ModelConfigurationSelection(passBsa.Selection);
         ModelRepacks = new ObservableCollection<ModelConfigurationRepack>(passBsa.Repacks.Select(e => new ModelConfigurationRepack(e)));
         DeleteItemCommand = new DelegateCommand(DeleteRepack);
      }

      private void DeleteRepack(object obj)
      {
         var repack = obj as ModelConfigurationRepack;
         if (repack != null)
         {
            RemoveRepack(repack);     
         }
      }

      public void DefaultRepack()
      {
         PassBsa.SetDefaultRepacks();
         ModelRepacks.Clear();
         foreach (ConfigurationRepack configurationRepack in PassBsa.Repacks)
         {
            ModelRepacks.Add(new ModelConfigurationRepack(configurationRepack));
         }
      }

      public void AddRepack()
      {
         var newElement = PassBsa.AddRepack();
         ModelRepacks.Add(new ModelConfigurationRepack(newElement));
      }

      public void RemoveRepack()
      {
         if (RepackSelected != null)
         {
            RemoveRepack(RepackSelected);
            RepackSelected = null;
         }
      }

      public void RemoveRepack(ModelConfigurationRepack modelConfigurationRepack)
      {
            PassBsa.RemoveRepack(modelConfigurationRepack.Repack);
            ModelRepacks.Remove(modelConfigurationRepack);      
      }

      private ModelConfigurationRepack _repackSelected;
      public ModelConfigurationRepack RepackSelected
      {
         get { return _repackSelected; }
         set
         {
            if (_repackSelected != value)
            {
               _repackSelected = value;
               OnPropertyChanged("RepackSelected");
            }
         }
      }

      public string GameParameter
      {
         get { return PassBsa.GameParameter; }
         set
         {
            if (PassBsa.GameParameter != value)
            {
               PassBsa.GameParameter = value;
               OnPropertyChanged("GameParameter");
            }
         }
      }

      public bool Enabled
      {
         get { return PassBsa.Enabled; }
         set
         {
            if (PassBsa.Enabled != value)
            {
               PassBsa.Enabled = value;
               OnPropertyChanged("Enabled");
               OnPropertyChanged("VisibilityEnabled");
            }
         }
      }
      public Visibility VisibilityEnabled { get { return Enabled ? Visibility.Visible : Visibility.Collapsed; } }

      public bool IsClean
      {
         get { return PassBsa.IsClean; }
         set
         {
            if (PassBsa.IsClean != value)
            {
               PassBsa.IsClean = value;
               OnPropertyChanged("IsClean");
            }
         }
      }

      public bool IsRepack
      {
         get { return PassBsa.IsRepack; }
         set
         {
            if (PassBsa.IsRepack != value)
            {
               PassBsa.IsRepack = value;
               OnPropertyChanged("IsRepack");
            }
         }
      }

      public bool IsRepackOnlyIfMissingBsa
      {
         get { return PassBsa.IsRepackOnlyIfMissingBsa; }
         set
         {
            if (PassBsa.IsRepackOnlyIfMissingBsa != value)
            {
               PassBsa.IsRepackOnlyIfMissingBsa = value;
               OnPropertyChanged("IsRepackOnlyIfMissingBsa");
            }
         }
      }

      public bool IsUnpack
      {
         get { return PassBsa.IsUnpack; }
         set
         {
            if (PassBsa.IsUnpack != value)
            {
               PassBsa.IsUnpack = value;
               OnPropertyChanged("IsUnpack");
            }
         }
      }

      public bool IsCheckFormatIsGameFormat
      {
         get { return PassBsa.IsCheckFormatIsGameFormat; }
         set
         {
            if (PassBsa.IsCheckFormatIsGameFormat != value)
            {
               PassBsa.IsCheckFormatIsGameFormat = value;
               OnPropertyChanged("IsCheckFormatIsGameFormat");
            }
         }
      }


      public bool IsCopyAsLoose
      {
         get { return PassBsa.IsCopyAsLoose; }
         set
         {
            if (PassBsa.IsCopyAsLoose != value)
            {
               PassBsa.IsCopyAsLoose = value;
               OnPropertyChanged("IsCopyAsLoose");
               OnPropertyChanged("VisibilityIsCopyAsLoose");
            }
         }
      }
      public bool IsCopyAsLooseIfDummy
      {
         get { return PassBsa.IsCopyAsLooseIfDummy; }
         set
         {
            if (PassBsa.IsCopyAsLooseIfDummy != value)
            {
               PassBsa.IsCopyAsLooseIfDummy = value;
               OnPropertyChanged("IsCopyAsLooseIfDummy");        
            }
         }
      }

      public bool IsTreatNonTextureArchives
      {
         get { return PassBsa.IsTreatNonTextureArchives; }
         set
         {
            if (PassBsa.IsTreatNonTextureArchives != value)
            {
               PassBsa.IsTreatNonTextureArchives = value;
               OnPropertyChanged("IsTreatNonTextureArchives");
            }
         }
      }

      public bool IsRepackLooseFilesInBsa
      {
         get { return PassBsa.IsRepackLooseFilesInBsa; }
         set
         {
            if (PassBsa.IsRepackLooseFilesInBsa != value)
            {
               PassBsa.IsRepackLooseFilesInBsa = value;
               OnPropertyChanged("IsRepackLooseFilesInBsa");
               OnPropertyChanged("VisibilityRepackLooseFilesInBsa");
               OnPropertyChanged("VisibilityRepackIntelligent");
               OnPropertyChanged("VisibilityRepackNotIntelligent");
            }
         }
      }
      public bool IsIntelligentPacking
      {
         get { return PassBsa.IsIntelligentPacking; }
         set
         {
            if (PassBsa.IsIntelligentPacking != value)
            {
               PassBsa.IsIntelligentPacking = value;
               OnPropertyChanged("IsIntelligentPacking");                
               OnPropertyChanged("VisibilityRepackIntelligent");
               OnPropertyChanged("VisibilityRepackNotIntelligent");
            }
         }
      }
      public bool IsRepackCreateDummy
      {
         get { return PassBsa.IsRepackCreateDummy; }
         set
         {
            if (PassBsa.IsRepackCreateDummy != value)
            {
               PassBsa.IsRepackCreateDummy = value;
               OnPropertyChanged("IsRepackCreateDummy");           
            }
         }
      }
      public Visibility VisibilityIsCopyAsLoose { get { return IsCopyAsLoose ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityRepackLooseFilesInBsa { get { return IsRepackLooseFilesInBsa   ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityRepackIntelligent { get { return IsIntelligentPacking ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityRepackNotIntelligent { get { return IsIntelligentPacking ? Visibility.Collapsed : Visibility.Visible; } }


      public ICommand DeleteItemCommand { get; set; }
   }
}
