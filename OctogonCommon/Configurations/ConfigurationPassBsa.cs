using System.Collections.Generic;

namespace OctagonCommon.Configurations
{
   public class ConfigurationPassBsa : IConfigurationSelection
   {
      public ConfigurationPassBsa()
      {
         Selection = new ConfigurationSelection();
         //set default bsarch tool game as skyrim special edition
         GameParameter = "sse";
         //
         IsUnpack = true;
         IsRepack = true;
         IsClean = true;
         //
         Repacks = new List<ConfigurationRepack>();
      }

      public bool Enabled { get; set; }

      public string GameParameter { get; set; }

      public bool IsClean { get; set; }
      public bool IsRepack { get; set; }
      public bool IsRepackOnlyIfMissingBsa { get; set; }
      public bool IsUnpack { get; set; }
      public bool IsCheckFormatIsGameFormat { get; set; }

      public bool IsCopyAsLoose { get; set; }
      public bool IsTreatNonTextureArchives { get; set; }
      public bool IsCopyAsLooseIfDummy { get; set; }

      public bool IsRepackLooseFilesInBsa { get; set; }
      public bool IsIntelligentPacking { get; set; }
      public bool IsRepackCreateDummy { get; set; }

      public List<ConfigurationRepack> Repacks { get; set; }

      public ConfigurationSelection Selection { get; set; }

      public bool MustClean()
      {
         return IsClean && !IsCopyAsLoose;
      }

      public bool MustRepack()
      {
         return IsRepack && !IsCopyAsLoose;
      }

      public bool MustUnpack()
      {
         return IsUnpack && !IsCopyAsLoose;
      }

      public ConfigurationRepack AddRepack()
      {
         var newElement = new ConfigurationRepack();
         Repacks.Add(newElement);
         return newElement;
      }

      public void RemoveRepack(ConfigurationRepack configurationRepack)
      {
         Repacks.Remove(configurationRepack);
      }

      public void SetDefaultRepacks()
      {
         const long maxSize = 2400000000;
         Repacks.Clear();
         Repacks.Add(new ConfigurationRepack("OctagonMeshes$.bsa", GameParameter, true, maxSize, "Meshes"));
         Repacks.Add(new ConfigurationRepack("OctagonTextures$.bsa", GameParameter, true, maxSize, "Textures"));
         Repacks.Add(new ConfigurationRepack("OctagonInterface$.bsa", GameParameter, true, maxSize, "Interface"));
         Repacks.Add(new ConfigurationRepack("OctagonShaders$.bsa", GameParameter, true, maxSize, "Shadersfx"));
         Repacks.Add(new ConfigurationRepack("OctagonSounds$.bsa", GameParameter, false, maxSize, "Sound"));
         Repacks.Add(new ConfigurationRepack("OctagonMusic$.bsa", GameParameter, false, maxSize, "Music"));
         Repacks.Add(new ConfigurationRepack("OctagonMisc$.bsa", GameParameter, true, maxSize, "scripts", "grass"));
      }
   }
}