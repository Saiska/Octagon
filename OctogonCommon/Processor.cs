//  by Saiska 15/07/2019

#region Dependencies

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using OctagonCommon.Args;
using OctagonCommon.Configurations;
using OctagonCommon.Executions;
using OctagonCommon.Informations;
using OctagonCommon.Statics;

#endregion

namespace OctagonCommon
{
   public class Processor
   {
      //test GMIC
      //-source "C:\games\testtextures" -gmic "fx_retrofade 20,6,40,0" -ee _n.dds -ee _e.dds -ee _m.dds -bsa -i textures7 -ar -au -ac -m

      private const string ProgressChecking = "Checking parameters";

      public FileExecution FileExecution { get; set; }
      public FilePreparation FilePreparation { get; set; }
      public FileInformation FileInformation { get; set; }
      public ExternalTools ExternalTools { get; set; }
      public ProcessorArgs ProcessorArgs { get; set; }

      public Processor(ProcessorArgs processorArgs)
      {
         ProcessorArgs = processorArgs;
         //                               
         ExternalTools = new ExternalTools(ProcessorArgs.ConfigurationMain.Paths);
         FileExecution = new FileExecution(ExternalTools);
         FilePreparation = new FilePreparation(ExternalTools);
         FileInformation = new FileInformation();
      }

      public void StartProcess0()
      {
         Thread thread = new Thread(new ParameterizedThreadStart(StartProcess0));
         thread.Start(ProcessorArgs);
      }

      public void StartProcess1()
      {
         Thread thread = new Thread(new ParameterizedThreadStart(StartProcess1));
         thread.Start(ProcessorArgs);
      }

      public void StartProcess2()
      {
         Thread thread = new Thread(new ParameterizedThreadStart(StartProcess2));
         thread.Start(ProcessorArgs);
      }

      public void StartProcess3()
      {
         Thread thread = new Thread(new ParameterizedThreadStart(StartProcess3));
         thread.Start(ProcessorArgs);
      }

      //      public static string StartProcess(ProcessorArgs args)
      //      {
      //#if DEBUG
      //         Thread thread = new Thread(new ParameterizedThreadStart(StartProcess));
      //         thread.Start(args);
      //#else
      //         try
      //         {                       
      //            Thread thread = new Thread(new ParameterizedThreadStart(StartProcess));
      //            thread.Start(args);
      //         }
      //         catch (Exception e)
      //         {
      //            return string.Format("Exception: {0}", e);
      //         }
      //#endif
      //         return "Process ended without exceptions";
      //      }        


      public event EventHandler<StatusArgs> ProcessorEnded;

      private void OnProcessorEnded(int status)
      {
         var handler = ProcessorEnded;
         if (handler != null)
            handler(this, new StatusArgs(status));
      }

