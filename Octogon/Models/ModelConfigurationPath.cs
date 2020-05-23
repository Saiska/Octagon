using OctagonCommon.Configurations;

namespace Octagon.Models
{
   public class ModelConfigurationPath   :AModel
   {
      public ConfigurationPath Path { get; private set; }

      public ModelConfigurationPath(ConfigurationPath path)
      {
         Path = path;
      }

      public string PathBsarch
      {
         get { return Path.PathBsarch; }
         set
         {
            if (Path.PathBsarch != value)
            {
               Path.PathBsarch = value;
               OnPropertyChanged("PathBsarch");
            }
         }
      }

      public string PathGmic
      {
         get { return Path.PathGmic; }
         set
         {
            if (Path.PathGmic != value)
            {
               Path.PathGmic = value;
               OnPropertyChanged("PathGmic");
            }
         }
      }

      public string PathCustomTool
      {
         get { return Path.PathCustomTool; }
         set
         {
            if (Path.PathCustomTool != value)
            {
               Path.PathCustomTool = value;
               OnPropertyChanged("PathCustomTool");
            }
         }
      }

      public string PathTexconv
      {
         get { return Path.PathTexconv; }
         set
         {
            if (Path.PathTexconv != value)
            {
               Path.PathTexconv = value;
               OnPropertyChanged("PathTexconv");
            }
         }
      }

      public string PathTexdiag
      {
         get { return Path.PathTexdiag; }
         set
         {
            if (Path.PathTexdiag != value)
            {
               Path.PathTexdiag = value;
               OnPropertyChanged("PathTexdiag");
            }
         }
      }
   }
}
