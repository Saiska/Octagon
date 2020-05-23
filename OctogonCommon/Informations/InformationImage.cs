using OctagonCommon.Statics;

namespace OctagonCommon.Informations
{
   public class InformationImage
   {
      public int Height { get; set; }
      public int Mipmaps { get; set; }
      public int Width { get; set; }
      public string Format { get; set; }
      public TypeTexCompression TypeTexCompression { get; set; }
   }
}