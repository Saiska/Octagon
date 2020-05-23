using System;
using OctagonCommon;
using OctagonCommon.Configurations;

namespace Octagon.Models
{
   public class ModelConfiguration : AModel
   {
      public ModelConfiguration()
      {
         Persistence = new Persistence();
         //
         try
         {
            Main = new ModelConfigurationMain(Persistence.Read());
         }
         catch (Exception)
         {
            var newCfg = new ConfigurationMain();
            newCfg.PassBsa.SetDefaultRepacks();   
            Main = new ModelConfigurationMain(newCfg);
         }
         //Main = new ModelConfigurationMain(Persistence.Read() ?? new ConfigurationMain());   
      }

      public ModelConfigurationMain Main { get; set; }

      public Persistence Persistence { get; set; }

      public void Save()
      {
         Persistence.Write(Main.Main);
      }

      public void Save(string fileName)
      {
         Persistence.Write(Main.Main, fileName);
      }

      public void Open(string fileName)
      {
         try
         {
            var newCfg = Persistence.Read(fileName);
            if (newCfg != null)
            {
               Main = new ModelConfigurationMain(newCfg);
               OnPropertyChanged("Main");
            }
         }
         catch (Exception)
         {
            throw;
         }
      }

      public bool IsLogInFile
      {
         get { return Logger.Instance.FileLoggingEnabled; }
         set
         {
            if (Logger.Instance.FileLoggingEnabled != value)
            {
               Logger.Instance.FileLoggingEnabled = value;
               OnPropertyChanged("IsLogInFile");
            }
         }
      }

   }
}
