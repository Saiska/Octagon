namespace OctagonCommon.Configurations
{
   public class ConfigurationString
   {
      public ConfigurationString()
      {
         Value = string.Empty;
      }

      public ConfigurationString(string value)
      {
         Value = value;
      }

      public string Value { get; set; }
   }
}