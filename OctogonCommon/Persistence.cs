using System;
using System.IO;
using Newtonsoft.Json;
using OctagonCommon.Configurations;

namespace OctagonCommon
{
   public class Persistence
   {
      private const string ConfigurationFile = "lastconfig.json";
      private const string ConfigurationFileOctagonExt = "lastconfig.oconfig";

      public static JsonSerializer Serializer { get; set; }

      static Persistence()    
      {
         Serializer = new JsonSerializer { Formatting = Formatting.Indented };
      }

      public ConfigurationMain Read()
      {
         if (File.Exists(ConfigurationFileOctagonExt))
         {
            return Read(ConfigurationFileOctagonExt);
         }
         return Read(ConfigurationFile);
      }

      public ConfigurationMain Read(string fileName)
      {
         try
         {
            if (!File.Exists(fileName))
            {
               throw new FileLoadException(string.Format("{0} don't exist", fileName));
            }
            //
            using (StreamReader file = File.OpenText(fileName))
            {
               return Serializer.Deserialize(file, typeof(ConfigurationMain)) as ConfigurationMain;
            }
         }
         catch (Exception e)
         {
            throw new FileLoadException(string.Format("{0} is not a valid configuration file", fileName), e);
         }
      }

      public void Write(ConfigurationMain configuration, string fileName = ConfigurationFileOctagonExt)
      {
         using (StreamWriter file = File.CreateText(fileName))
         {
            Serializer.Serialize(file, configuration);
         }
      }
   }

}