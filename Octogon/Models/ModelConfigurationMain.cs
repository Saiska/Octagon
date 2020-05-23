using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Octagon.Tools;
using OctagonCommon.Configurations;

namespace Octagon.Models
{
   public class ModelConfigurationMain : AModel, IModelConfigurationSelection
   {
      public ModelConfigurationSelection ModelSelection { get; set; }
      public ModelConfigurationPath ModelPath { get; set; }
      public ModelConfigurationSearch ModelSearch{ get; set; }
      public ModelConfigurationPassBsa ModelPassBsa { get; set; }
      public ObservableCollection<ModelConfigurationPass> ModelPasses { get; private set; }

      public ConfigurationMain Main { get; set; }

      public ModelConfigurationMain(ConfigurationMain main)
      {
         Main = main;
         ModelSelection = new ModelConfigurationSelection(main.Selection);
         ModelPath = new ModelConfigurationPath(main.Paths);
         ModelPassBsa = new ModelConfigurationPassBsa(main.PassBsa);
         ModelSearch = new ModelConfigurationSearch(main.Search);
         ModelPasses = new ObservableCollection<ModelConfigurationPass>(main.Passes.Select(e => new ModelConfigurationPass(e)));

         DeleteItemCommand = new DelegateCommand(RemovePass);
      }

      public DelegateCommand DeleteItemCommand { get; set; }


      public void AddFormatPass()
      {
         var pass = ConfigurationPass.GetNewForceFormat("BC7_UNORM");
         Main.Passes.Add(pass);
         ModelPasses.Add(new ModelConfigurationPass(pass));
      }

      public void AddDividePass()
      {
         var pass = ConfigurationPass.GetNewDivider(2);
         Main.Passes.Add(pass);
         ModelPasses.Add(new ModelConfigurationPass(pass));
      }

      public void AddFixedPass()
      {
         var pass = ConfigurationPass.GetNewFixer(1024);
         Main.Passes.Add(pass);
         ModelPasses.Add(new ModelConfigurationPass(pass));
      }

      public void AddUpscaleFactor()
      {
         var pass = ConfigurationPass.GetUpscaleFactor(2);
         Main.Passes.Add(pass);
         ModelPasses.Add(new ModelConfigurationPass(pass));
      }

      public void AddUpscaleFixed()
      {
         var pass = ConfigurationPass.GetUpscaleFixed(4096);
         Main.Passes.Add(pass);
         ModelPasses.Add(new ModelConfigurationPass(pass));
      }

      public void AddCorrectSize()
      {
         var pass = ConfigurationPass.GetCorrectSize();
         Main.Passes.Add(pass);
         ModelPasses.Add(new ModelConfigurationPass(pass));
      }

      public void AddGmic()
      {
         var pass = ConfigurationPass.GetGmic();
         Main.Passes.Add(pass);
         ModelPasses.Add(new ModelConfigurationPass(pass));
      }


      public void AddForce()
      {
         var pass = ConfigurationPass.GetForce();
         Main.Passes.Add(pass);
         ModelPasses.Add(new ModelConfigurationPass(pass));
      }

      public void AddCustom()
      {
         var pass = ConfigurationPass.GetCustom();
         Main.Passes.Add(pass);
         ModelPasses.Add(new ModelConfigurationPass(pass));
      }

      public void AddForceMipmap()
      {
         var pass = ConfigurationPass.GetForceMipmap();
         Main.Passes.Add(pass);
         ModelPasses.Add(new ModelConfigurationPass(pass));
      }

      public void AddCorrectMipmap()
      {
         var pass = ConfigurationPass.GetCorrectMipmap();
         Main.Passes.Add(pass);
         ModelPasses.Add(new ModelConfigurationPass(pass));
      }

      public void RemovePass(object obj)
      {
         var passSelected = obj as ModelConfigurationPass;
         if (passSelected != null)
         {
            Main.Passes.Remove(passSelected.Pass);
            ModelPasses.Remove(passSelected);         
         }
      }          

      public string PathBackup
      {
         get { return Main.PathBackup; }
         set
         {
            if (Main.PathBackup != value)
            {
               Main.PathBackup = value;
               OnPropertyChanged("PathBackup");
            }
         }
      }

      public string PathSource
      {
         get { return Main.PathSource; }
         set
         {
            if (Main.PathSource != value)
            {
               Main.PathSource = value;
               OnPropertyChanged("PathSource");
            }
         }
      }
         
      public string PathMergeDirectory
      {
         get { return Main.PathMergeDirectory; }
         set
         {
            if (Main.PathMergeDirectory != value)
            {
               Main.PathMergeDirectory = value;
               OnPropertyChanged("PathMergeDirectory");
            }
         }
      }

