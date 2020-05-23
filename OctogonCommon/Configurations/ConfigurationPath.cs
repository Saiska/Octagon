namespace OctagonCommon.Configurations
{
   public class ConfigurationPath
   {
      public ConfigurationPath()
      {
         PathBsarch = "bsarch.exe";
         PathTexconv = "texconv.exe";
         PathTexdiag = "texdiag.exe";
         PathGmic = "gmic.exe";
         PathCustomTool = "*.exe";
      }

      public string PathBsarch { get; set; }

      public string PathTexconv { get; set; }

      public string PathTexdiag { get; set; }

      public string PathGmic { get; set; }

      public string PathCustomTool { get; set; }

   }
}
