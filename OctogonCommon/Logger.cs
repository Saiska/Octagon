using System;
using System.IO;
using OctagonCommon.Args;
using OctagonCommon.Statics;

namespace OctagonCommon
{
   public class Logger
   {
      private const string FileName = "logs.txt";

      public static Logger Instance;
      static Logger()
      {
         Instance = new Logger();
      }

      public Logger()
      {
         _lock = new object();
      }

      public bool FileLoggingEnabled;

      public event EventHandler<LoggerArgs> TextLogged;

      private void OnTextLogged(string text, TypeLog typeLog = TypeLog.Normal)
      {
         var handler = TextLogged;
         if (handler != null)
            handler(this, new LoggerArgs(GetFormattedText(text, typeLog), typeLog));
      }

      private string GetFormattedText(string text, TypeLog typeLog = TypeLog.Normal)
      {
         switch (typeLog)
         {
            case TypeLog.Normal:
               return text;
            case TypeLog.Warning:
               return string.Format("Info: {0}", text);
            case TypeLog.Error:
               return string.Format("Error: {0}", text);
            default:
               throw new ArgumentOutOfRangeException("typeLog", typeLog, null);
         }
      }

      private readonly object _lock;
      public void ProcessLog(string s, TypeLog typeLog = TypeLog.Normal)
      {
         //
         lock (_lock)
         {
            if (FileLoggingEnabled)
            {
               try
               {
                  using (var log = File.AppendText(FileName))
                  {
                     var message = string.Format("{0} {1}", DateTime.Now, GetFormattedText(s, typeLog));
                     log.WriteLine(message);
                  }
               }
               catch (Exception e)
               {
                  OnTextLogged(e.ToString(), TypeLog.Error);
                  throw;
               }
            }
            //    
            OnTextLogged(s, typeLog);
         }
      }

      public static void Log(string s, TypeLog typeLog = TypeLog.Normal)
      {
         Instance.ProcessLog(s, typeLog);
      }

      public static void Log(string s, object arg0, TypeLog typeLog = TypeLog.Normal)
      {
         Instance.ProcessLog(string.Format(s, arg0), typeLog);
      }

      public static void Log(string s, object arg0, object arg1, TypeLog typeLog = TypeLog.Normal)
      {
         Instance.ProcessLog(string.Format(s, arg0, arg1), typeLog);
      }

      public static void Log(string s, object arg0, object arg1, object arg2, TypeLog typeLog = TypeLog.Normal)
      {
         Instance.ProcessLog(string.Format(s, arg0, arg1, arg2), typeLog);
      }

      public static void Log(string s, object arg0, object arg1, object arg2, object arg3, TypeLog typeLog = TypeLog.Normal)
      {
         Instance.ProcessLog(string.Format(s, arg0, arg1, arg2, arg3), typeLog);
      }

      public static void Log(string s, object arg0, object arg1, object arg2, object arg3, object arg4, TypeLog typeLog = TypeLog.Normal)
      {
         Instance.ProcessLog(string.Format(s, arg0, arg1, arg2, arg3, arg4), typeLog);
      }

      public static void Log(Exception exception)
      {
         Instance.ProcessLog(exception.Message, TypeLog.Error);
      }
   }
}
