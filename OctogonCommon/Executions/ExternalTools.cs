#region Dependencies

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using OctagonCommon.Configurations;
using OctagonCommon.Informations;
using OctagonCommon.Statics;

#endregion

namespace OctagonCommon.Executions
{
   public class ExternalTools
   {
      private const string ProgressPack = "Bsarch is packing";
      private const string ProgressUnpack = "Bsarch is unpacking";

      public ConfigurationPath ConfigurationPath { get; set; }

      public ExternalTools(ConfigurationPath configurationPath)
      {
         ConfigurationPath = configurationPath;
      }

      public List<string> CallBsaPack(string filePath, string dirPath, bool isCompressed, string bsaarchGameParameter, bool isMultithread, bool verbose)
      {
         var call = string.Format("pack \"{0}\" \"{1}\" -{2}", dirPath, filePath, bsaarchGameParameter);
         if (isCompressed)
         {
            call = string.Format("{0} -z", call);
         }
         if (isMultithread)
         {
            call = string.Format("{0} -mt", call);
         }
         //
         var fileName = Path.GetFileNameWithoutExtension(filePath);
         return CallBsarch(string.Format("{0}: {1}", ProgressPack, fileName), call, verbose);
      }

      private bool CallBsarch(string arg, string search)
      {
         var startInfo = new ProcessStartInfo
         {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            FileName = ConfigurationPath.PathBsarch,
            Arguments = arg
         };
         //
         var process = new Process { StartInfo = startInfo };
         process.Start();
         bool searchFound = false;
         if (string.IsNullOrEmpty(search))
            throw new Exception("Wrong search parameter");
         process.OutputDataReceived += (sender, args) =>
         {
            if (!string.IsNullOrEmpty(args.Data))
            {
               if (args.Data.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
               {
                  searchFound = true;
                  process.CancelOutputRead();
               }
            }
         };
         process.BeginOutputReadLine();
         process.WaitForExit();
         return searchFound;
      }

      private List<string> CallBsarch(string eventName, string arg, bool verbose)
      {
         var startInfo = new ProcessStartInfo
         {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            FileName = ConfigurationPath.PathBsarch,
            Arguments = arg
         };
         //
         var process = new Process { StartInfo = startInfo };
         process.Start();
         Progresser.EventStart(eventName);
         List<string> result = new List<string>();
         string error = null;
         process.OutputDataReceived += (sender, args) =>
         {
            result.Add(args.Data);
            var data = args.Data;
            if (!string.IsNullOrWhiteSpace(data))
            {
               data = data.Trim();
               if (data.StartsWith("[") && data.EndsWith("%]"))
               {
                  data = data.Replace("[", string.Empty);
                  data = data.Replace("%]", string.Empty);
                  int i;
                  if (int.TryParse(data, out i))
                  {
                     Progresser.ChangeProgressPct(eventName, i / 100d);
                  }
               }
               else if (data.StartsWith("EFCreateError", StringComparison.OrdinalIgnoreCase))
               {
                  error = string.Format("File system error, file name not ASCII ? {0}", data);
               }
               else if (data.ToLower().Contains("exception"))
               {
                  error = data;
               }
               else
               {
                  if (verbose)
                  {
                     Logger.Log(data);
                  }
               }
            }
         };
         process.BeginOutputReadLine();
         process.WaitForExit();
         Progresser.EventEnd(eventName);
         if (error != null)
         {
            throw new Exception(error);
         }
         return result;
      }

      public bool CallBsarchList(string filepath, string search)
      {
         return CallBsarch(string.Format(" \"{0}\" -list", filepath), search);
      }

      public bool CallBsarchProperty(string filepath, string search)
      {
         return CallBsarch(string.Format(" \"{0}\"", filepath), search);
      }

      public List<string> CallBsaUnPack(string filePath, string dirPath, bool isMultithread, bool verbose)
      {
         var fileName = Path.GetFileNameWithoutExtension(filePath);
         var call = string.Format("unpack \"{0}\" \"{1}\" ", filePath, dirPath);
         if (isMultithread)
         {
            call = string.Format("{0} -mt", call);
         }
         return CallBsarch(string.Format("{0}: {1}", ProgressUnpack, fileName), call, verbose);
      }

      public InformationProcess CallDxDiag(string filePath, bool isGetOutput, bool isVerbose)
      {
         var startInfo = new ProcessStartInfo
         {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            FileName = ConfigurationPath.PathTexdiag,
            Arguments = string.Format("info \"{0}\" ", filePath)
         };
         //
         var process = new Process { StartInfo = startInfo };
         return CheckOutput(process, isGetOutput, isVerbose);
      }

      public InformationProcess CallTexConv(string filePath, string ouputDir, int newW, int newH, int mips, string format, TypeTexCompression typeTexCompression, bool isGetOutput, bool isVerbose)
      {
         string compressionParam = string.Empty;
         switch (typeTexCompression)
         {
            case TypeTexCompression.BC7Max:
               compressionParam = " -bcmax";
               break;
            case TypeTexCompression.BC7Min:
               compressionParam = " -bcquick";
               break;
            case TypeTexCompression.BC13Dith:
               compressionParam = " -bcdither";
               break;
            case TypeTexCompression.BC13Uni:
               compressionParam = " -bcuniform";
               break;
         }
         var startInfo = new ProcessStartInfo
         {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            FileName = ConfigurationPath.PathTexconv,
            Arguments = string.Format("-nologo -y -sepalpha -f {0}{1} -w {2} -h {3} -m {4} -o \"{5}\" \"{6}\"", format, compressionParam, newW, newH, mips, ouputDir, filePath)
         };
         //
         var process = new Process { StartInfo = startInfo };
         return CheckOutput(process, isGetOutput, isVerbose);
      }

      public InformationProcess CallTexConvDdsToPng(string filePath, string ouputDir, bool isGetOutput, bool isVerbose)
      {
         var startInfo = new ProcessStartInfo
         {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            FileName = ConfigurationPath.PathTexconv,
            Arguments = string.Format("-nologo -y -ft png -o \"{0}\" \"{1}\"", ouputDir, filePath)
         };
         //
         var process = new Process { StartInfo = startInfo };
         return CheckOutput(process, isGetOutput, isVerbose);
      }

      private InformationProcess CheckOutput(Process process, bool isGetOutput, bool isVerbose)
      {
         bool isError = false;
         List<string> output = new List<string>();
         StringBuilder errors = new StringBuilder();
         //
         process.ErrorDataReceived += (sender, args) =>
         {
            if (!string.IsNullOrEmpty(args.Data))
            {
               Logger.Log(args.Data, TypeLog.Error);
               errors.AppendLine(args.Data);
               isError = true;
            }
         };
         process.OutputDataReceived += (sender, args) =>
         {
            if (!string.IsNullOrEmpty(args.Data))
            {
               if (isVerbose)
                  Logger.Log(args.Data);
               if (isGetOutput)
                  output.Add(args.Data.Trim());
            }
         };
         //                                 
         process.Start();
         //
         process.BeginErrorReadLine();
         process.BeginOutputReadLine();
         //                    
         process.WaitForExit();
         isError |= process.ExitCode > 0;
         process.Close();
         //
         if (isError || isVerbose)
         {
            var file = process.StartInfo.FileName;
            var args = process.StartInfo.Arguments;
            Logger.Log("Program call was: {0} {1}", file, args, TypeLog.Warning);
         }
         //
         return new InformationProcess(output, errors.ToString(), isError);
      }

      public void CallGmic(string filePath, string parameter)
      {
         var startInfo = new ProcessStartInfo
         {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = true,
            CreateNoWindow = true,
            //RedirectStandardOutput = true,       
            //RedirectStandardError = true,       
            FileName = ConfigurationPath.PathGmic,
            Arguments = string.Format("-v - \"{0}\" {1} output \"{2}\" ", filePath, parameter, filePath)
         };
         //
         var process = new Process { StartInfo = startInfo };
         process.Start();
         process.WaitForExit();
         // process.StandardError.ReadToEnd();
         // return process.StandardOutput.ReadToEnd();
      }

      public void CallCustom(string parameter)
      {
         var startInfo = new ProcessStartInfo
         {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = true,
            CreateNoWindow = true,
            //RedirectStandardOutput = true,       
            //RedirectStandardError = true,       
            FileName = ConfigurationPath.PathCustomTool,
            Arguments = string.Format("{0}", parameter)
         };
         //
         var process = new Process { StartInfo = startInfo };
         process.Start();
         process.WaitForExit();
         // process.StandardError.ReadToEnd();
         // return process.StandardOutput.ReadToEnd();
      }
   }
}