      private void StartProcess0(object o)
      {
         try
         {
            ProcessorArgs processorArgs = (ProcessorArgs)o;
            ConfigurationMain mainCfg = processorArgs.ConfigurationMain;
            //      
            Logger.Log("Checking configuration...");
            //
            Progresser.EventStart(ProgressChecking);
            Progresser.ChangeProgress(ProgressChecking, 0);
            //
            if (mainCfg.Passes.Any())
            {
               if (!File.Exists(mainCfg.Paths.PathTexconv))
               {
                  throw new FileNotFoundException("Texconv.exe not found. Check your exe paths in tool paths !");
               }
               //
               if (!File.Exists(mainCfg.Paths.PathTexdiag))
               {
                  throw new FileNotFoundException("Texdiag.exe not found. Check your exe paths in tool paths !");
               }
               //      
               if (mainCfg.Passes.Any(p => p.TypePass == TypePass.ApplyGmic) && !File.Exists(mainCfg.Paths.PathGmic))
               {
                  throw new FileNotFoundException("Gmic.exe used and not found. Check your exe paths in tool paths !");
               }
               //      
               if (mainCfg.Passes.Any(p => p.TypePass == TypePass.ApplyCustom) && !File.Exists(mainCfg.Paths.PathCustomTool))
               {
                  throw new FileNotFoundException("Custom tool used and not found. Check your exe paths in tool paths !");
               }
            }
            //
            if (mainCfg.PassBsa.Enabled && !File.Exists(mainCfg.Paths.PathBsarch))
            {
               throw new FileNotFoundException("Bsarch.exe not found. Check your exe paths in tool paths !");
            }
            //         
            Logger.Log("Needed tools are found: ok");
            //                                       
            if (string.IsNullOrEmpty(mainCfg.PathSource))
            {
               throw new ArgumentException("Source folder path is mandatory.");
            }
            //                                      
            if (!(new DirectoryInfo(mainCfg.PathSource).Exists))
            {
               throw new DirectoryNotFoundException("Source folder don't exist.");
            }
            //                                       
            if (mainCfg.IsBackupActivated && string.IsNullOrEmpty(mainCfg.PathBackup))
            {
               throw new ArgumentException("Backup folder path is mandatory when using backup functions.");
            }
            //                                      
            if (mainCfg.IsBackupActivated && !(new DirectoryInfo(mainCfg.PathBackup).Exists))
            {
               throw new DirectoryNotFoundException("Backup folder don't exist.");
            }
            //                            
            if (mainCfg.IsBackupActivated && Equals(mainCfg.PathSource, mainCfg.PathBackup))
            {
               throw new ArgumentException("Source folder and backup folder cannot be the same.");
            }
            //
            Logger.Log("Configuration: ok");
            //
            mainCfg.Selection.CalculateStartFileValidation();
            mainCfg.PassBsa.Selection.CalculateStartFileValidation();
            foreach (ConfigurationPass configurationPass in mainCfg.Passes)
            {
               configurationPass.Selection.CalculateStartFileValidation();
            }
            //
            Logger.Log("Initializing validations ok");
            //             
            Progresser.ChangeProgress(ProgressChecking, 1);
            Progresser.EventEnd(ProgressChecking);
            //
            OnProcessorEnded(1);
         }
         catch (AggregateException e)
         {
            Logger.Log(e);
            foreach (Exception innerException in e.InnerExceptions)
            {
               Logger.Log(innerException);
            }
            OnProcessorEnded(-1);
         }
         catch (Exception e)
         {
            Logger.Log(e);
            OnProcessorEnded(-1);
         }
      }

      private void StartProcess1(object o)
      {
         try
         {
            ProcessorArgs processorArgs = (ProcessorArgs)o;
            ConfigurationMain mainCfg = processorArgs.ConfigurationMain;
            //
            if (mainCfg.IsShowResults)
            {
               Logger.Log("Preparing futur results data");
               var infoTextures = new DirectoryInfo(mainCfg.PathSource);
               //                                                                                                         
               foreach (var directoryInfo in infoTextures.GetDirectories())
               {
                  processorArgs.Folders.Add(directoryInfo.Name,
                     new InformationDirectory { Size = FileUtils.CalculateDirectorySizeMo(directoryInfo, mainCfg.PassBsa.Enabled) });
               }
               processorArgs.Folders.Add(infoTextures.Name,
                  new InformationDirectory { Size = FileUtils.CalculateDirectorySizeMo(infoTextures, mainCfg.PassBsa.Enabled) });
            }
            //
            if (mainCfg.PassBsa.Enabled)
            {
               FileInformation.GetFileInfos(mainCfg.PathSource, mainCfg.PathBackup, processorArgs.FilesBsa, true, mainCfg);
               FilePreparation.PrepareTreat(processorArgs.FilesBsa, mainCfg, processorArgs.BsaOrders, null, null, true);
               FileExecution.ExecuteTreat(processorArgs.BsaOrders, mainCfg, true);
               if (mainCfg.PassBsa.MustUnpack())
               {
                  FileExecution.UnpackBsa(processorArgs.BsaOrders, mainCfg);
               }
               if (mainCfg.PassBsa.IsCopyAsLoose)
               {
                  if (mainCfg.PassBsa.IsCopyAsLooseIfDummy)
                  {
                     List<InformationOrder> confirmedBsaUnpack = new List<InformationOrder>();
                     List<InformationCopy> confirmedDeletes = new List<InformationCopy>();
                     foreach (InformationOrder bsaOrder in processorArgs.BsaOrders)
                     {
                        var bsaName = Path.GetFileNameWithoutExtension(bsaOrder.FileSource.FullName);
                        var espName = bsaName.ToUpper().Replace(" - TEXTURES", "").Replace("- TEXTURES", "");
                        var espFile = Path.Combine(bsaOrder.FileSource.Directory.FullName, string.Format("{0}.esp", espName));
                        FileInfo dummy = new FileInfo("dummy.esp");
                        if (!dummy.Exists)
                        {
                           throw new Exception("No dummy esp to compare from. A dummy.esp file must be present in Octagon directory.");
                        }
                        FileInfo esp = new FileInfo(espFile);
                        if (esp.Exists && dummy.Length == esp.Length)
                        {
                           confirmedBsaUnpack.Add(bsaOrder);
                           if (!confirmedDeletes.Any(e => string.Equals(e.Target, espFile, StringComparison.OrdinalIgnoreCase)))
                           {
                              confirmedDeletes.Add(new InformationCopy(null, espFile, false, false));
                           }
                        }
                     }
                     FileExecution.CopyBsaAsLooseSimplified(confirmedBsaUnpack, mainCfg);
                     foreach (InformationCopy confirmedDelete in confirmedDeletes)
                     {
                        FileUtils.ExecuteCopyOrDelete(confirmedDelete);
                     }
                  }
                  else
                  {
                     FileExecution.CopyBsaAsLooseSimplified(processorArgs.BsaOrders, mainCfg);
                  }
               }
            }
            //     
            if (mainCfg.HasTextureOperation() || (mainCfg.IsBackupActivated && mainCfg.IsRecopyOriginal))
            {
               FileInformation.GetFileInfos(mainCfg.PathSource, mainCfg.PathBackup, processorArgs.FilesDds, false, mainCfg);
            }
            OnProcessorEnded(2);
         }
         catch (AggregateException e)
         {
            Logger.Log(e);
            foreach (Exception innerException in e.InnerExceptions)
            {
               Logger.Log(innerException);
            }
            OnProcessorEnded(-1);
         }
         catch (Exception e)
         {
            Logger.Log(e);
            OnProcessorEnded(-1);
         }
      }


