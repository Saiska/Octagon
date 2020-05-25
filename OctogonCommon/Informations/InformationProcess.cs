using System.Collections.Generic;
using System.IO;

namespace OctagonCommon.Informations
{
   public class InformationProcess
   {
      public InformationProcess(List<string> output, string errors, bool hasError)
      {
         Output = output;
         Errors = errors;
         HasError = hasError;
      }
                                                        
      public List<string> Output { get; private set; }
      public string Errors { get; private set; }
      public bool HasError { get; private set; }

   }
}