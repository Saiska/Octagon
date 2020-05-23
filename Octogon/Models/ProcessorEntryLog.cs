using OctagonCommon.Args;
using OctagonCommon.Statics;

namespace Octagon.Models
{
   public class ProcessorEntryLog
   {
      public ProcessorEntryLog(LoggerArgs loggerArgs)
      {
         Text = loggerArgs.Text;
         TypeLog = loggerArgs.TypeLog;
      }

      public ProcessorEntryLog(string text, TypeLog typeLog)
      {
         Text = text;
         TypeLog = typeLog;
      }

      public TypeLog TypeLog { get; set; }
      public string Text { get; set; }
   }
}