using OctagonCommon.Configurations;

namespace Octagon.Models
{
   public class ModelConfigurationString : AModel
   {
      public ConfigurationString ConfigurationString { get; private set; }
      public ModelConfigurationString(ConfigurationString configurationString)
      {
         ConfigurationString = configurationString;                               
      }

      public string Value
      {
         get { return ConfigurationString.Value; }
         set
         {
            if (ConfigurationString.Value != value)
            {
               ConfigurationString.Value = value;
               OnPropertyChanged("Value");
            }
         }
      }
                 
   }
}