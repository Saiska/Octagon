#region Dependencies

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctagonCommon.Configurations;
using OctagonCommon.Informations;
using OctagonCommon.Statics;

#endregion

namespace OctagonCommon.Executions
{
   public class FileInformation
   {
      private const string ProgressSearchingArchives = "Searching archives";
      private const string ProgressSearchingTextures = "Searching textures";


      public void GetFileInfos(string sourceDirectory, string targetDirectory, List<InformationFile> result, bool isBsa, ConfigurationMain main)
      {
         var eventName = isBsa ? ProgressSearchingArchives : ProgressSearchingTextures;
         var extension = isBsa ? ArchiveExtensionList.List : TextureExtensionList.List;
         //
         Progresser.EventStart(eventName);
         int fileCount = 0;
         foreach (var ext in extension)
         {
            fileCount += Directory.GetFiles(sourceDirectory, string.Format("*{0}", ext), SearchOption.AllDirectories).Length;
         }
         //
         DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
         if (string.IsNullOrEmpty(targetDirectory) || !main.IsBackupActivated)
         {
            GetFileInfos(diSource, result, isBsa, eventName, main, fileCount, true);
         }
         else
         {
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            GetFileInfos(diSource, diTarget, result, isBsa, eventName, main, fileCount, true);
         }
         //                                                               
         //                                           
         Logger.Log("Files scan complete: {0} {1} files found", result.Count, extension.Aggregate((i, j) => string.Format("{0}|{1}", i, j)));
         Progresser.EventEnd(eventName);
      }

      public void GetFileInfos(DirectoryInfo source, List<InformationFile> result, bool isBsa, string eventName,
        ConfigurationMain mainCfg, int fileCount, bool firstPass)
      {
         //                        
         var extension = isBsa ? ArchiveExtensionList.List : TextureExtensionList.List;
         //               
         //                                                  
         // Get  each file into the new directory.
         foreach (FileInfo fileSource in source.GetFiles())
         {                  
            // Patch for non existing ba2 but existing ba2 unarchived directory
            if (isBsa && EspExtensionList.List.Any(e => string.Equals(e, fileSource.Extension, StringComparison.InvariantCultureIgnoreCase)))
            {
               //var custom = FileUtils.GetBsaTempDirectory(fileSource);   
               FileInfo possible1 = new FileInfo(Path.Combine(fileSource.DirectoryName, Path.GetFileNameWithoutExtension(fileSource.FullName) + " - textures.ba2"));
               FileInfo possible2 = new FileInfo(Path.Combine(fileSource.DirectoryName, Path.GetFileNameWithoutExtension(fileSource.FullName) + " - main.ba2"));

               if (!File.Exists(possible1.FullName))
               {
                  var custom1 = FileUtils.GetBsaTempDirectory(possible1);
                  if (Directory.Exists(custom1))
                  {
                     result.Add(new InformationFile(possible1, possible1));
                  }
               }

               if (!File.Exists(possible2.FullName))
               {
                  var custom1 = FileUtils.GetBsaTempDirectory(possible2);
                  if (Directory.Exists(custom1))
                  {
                     result.Add(new InformationFile(possible2, possible2));
                  }
               }


            }
            //ignore no dds file                                                                               
            if (!extension.Any(e => string.Equals(e, fileSource.Extension, StringComparison.InvariantCultureIgnoreCase)))
            {
               continue;
            }
            //#if DEBUG
            //            Thread.CurrentThread.Join(10);
            //#endif
            //         
            // Stop here if all validation failed
            if (!isBsa)
            {
               if (mainCfg.Search.IsSearchInDepthNotNeeded(fileSource.Name))
               {
                  continue;
               }
               //
               bool passValidation = mainCfg.Search.IsSearchEnabled;
               //
               if (!passValidation)
               {
                  foreach (var scalePass in mainCfg.Passes)
                  {
                     if (scalePass.Selection.GetValidation(Path.GetFileNameWithoutExtension(fileSource.FullName), fileSource.FullName))
                     {
                        passValidation = true;
                        break;
                     }
                  }
               }
               //
               if (!passValidation)
                  continue;
            }
            else
            {
               if (!mainCfg.PassBsa.Selection.GetValidation(Path.GetFileNameWithoutExtension(fileSource.FullName), fileSource.FullName))
               {
                  continue;
               }
            }
            //     
            if (mainCfg.IsVerbose)
            {
               var msg = string.Format("Found: {0}", fileSource.FullName);
               //     
               Logger.Log(msg);
            }
            //      
            Progresser.ChangeProgress(eventName, fileCount);
            //
            result.Add(new InformationFile(fileSource, fileSource));
         }
         //             
         if (isBsa && !firstPass)
         {
            return;
         }
         // Copy each subdirectory using recursion.            
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            if (firstPass && !mainCfg.Selection.GetValidation(diSourceSubDir.Name, diSourceSubDir.FullName))
               continue;
            //
            GetFileInfos(diSourceSubDir, result, isBsa, eventName, mainCfg, fileCount, false);
         }

      }

