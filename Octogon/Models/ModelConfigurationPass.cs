using System;
using System.Windows;
using OctagonCommon.Configurations;
using OctagonCommon.Statics;

namespace Octagon.Models
{
   public class ModelConfigurationPass : AModel, IModelConfigurationSelection
   {
      public ModelConfigurationSelection ModelSelection { get; set; }

      public ConfigurationPass Pass { get; private set; }

      public ModelConfigurationPass(ConfigurationPass pass)
      {
         Pass = pass;
         ModelSelection = new ModelConfigurationSelection(pass.Selection);
         switch (pass.TypePass)
         {
            case TypePass.DownscaleFactor:
               Title = "Downscale texture by factor";
               break;
            case TypePass.DownscaleFixed:
               Title = "Downscale texture by size";
               break;
            case TypePass.Format:
               Title = "Change texture format";
               break;
            case TypePass.ApplyCustom:
               Title = "Apply custom tool";
               break;
            case TypePass.Force:
               Title = "Force recompression";
               break;
            case TypePass.ApplyGmic:
               Title = "Apply GMIC filter";
               break;
            case TypePass.UpscaleFactor:
               Title = "Upscale texture by factor";
               break;
            case TypePass.UpscaleFixed:
               Title = "Upscale texture by size";
               break;
            case TypePass.CorrectMipmaps:
               Title = "Mipmaps correction";
               break;
            case TypePass.ForceMipmaps:
               Title = "Force mipmaps generation";
               break;
            case TypePass.CorrectSize:
               Title = "Size correction";
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      public Visibility VisibilityDownscaleFactor { get { return Pass.TypePass == TypePass.DownscaleFactor  ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityDownscaleFixed { get { return Pass.TypePass == TypePass.DownscaleFixed ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityUpscaleFactor { get { return Pass.TypePass == TypePass.UpscaleFactor  ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityUpscaleFixed { get { return Pass.TypePass == TypePass.UpscaleFixed  ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityFormat { get { return Pass.TypePass == TypePass.Format ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityCompression { get { return Pass.TypePass == TypePass.Format || Pass.TypePass == TypePass.Force ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityMinSize { get { return Pass.TypePass == TypePass.DownscaleFactor || Pass.TypePass == TypePass.DownscaleFixed ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityMaxSize { get { return Pass.TypePass == TypePass.UpscaleFixed || Pass.TypePass == TypePass.UpscaleFactor ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityCommand { get { return Pass.TypePass == TypePass.ApplyCustom || Pass.TypePass == TypePass.ApplyGmic ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityApplyPng { get { return Pass.TypePass == TypePass.ApplyCustom ? Visibility.Visible : Visibility.Collapsed; } }


      public string Title { get; private set; }

      public TypePass TypePass
      {
         get { return Pass.TypePass; }
      }

      public string ForceFormat
      {
         get { return Pass.ForceFormat; }
         set
         {
            if (Pass.ForceFormat != value)
            {
               Pass.ForceFormat = value;
               OnPropertyChanged("ForceFormat");
            }
         }
      }

      public string Command
      {
         get { return Pass.Command; }
         set
         {
            if (Pass.Command != value)
            {
               Pass.Command = value;
               OnPropertyChanged("Command");
            }
         }
      }

      public bool IsApplyOnPng
      {
         get { return Pass.IsApplyOnPng; }
         set
         {
            if (Pass.IsApplyOnPng != value)
            {
               Pass.IsApplyOnPng = value;
               OnPropertyChanged("IsApplyOnPng");
            }
         }
      }

      public TypeTexCompression TypeTexCompression
      {
         get { return Pass.TypeTexCompression; }
         set
         {
            if (Pass.TypeTexCompression != value)
            {
               Pass.TypeTexCompression = value;
               OnPropertyChanged("TypeTexCompression");
            }
         }
      }

      public int WantedFactor
      {
         get { return Pass.WantedFactor; }
         set
         {
            if (Pass.WantedFactor != value)
            {
               Pass.WantedFactor = value;
               OnPropertyChanged("WantedFactor");
            }
         }
      }

      public int WantedMinSize
      {
         get { return Pass.WantedMinSize; }
         set
         {
            if (Pass.WantedMinSize != value)
            {
               Pass.WantedMinSize = value;
               OnPropertyChanged("WantedMinSize");
            }
         }
      }

      public int WantedMaxSize
      {
         get { return Pass.WantedMaxSize; }
         set
         {
            if (Pass.WantedMaxSize != value)
            {
               Pass.WantedMaxSize = value;
               OnPropertyChanged("WantedMinSize");
            }
         }
      }

      public int WantedSize
      {
         get { return Pass.WantedSize; }
         set
         {
            if (Pass.WantedSize != value)
            {
               Pass.WantedSize = value;
               OnPropertyChanged("WantedSize");
            }
         }
      }

   }
}