      public string PathMergePriorityFile
      {
         get { return Main.PathMergePriorityFile; }
         set
         {
            if (Main.PathMergePriorityFile != value)
            {
               Main.PathMergePriorityFile = value;
               OnPropertyChanged("PathMergePriorityFile");
            }
         }
      }

      public bool IsBackupActivated
      {
         get { return Main.IsBackupActivated; }
         set
         {
            if (Main.IsBackupActivated != value)
            {
               Main.IsBackupActivated = value;
               OnPropertyChanged("IsBackupActivated");
               OnPropertyChanged("VisibilityBackup");
            }
         }
      }
      public Visibility VisibilityBackup { get { return IsBackupActivated ? Visibility.Visible : Visibility.Collapsed; } }

      public bool IsUnmergeActivated
      {
         get { return Main.IsUnmergeActivated; }
         set
         {
            if (Main.IsUnmergeActivated != value)
            {
               Main.IsUnmergeActivated = value;
               OnPropertyChanged("IsUnmergeActivated");   
            }
         }
      }
      public bool IsMergeDeleteIfNotInSource
      {
         get { return Main.IsMergeDeleteIfNotInSource; }
         set
         {
            if (Main.IsMergeDeleteIfNotInSource != value)
            {
               Main.IsMergeDeleteIfNotInSource = value;
               OnPropertyChanged("IsMergeDeleteIfNotInSource");   
            }
         }
      }
      public bool IsMergeAssertCase
      {
         get { return Main.IsMergeAssertCase; }
         set
         {
            if (Main.IsMergeAssertCase != value)
            {
               Main.IsMergeAssertCase = value;
               OnPropertyChanged("IsMergeAssertCase");   
            }
         }
      }
      public bool IsMergeActivated
      {
         get { return Main.IsMergeActivated; }
         set
         {
            if (Main.IsMergeActivated != value)
            {
               Main.IsMergeActivated = value;
               OnPropertyChanged("IsMergeActivated");
               OnPropertyChanged("VisibilityMerge");
            }
         }
      }
      public Visibility VisibilityMerge { get { return IsMergeActivated ? Visibility.Visible : Visibility.Collapsed; } }

   

      public bool IsVerbose
      {
         get { return Main.IsVerbose; }
         set
         {
            if (Main.IsVerbose != value)
            {
               Main.IsVerbose = value;
               OnPropertyChanged("IsVerbose");
            }
         }
      }

      public bool IsCleanBackup
      {
         get { return Main.IsCleanBackup; }
         set
         {
            if (Main.IsCleanBackup != value)
            {
               Main.IsCleanBackup = value;
               OnPropertyChanged("IsCleanBackup");
            }
         }
      }

      public bool IsNoConfirmationMessage
      {
         get { return Main.IsNoConfirmationMessage; }
         set
         {
            if (Main.IsNoConfirmationMessage != value)
            {
               Main.IsNoConfirmationMessage = value;
               OnPropertyChanged("IsNoConfirmationMessage");
            }
         }
      }

      public bool IsOnlyNewFromArchive
      {
         get { return Main.IsOnlyNewFromArchive; }
         set
         {
            if (Main.IsOnlyNewFromArchive != value)
            {
               Main.IsOnlyNewFromArchive = value;
               OnPropertyChanged("IsOnlyNewFromArchive");
            }
         }
      }

      public bool IsRecopyOriginal
      {
         get { return Main.IsRecopyOriginal; }
         set
         {
            if (Main.IsRecopyOriginal != value)
            {
               Main.IsRecopyOriginal = value;
               OnPropertyChanged("IsRecopyOriginal");
            }
         }
      }

      public bool IsRefreshBackup
      {
         get { return Main.IsRefreshBackup; }
         set
         {
            if (Main.IsRefreshBackup != value)
            {
               Main.IsRefreshBackup = value;
               OnPropertyChanged("IsRefreshBackup");
            }
         }
      }

      public bool IsShowResults
      {
         get { return Main.IsShowResults; }
         set
         {
            if (Main.IsShowResults != value)
            {
               Main.IsShowResults = value;
               OnPropertyChanged("IsShowResults");
            }
         }
      }

      public bool IsUseBackup
      {
         get { return Main.IsUseBackup; }
         set
         {
            if (Main.IsUseBackup != value)
            {
               Main.IsUseBackup = value;
               OnPropertyChanged("IsUseBackup");
            }
         }
      }

      public bool IsUseMultithreading
      {
         get { return Main.IsUseMultithreading; }
         set
         {
            if (Main.IsUseMultithreading != value)
            {
               Main.IsUseMultithreading = value;
               OnPropertyChanged("IsUseMultithreading");
            }
         }
      }

   }
}
