#region Dependencies

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace OctagonCommon.Configurations
{
   public class ConfigurationMain : IConfigurationSelection
   {
      public ConfigurationMain()
      {
         IsCleanBackup = false;
         IsNoConfirmationMessage = false;
         IsRecopyOriginal = false;
         IsRefreshBackup = true;
         IsOnlyNewFromArchive = false;
         IsShowResults = false;
         IsUseMultithreading = true;
         IsUseBackup = false;
         IsVerbose = false;
         IsBackupActivated = false;
         IsMergeActivated = false;
         //
         PathSource = string.Empty;
         PathBackup = string.Empty;
         PathMergeDirectory = string.Empty;
         PathMergePriorityFile = string.Empty;
         //
         Passes = new List<ConfigurationPass>();
         //
         Selection = new ConfigurationSelection();
         Paths = new ConfigurationPath();
         //
         PassBsa = new ConfigurationPassBsa();      
         //
         Search = new ConfigurationSearch();
      }

      public ConfigurationPath Paths { get; set; }

      public ConfigurationSearch Search { get; set; }

      public ConfigurationSelection Selection { get; set; }

      public bool IsNoConfirmationMessage { get; set; }
      public bool IsShowResults { get; set; }
      public bool IsUseMultithreading { get; set; }
      public bool IsVerbose { get; set; }

      public bool IsBackupActivated { get; set; }
      public bool IsUseBackup { get; set; }
      public bool IsOnlyNewFromArchive { get; set; }
      public bool IsRecopyOriginal { get; set; }
      public bool IsRefreshBackup { get; set; }
      public bool IsCleanBackup { get; set; }

      public ConfigurationPassBsa PassBsa { get; set; }      

      public List<ConfigurationPass> Passes { get; set; }

      public string PathBackup { get; set; }
      public string PathSource { get; set; }

      public bool IsMergeActivated { get; set; }
      public bool IsMergeAssertCase { get; set; }
      public bool IsMergeDeleteIfNotInSource { get; set; }
      public bool IsUnmergeActivated { get; set; }
      public string PathMergeDirectory { get; set; }
      public string PathMergePriorityFile { get; set; }

      public bool HasTextureOperation()
      {
         return Search.IsSearchEnabled || Passes.Any();
      }
   }
}