#region Dependencies

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OctagonCommon.Statics;

#endregion

namespace OctagonCommon.Configurations
{
   public class ConfigurationSelection
   {
      public ConfigurationSelection()
      {
         Filters = new List<ConfigurationSelectionPass>();
         StartFileValidation = true;
      }

      public List<ConfigurationSelectionPass> Filters { get; set; }

      public bool StartFileValidation { get; set; }

      public ConfigurationSelectionPass AddFilter(TypeSelection typeSelection, string pattern, int occurence, bool isCheckFullPath)
      {
         var newElement = new ConfigurationSelectionPass(typeSelection, pattern, occurence, isCheckFullPath);
         Filters.Add(newElement);
         return newElement;
      }

      public void RemoveFilter(ConfigurationSelectionPass configurationSelectionPass)
      {
         Filters.Remove(configurationSelectionPass);
      }

      public void CalculateStartFileValidation()
      {
         StartFileValidation = Filters.All(e => e.IsStartValidated());
         //StartFileValidation &= Filters.All(e => e.IsStartValidated()); 
      }

      public bool GetValidation(string name, string fullPath)
      {
         bool fileValidation = StartFileValidation;
         // Assert pattern validation
         foreach (ConfigurationSelectionPass filter in Filters)
         {
            fileValidation = filter.Validate(fileValidation, name, fullPath);
         }
         //
         return fileValidation;
      }

      public void Write(string fileName)
      {
         using (StreamWriter file = File.CreateText(fileName))
         {
            Persistence.Serializer.Serialize(file, Filters);
         }
      }

      public void Read(string fileName)
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
               Filters = Persistence.Serializer.Deserialize(file, typeof(List<ConfigurationSelectionPass>)) as List<ConfigurationSelectionPass>;
            }
         }
         catch (Exception e)
         {
            throw new FileLoadException(string.Format("{0} is not a valid filters file", fileName), e);
         }
      }
   }
}