      private void StartProcess2(object o)
      {
         try
         {
            ProcessorArgs processorArgs = (ProcessorArgs)o;
            ConfigurationMain mainCfg = processorArgs.ConfigurationMain;
            //
            FilePreparation.PrepareTreat(processorArgs.FilesDds, mainCfg, processorArgs.Orders, processorArgs.DiscardedOrders, processorArgs.SearchResults, false);
            //
            if (mainCfg.IsMergeActivated)
            {
               if (mainCfg.IsUnmergeActivated)
               {
                  FilePreparation.PrepareUnmerge(mainCfg, processorArgs.DeleteOrders);
               }
               else
               {
                  FilePreparation.PrepareMerge(mainCfg, processorArgs.CopyOrders);
               }
            }
            //
            OnProcessorEnded(3);
         }
         catch (AggregateException e)
         {
            Logger.Log(e);
            foreach (Exception innerException in e.InnerExceptions)
            {
               Logger.Log(innerException);
            }
            OnProcessorEnded(-1);
         }
         catch (Exception e)
         {
            Logger.Log(e);
            OnProcessorEnded(-1);
         }
      }

      private void StartProcess3(object o)
      {
         try
         {
            ProcessorArgs processorArgs = (ProcessorArgs)o;
            ConfigurationMain mainCfg = processorArgs.ConfigurationMain;
            //
            FileExecution.ExecuteTreat(processorArgs.Orders, mainCfg, false);
            //
            if (mainCfg.PassBsa.Enabled)
            {
               if (mainCfg.PassBsa.MustRepack())
               {
                  FileExecution.RepackBsa(processorArgs.BsaOrders, mainCfg.PassBsa.GameParameter, mainCfg.IsUseMultithreading, mainCfg.IsVerbose);
               }
               if (mainCfg.PassBsa.MustClean())
               {
                  FileExecution.CleanBsa(processorArgs.BsaOrders, mainCfg.IsVerbose);
               }
            }
            //
            if (mainCfg.IsMergeActivated)
            {
               if (mainCfg.IsUnmergeActivated)
               {
                  FileExecution.DeleteFiles(mainCfg, processorArgs.DeleteOrders);
               }
               else
               {
                  FileExecution.Merge(mainCfg, processorArgs.CopyOrders);
               }
            }
            //
            if (mainCfg.PassBsa.Enabled && mainCfg.PassBsa.IsRepackLooseFilesInBsa)
            {
               var bsaToProcess = mainCfg.PassBsa.IsIntelligentPacking ?
                  FileUtils.GetIntelligentPacking(mainCfg.Selection, mainCfg.PathSource, mainCfg.PassBsa.GameParameter, mainCfg.PassBsa.IsRepackCreateDummy, mainCfg.IsVerbose) :
                  FileUtils.PrepareForPacking(mainCfg.PassBsa.Repacks, mainCfg.PathSource, true, mainCfg.IsVerbose);



               List<DirectoryInfo> dirToDeleteIfFail = new List<DirectoryInfo>();
               foreach (InformationRepackBsa informationRepackBsa in bsaToProcess)
               {

                  ExternalTools.CallBsaPack(Path.Combine(informationRepackBsa.Source.FullName, informationRepackBsa.CurrentBsaName),
                     informationRepackBsa.CurrentTarget.FullName, informationRepackBsa.IsCompressed, informationRepackBsa.GameParameter, mainCfg.IsUseMultithreading, mainCfg.IsVerbose);
                  //
                  informationRepackBsa.CurrentTarget.Refresh();
                  if (informationRepackBsa.CurrentTarget.Exists)
                  {
                     try
                     {
                        FileUtils.DeleteCompleteDirectory(informationRepackBsa.CurrentTarget);
                     }
                     catch
                     {
                        dirToDeleteIfFail.Add(informationRepackBsa.CurrentTarget);
                     }
                  }
               }
               //
               foreach (DirectoryInfo directoryInfo in dirToDeleteIfFail)
               {
                  FileUtils.DeleteCompleteDirectory(directoryInfo);
               }
            }
            //Cleaning empty subdir      
            if (mainCfg.IsBackupActivated && mainCfg.IsRefreshBackup)
            {
               FileInformation.CleanEmptySubdir(mainCfg.PathBackup);
            }
            //Cleaning no present source subdir      
            if (mainCfg.IsBackupActivated && mainCfg.IsCleanBackup)
            {
               FileInformation.CleanBackupFromSource(mainCfg.PathSource, mainCfg.PathBackup);
            }
            //
            if (mainCfg.IsShowResults)
            {
               var infoTextures = new DirectoryInfo(mainCfg.PathSource);
               //
               foreach (var directoryInfo in infoTextures.GetDirectories())
               {
                  processorArgs.Folders[directoryInfo.Name].NewSize = FileUtils.CalculateDirectorySizeMo(directoryInfo, mainCfg.PassBsa.Enabled);
               }
               processorArgs.Folders[infoTextures.Name].NewSize = FileUtils.CalculateDirectorySizeMo(infoTextures, mainCfg.PassBsa.Enabled);
               // Show sizes before/after resizes
               Logger.Log("Results: (Size before resize in Mo => current size in Mo)");
               foreach (var keyValuePair in processorArgs.Folders.OrderBy(e => e.Value.Size))
               {
                  Logger.Log("{0,15:F2} Mo => {1,15:F2} Mo {2}",
                     keyValuePair.Value.Size,
                     keyValuePair.Value.NewSize,
                     keyValuePair.Key);
               }
            }

            OnProcessorEnded(-1);
         }
         catch (AggregateException e)
         {
            Logger.Log(e);
            foreach (Exception innerException in e.InnerExceptions)
            {
               Logger.Log(innerException);
            }
            OnProcessorEnded(-1);
         }
         catch (Exception e)
         {
            Logger.Log(e);
            OnProcessorEnded(-1);
         }
      }