      public void GetFileInfos(DirectoryInfo source, DirectoryInfo target, List<InformationFile> result, bool isBsa, string eventName,
         ConfigurationMain mainCfg, int fileCount, bool firstPass)
      {
         //                        
         var extension = isBsa ? ArchiveExtensionList.List : TextureExtensionList.List;
         //
         Directory.CreateDirectory(target.FullName);
         //                                                  
         // Get  each file into the new directory.
         foreach (FileInfo fileSource in source.GetFiles())
         {
            var path = Path.Combine(target.FullName, fileSource.Name);
            var fileTarget = new FileInfo(path);
            //ignore no dds file                                                                          
            if (!extension.Any(e => string.Equals(e, fileSource.Extension, StringComparison.InvariantCultureIgnoreCase)))
            {
               continue;
            }
            //#if DEBUG
            //            Thread.CurrentThread.Join(10);
            //#endif
            // Check from only new file comparing to archive
            if (mainCfg.IsOnlyNewFromArchive && mainCfg.IsBackupActivated && fileTarget.Exists)
            {
               continue;
            }
            //         
            // Stop here if all validation failed
            if (!isBsa)
            {
               if (mainCfg.Search.IsSearchInDepthNotNeeded(fileSource.Name))
               {
                  continue;
               }
               //
               bool passValidation = mainCfg.Search.IsSearchEnabled;
               //    
               if (!passValidation && mainCfg.IsBackupActivated && mainCfg.IsRecopyOriginal)
               {
                  if (fileTarget.Exists && fileTarget.Length != fileSource.Length)
                  {
                     passValidation = true;
                  }
               }
               //
               if (!passValidation)
               {
                  foreach (var scalePass in mainCfg.Passes)
                  {
                     if (scalePass.Selection.GetValidation(Path.GetFileNameWithoutExtension(fileSource.FullName), fileSource.FullName))
                     {
                        passValidation = true;
                        break;
                     }
                  }
               }
               //
               if (!passValidation)
                  continue;
            }
            else
            {
               if (!mainCfg.PassBsa.Selection.GetValidation(Path.GetFileNameWithoutExtension(fileSource.FullName), fileSource.FullName))
               {
                  continue;
               }
            }
            //     
            if (mainCfg.IsVerbose)
            {
               var msg = string.Format("Found: {0}", fileSource.FullName);
               //     
               Logger.Log(msg);
            }
            //      
            Progresser.ChangeProgress(eventName, fileCount);
            //
            result.Add(new InformationFile(fileSource, fileTarget));
         }
         //             
         if (isBsa && !firstPass)
         {
            return;
         }
         // Copy each subdirectory using recursion.            
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            if (firstPass && !mainCfg.Selection.GetValidation(diSourceSubDir.Name, diSourceSubDir.FullName))
               continue;
            //
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            //
            GetFileInfos(diSourceSubDir, nextTargetSubDir, result, isBsa, eventName, mainCfg, fileCount, false);
         }

      }

      public void CleanEmptySubdir(string mainCfgPathBackup)
      {
         Logger.Log("Cleaning backup empty directories...");
         DirectoryInfo dir = new DirectoryInfo(mainCfgPathBackup);
         foreach (DirectoryInfo subDir in dir.GetDirectories())
         {
            if (FileUtils.CalculateDirectorySize(subDir, true) == 0)
            {
               Directory.Delete(subDir.FullName, true);
            }
         }
      }

      public void CleanBackupFromSource(string mainCfgPathSource, string mainCfgPathBackup)
      {
         Logger.Log("Cleaning deleted mods...");
         //
         DirectoryInfo dirSrc = new DirectoryInfo(mainCfgPathSource);
         DirectoryInfo dirBck = new DirectoryInfo(mainCfgPathBackup);
         //
         foreach (DirectoryInfo subDir in dirBck.GetDirectories())
         {
            var tarDir = dirSrc.GetDirectories().FirstOrDefault(b => Equals(b.Name, subDir.Name));
            if (tarDir == null)
            {
               Directory.Delete(subDir.FullName, true);
            }
         }
      }
   }
}