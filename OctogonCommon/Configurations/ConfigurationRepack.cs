using System.Collections.Generic;
using System.Linq;

namespace OctagonCommon.Configurations
{
   public class ConfigurationRepack
   {
      public ConfigurationRepack()
      {
         SourceNames = new List<ConfigurationString>();
      }

      public ConfigurationRepack(string bsaName, string gameParameter, bool isCompressed, long size, params string[] sources)
      {
         BsaName = bsaName;
         IsCompressed = isCompressed;
         Size = size;
         GameParameter = gameParameter;
         SourceNames = sources.Select(e => new ConfigurationString(e)).ToList();
      }

      public List<ConfigurationString> SourceNames { get; set; }

      public string BsaName { get; set; }

      public string GameParameter { get; set; }

      public bool IsCompressed { get; set; }

      public long Size { get; set; }

      public ConfigurationString AddString()
      {
         var n = new ConfigurationString();
         SourceNames.Add(n);
         return n;
      }

      public void RemoveString(ConfigurationString configurationString)
      {
         SourceNames.Remove(configurationString);
      }
   }
}