      //      private static void StartProcess(object o)
      //      {
      //         ProcessorArgs processorArgs = (ProcessorArgs)o;
      //         ConfigurationMain mainCfg = processorArgs.ConfigurationMain;
      //         //
      //         Logger.Log("Starting...");
      //         //                                          
      //         var folders = new Dictionary<string, InformationDirectory>();
      //         // 
      //         var ddsFiles = new List<InformationFile>();
      //         var bsaFiles = new List<InformationFile>();
      //         //          
      //         if (mainCfg.IsShowResults)
      //         {
      //            var infoTextures = new DirectoryInfo(mainCfg.PathSource);
      //            //                                                                                                         
      //            foreach (var directoryInfo in infoTextures.GetDirectories())
      //            {
      //               folders.Add(directoryInfo.Name,
      //                  new InformationDirectory { Size = FileUtils.CalculateDirectorySizeMo(directoryInfo, mainCfg.PassBsa.Enabled) });
      //            }
      //            folders.Add(infoTextures.Name,
      //               new InformationDirectory { Size = FileUtils.CalculateDirectorySizeMo(infoTextures, mainCfg.PassBsa.Enabled) });
      //         }
      //         //
      //         List<InformationOrder> bsaOrders = null;
      //         if (mainCfg.PassBsa.Enabled)
      //         {
      //            Logger.Log("Preprocessing bsa archives...");
      //            FileInformation.GetFileInfos(mainCfg.PathSource, mainCfg.PathBackup, bsaFiles, ".BSA", mainCfg);
      //            bsaOrders = FilePreparation.PrepareTreat(bsaFiles, mainCfg, true);
      //            FileExecution.ExecuteTreat(bsaOrders, mainCfg);
      //            if (mainCfg.PassBsa.IsUnpack)
      //            {
      //               Logger.Log("Unpacking bsa to temp files...");
      //               FileExecution.UnpackBsa(bsaOrders, mainCfg);
      //            }
      //         }
      //         //   
      //         Logger.Log("Working on textures...");
      //         //
      //         FileInformation.GetFileInfos(mainCfg.PathSource, mainCfg.PathBackup, ddsFiles, ".DDS", mainCfg);
      //         //
      //         // No textures dir has been found, strange

