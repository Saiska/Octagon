using System;
using OctagonCommon.Statics;

namespace OctagonCommon.Args
{
   public class LoggerArgs : EventArgs
   {
      public string Text { get; set; }
      public TypeLog TypeLog { get; set; }

      public LoggerArgs(string text, TypeLog typeLog)
      {
         Text = text;
         TypeLog = typeLog;
      }
   }
}