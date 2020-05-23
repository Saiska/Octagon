namespace Octagon.Models
{
   public class ProcessorStep1
   {
      public string FileSourceName { get; private set; }
      public string FileSourceFullName { get; private set; }
      public string FileTargetFullName { get; private set; }
      public bool IsBsa { get; private set; }

      public ProcessorStep1(string fileSourceName, string fileSourceFullName, string fileTargetFullName, bool isBsa)
      {
         FileSourceName = fileSourceName;
         FileSourceFullName = fileSourceFullName;
         FileTargetFullName = fileTargetFullName;
         IsBsa = isBsa;
      }
   }
}