      //         //        
      //         var ddsOrders = FilePreparation.PrepareTreat(ddsFiles, mainCfg, false);
      //         //               
      //#if DEBUG
      //         Logger.Log("(Debug) Hit key to start processing");
      //         Console.ReadKey();
      //#endif
      //         FileExecution.ExecuteTreat(ddsOrders, mainCfg);
      //         //
      //         if (mainCfg.PassBsa.Enabled)
      //         {
      //            if (mainCfg.PassBsa.IsRepack)
      //            {
      //               Logger.Log("Repacking bsa archives...");
      //               FileExecution.RepackBsa(bsaOrders, mainCfg.PassBsa.GameParameter, mainCfg.IsVerbose);
      //            }
      //            if (mainCfg.PassBsa.IsClean)
      //            {
      //               Logger.Log("Cleaning bsa temp files...");
      //               FileExecution.CleanBsa(bsaOrders, mainCfg.IsVerbose);
      //            }
      //         }
      //         //Cleaning empty subdir      
      //         if (mainCfg.IsRefreshBackup)
      //         {
      //            Logger.Log("Cleaning backup empty directories...");
      //            FileInformation.CleanEmptySubdir(mainCfg.PathBackup);
      //         }
      //         //Cleaning no present source subdir      
      //         if (mainCfg.IsCleanBackup)
      //         {
      //            Logger.Log("Cleaning deleted mods...");
      //            FileInformation.CleanBackupFromSource(mainCfg.PathSource, mainCfg.PathBackup);
      //         }
      //         //
      //         if (mainCfg.IsShowResults)
      //         {
      //            var infoTextures = new DirectoryInfo(mainCfg.PathSource);
      //            //
      //            foreach (var directoryInfo in infoTextures.GetDirectories())
      //            {
      //               folders[directoryInfo.Name].NewSize = FileUtils.CalculateDirectorySizeMo(directoryInfo, mainCfg.PassBsa.Enabled);
      //            }
      //            folders[infoTextures.Name].NewSize = FileUtils.CalculateDirectorySizeMo(infoTextures, mainCfg.PassBsa.Enabled);
      //            // Show sizes before/after resizes
      //            Logger.Log("Results: (Size before resize in Mo => current size in Mo)");
      //            foreach (var keyValuePair in folders.OrderBy(e => e.Value.Size))
      //            {
      //               Logger.Log("{0,15:F2} Mo => {1,15:F2} Mo {2}",
      //                  keyValuePair.Value.Size,
      //                  keyValuePair.Value.NewSize,
      //                  keyValuePair.Key);
      //            }
      //         }
      //      }
   }
}