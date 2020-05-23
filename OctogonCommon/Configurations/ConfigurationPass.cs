#region Dependencies

#endregion

using OctagonCommon.Statics;

namespace OctagonCommon.Configurations
{
   public class ConfigurationPass : IConfigurationSelection
   {
      public ConfigurationPass()
      {
         Selection = new ConfigurationSelection();
         //
         WantedFactor = 1;     
         WantedSize = 0;
         WantedMinSize = 128;
         WantedMaxSize = 4096;
         Command = string.Empty;
         TypeTexCompression = TypeTexCompression.None;
      }

      public int WantedFactor { get; set; }     
      public int WantedMinSize { get; set; }
      public int WantedMaxSize { get; set; }
      public int WantedSize { get; set; }
      public string ForceFormat { get; set; }
      public TypeTexCompression TypeTexCompression { get; set; }
      public TypePass TypePass { get; set; }
      public string Command { get; set; }
      public bool IsApplyOnPng { get; set; }

      public ConfigurationSelection Selection { get; set; }

      public static ConfigurationPass GetNewDivider(int div)
      {
         return new ConfigurationPass { WantedFactor = div, TypePass = TypePass.DownscaleFactor };
      }

      public static ConfigurationPass GetNewFixer(int size)
      {
         return new ConfigurationPass { WantedSize = size, TypePass = TypePass.DownscaleFixed };
      }

      public static ConfigurationPass GetNewForceFormat(string format)
      {
         return new ConfigurationPass { ForceFormat = format, TypePass = TypePass.Format };
      }

      public static ConfigurationPass GetForce()
      {
         return new ConfigurationPass { TypePass = TypePass.Force };
      }

      public static ConfigurationPass GetCustom()
      {
         return new ConfigurationPass { IsApplyOnPng = true, TypePass = TypePass.ApplyCustom };
      }

      public static ConfigurationPass GetUpscaleFactor(int mult)
      {
         return new ConfigurationPass { WantedFactor = mult, TypePass = TypePass.UpscaleFactor };
      }

      public static ConfigurationPass GetUpscaleFixed(int maxSize)
      {
         return new ConfigurationPass { WantedSize = maxSize, TypePass = TypePass.UpscaleFixed };
      }

      public static ConfigurationPass GetGmic()
      {
         return new ConfigurationPass { TypePass = TypePass.ApplyGmic };
      }

      public static ConfigurationPass GetForceMipmap()
      {
         return new ConfigurationPass { TypePass = TypePass.ForceMipmaps };
      }

      public static ConfigurationPass GetCorrectSize()
      {
         return new ConfigurationPass { TypePass = TypePass.CorrectSize };
      }

      public static ConfigurationPass GetCorrectMipmap()
      {
         return new ConfigurationPass { TypePass = TypePass.CorrectMipmaps };
      }
   }
}