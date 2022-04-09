#region Dependencies

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using OctagonCommon.Configurations;
using OctagonCommon.Informations;
using OctagonCommon.Statics;

#endregion

namespace OctagonCommon.Executions
{
   public static class FileUtils
   {
      //sArchiveToLoadInMemoryList=Skyrim - Animations.bsa
      //sResourceArchiveList=Skyrim - Misc.bsa, Skyrim - Shaders.bsa, Skyrim - Interface.bsa, Skyrim - Animations.bsa, Skyrim - Meshes0.bsa, Skyrim - Meshes1.bsa, Skyrim - Sounds.bsa
      //sResourceArchiveList2=voicesen0.bsa, voicesen1.bsa, voicesen2.bsa, voicesen3.bsa, voicesen4.bsa, Skyrim - Textures0.bsa, Skyrim - Textures1.bsa, Skyrim - Textures2.bsa, Skyrim - Textures3.bsa, Skyrim - Textures4.bsa, Skyrim - Textures5.bsa, Skyrim - Textures6.bsa, Skyrim - Textures7.bsa, Skyrim - Textures8.bsa, Skyrim - Patch.bsa

      //sArchiveToLoadInMemoryList=
      //sResourceArchiveList=OctagonMisc0.bsa, OctagonShaders0.bsa, OctagonInterface0.bsa, OctagonMeshes0.bsa, OctagonMeshes1.bsa, OctagonMeshes2.bsa, OctagonMeshes3.bsa, OctagonMeshes4.bsa, OctagonTextures0.bsa, OctagonTextures1.bsa, OctagonTextures2.bsa, OctagonTextures3.bsa, OctagonTextures4.bsa, OctagonTextures5.bsa, OctagonTextures6.bsa, OctagonTextures7.bsa, OctagonTextures8.bsa, OctagonTextures9.bsa, OctagonTextures10.bsa, OctagonTextures11.bsa
      //sResourceArchiveList2= OctagonMisc0.bsa, OctagonSounds0.bsa, OctagonSounds1.bsa, OctagonSounds2.bsa, OctagonSounds3.bsa, OctagonSounds4.bsa, OctagonSounds5.bsa, OctagonSounds6.bsa

      private static readonly List<string> FileExtentionOkInArchive = new List<string>() { ".tri", ".seq", ".png", ".dds", ".fuz", ".lip", ".wav", ".fxp", ".pex", ".xwm", ".nif", ".gid", ".swf", ".hkx", ".txt" };

      private static List<string> PossibleBsaDirs = new List<string>{ "meshes",
         "sound",
         "scripts",
         "seq",
         "Shadersfx",
         "grass",
         "Interface",
         "Music","textures"};

      public static long CalculateDirectorySize(DirectoryInfo d)
      {
         long size = 0;
         // Add file sizes.
         FileInfo[] fis = d.GetFiles();
         foreach (FileInfo fi in fis)
         {
            //ignore no dds file    
            size += fi.Length;
         }
         // Add subdirectory sizes.
         DirectoryInfo[] dis = d.GetDirectories();
         foreach (DirectoryInfo di in dis)
         {
            size += CalculateDirectorySize(di);
         }
         return size;
      }

      public static long CalculateDirectorySize(DirectoryInfo d, bool takeBsaToo)
      {
         long size = 0;
         // Add file sizes.
         FileInfo[] fis = d.GetFiles();
         foreach (FileInfo fi in fis)
         {
            //ignore no dds file
            // if(ArchiveExtensionList.List : TextureExtensionList.List;)
            if (TextureExtensionList.List.Any(e => string.Equals(e, fi.Extension, StringComparison.InvariantCultureIgnoreCase))
               || takeBsaToo && ArchiveExtensionList.List.Any(e => string.Equals(e, fi.Extension, StringComparison.InvariantCultureIgnoreCase)))
            {
               size += fi.Length;
            }
         }
         // Add subdirectory sizes.
         DirectoryInfo[] dis = d.GetDirectories();
         foreach (DirectoryInfo di in dis)
         {
            size += CalculateDirectorySize(di, takeBsaToo);
         }
         return size;
      }

      public static double CalculateDirectorySizeMo(DirectoryInfo d, bool takeBsaToo)
      {
         return CalculateDirectorySize(d, takeBsaToo) / (1024d * 1024d);
      }
      public static double CalculateDirectorySizeMo(DirectoryInfo d)
      {
         return CalculateDirectorySize(d) / (1024d * 1024d);
      }

      static long GetInt64HashCode(string strText)
      {
         long hashCode = 0;
         if (!string.IsNullOrEmpty(strText))
         {
            //Unicode Encode Covering all characterset
            byte[] byteContents = Encoding.Unicode.GetBytes(strText);
            SHA256 hash = new SHA256CryptoServiceProvider();
            byte[] hashText = hash.ComputeHash(byteContents);
            //32Byte hashText separate
            //hashCodeStart = 0~7  8Byte
            //hashCodeMedium = 8~23  8Byte
            //hashCodeEnd = 24~31  8Byte
            //and Fold
            long hashCodeStart = BitConverter.ToInt64(hashText, 0);
            long hashCodeMedium = BitConverter.ToInt64(hashText, 8);
            long hashCodeEnd = BitConverter.ToInt64(hashText, 24);
            hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
         }
         return (hashCode);
      }

      public static void CopyDirectoryTree(DirectoryInfo source, DirectoryInfo target)
      {
         Directory.CreateDirectory(target.FullName);
         //
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            //
            CopyDirectoryTree(diSourceSubDir, nextTargetSubDir);
         }
      }

      public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target, Action<string> action)
      {
         Directory.CreateDirectory(target.FullName);
         //
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            //
            CopyDirectory(diSourceSubDir, nextTargetSubDir, action);
         }
         //                  
         foreach (FileInfo fileInfo in source.GetFiles())
         {
            var path = Path.Combine(target.FullName, fileInfo.Name);
            action(string.Format("Copy {0}", fileInfo.FullName));
            fileInfo.CopyTo(path, false);
         }
      }

      public static void CopyDirectoryIfDate(DirectoryInfo source, DirectoryInfo target, DateTime date, Action<string> action)
      {
         Directory.CreateDirectory(target.FullName);
         //
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            //
            CopyDirectoryIfDate(diSourceSubDir, nextTargetSubDir, date, action);
         }
         //                  
         foreach (FileInfo fileInfo in source.GetFiles())
         {
            if (fileInfo.Name.Equals("meta.ini", StringComparison.OrdinalIgnoreCase))
               continue;
            if (DateTime.Compare(fileInfo.CreationTime, date) > 0 || DateTime.Compare(fileInfo.LastWriteTime, date) > 0)
            {
               var path = Path.Combine(target.FullName, fileInfo.Name);
               action(string.Format("Copy {0}", fileInfo.FullName));
               fileInfo.CopyTo(path, false);
            }

         }
      }

      public static void DeleteEmptyDirectory(DirectoryInfo source)
      {
         source.Refresh();
         //
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            DeleteEmptyDirectory(diSourceSubDir);
         }
         //   
         source.Refresh();
         //
         if (!source.GetDirectories().Any() && !source.GetFiles().Any())
         {
            try { source.Delete(); }
            catch (Exception e)
            {
               Logger.Log(e);
               Logger.Log(string.Format("Octagon continue without deleting {0} because it's used by another process. You can delete it after, it should be empty.", source.FullName), TypeLog.Warning);
            }
         }
      }

      public static void DeleteCompleteDirectory(DirectoryInfo source)
      {
         source.Refresh();
         //
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            DeleteCompleteDirectory(diSourceSubDir);
         }
         //   
         source.Refresh();
         //
         foreach (FileInfo fileInfo in source.GetFiles())
         {
            FileDeleteReadOnlyAttribute(fileInfo);
            fileInfo.Delete();
         }
         //
         source.Refresh();
         //
         if (!source.GetDirectories().Any() && !source.GetFiles().Any())
         {
            source.Delete();
         }
      }

      private static void FileDeleteReadOnlyAttribute(FileInfo fileInfo)
      {
         if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
         {
            File.SetAttributes(fileInfo.FullName, fileInfo.Attributes & ~FileAttributes.ReadOnly);
         }
      }

      public static List<InformationRepackBsa> PrepareForPacking(List<ConfigurationRepack> repacks, string pathSource, string defaultGameParmaeter, bool isNumbered, bool isVerbose)
      {
         var source = new DirectoryInfo(pathSource);
         //Generate directory for repack list
         var repacksList = new List<InformationRepack>();
         //
         foreach (ConfigurationRepack configurationRepack in repacks)
         {
            if (string.IsNullOrEmpty(configurationRepack.GameParameter))
            {
               configurationRepack.GameParameter = defaultGameParmaeter;
            }
            var currentRepack = new InformationRepack(configurationRepack);
            bool addValidated = false;
            foreach (DirectoryInfo directoryInfo in source.GetDirectories())
            {
               if (configurationRepack.SourceNames.Any(s => string.Equals(s.Value, directoryInfo.Name, StringComparison.OrdinalIgnoreCase)))
               {
                  currentRepack.Sources.Add(directoryInfo);
                  addValidated = true;
               }
            }
            //
            if (addValidated)
            {
               repacksList.Add(currentRepack);
            }
         }
         //                
         var result = new List<InformationRepackBsa>();
         foreach (InformationRepack informationRepack in repacksList)
         {
            int currentDir = 0;
            if (isNumbered && !informationRepack.ConfigurationRepack.BsaName.Contains("$"))
               throw new Exception(string.Format("Repacking BSA as {0} is not valid: names of bsa files must contains $ for multiple bsa files", informationRepack.ConfigurationRepack.BsaName));
            //
            while (true)
            {
               //
               long currentSize = 0;
               //
               var currentBsaName = isNumbered
                  ? informationRepack.ConfigurationRepack.BsaName.Replace("$", string.Format("{0}", currentDir))
                  : informationRepack.ConfigurationRepack.BsaName;
               var bsaPath = Path.Combine(source.FullName, currentBsaName);
               var bsaFile = new FileInfo(bsaPath);
               if (bsaFile.Exists)
               {
                  currentDir++;
                  continue;
               }
               var currentTarget = Directory.CreateDirectory(GetBsaTempName(source, currentBsaName));
               //        
               if (isVerbose)
               {
                  Logger.Log("Preparing new archive {0}", currentBsaName);
               }
               //
               foreach (DirectoryInfo informationRepackSource in informationRepack.Sources)
               {
                  var currentTargetWithSubDir = currentTarget.CreateSubdirectory(informationRepackSource.Name);
                  informationRepackSource.Refresh();
                  if (!informationRepackSource.Exists)
                     continue;
                  //
                  currentSize = MoveFilesUntilSizeReached(informationRepackSource, currentTargetWithSubDir, currentSize, informationRepack.ConfigurationRepack.Size);
                  //
                  if (currentSize >= informationRepack.ConfigurationRepack.Size)
                     break;
                  //
                  DeleteEmptyDirectory(informationRepackSource);
               }
               //
               result.Add(new InformationRepackBsa(source, currentBsaName, informationRepack.ConfigurationRepack.GameParameter, currentTarget, informationRepack.ConfigurationRepack.IsCompressed));
               //
               currentDir++;
               //  
               if (currentSize < informationRepack.ConfigurationRepack.Size)
                  break;
            }
         }
         //
         return result;
      }


      public static List<InformationRepackBsa> GetIntelligentPacking(ConfigurationSelection passBsaSelection, string pathSource, string gameParameter, bool isCreateDummy, bool isVerbose)
      {
         var source = new DirectoryInfo(pathSource);
         var result = new List<InformationRepackBsa>();
         //     
         //
         foreach (DirectoryInfo directoryInfo in source.GetDirectories())
         {
            if (!passBsaSelection.GetValidation(directoryInfo.Name, directoryInfo.FullName))
               continue;
            //
            //auto correction when doing bullshit 
            var correctThese = directoryInfo.GetDirectories().Where(d => d.Name.EndsWith("octatempdir", StringComparison.OrdinalIgnoreCase));
            foreach (DirectoryInfo info in correctThese)
            {
               MoveFiles(info, directoryInfo);
               DeleteEmptyDirectory(info);
            }
            directoryInfo.Refresh();
            //dummy esp

            //          
            if (!directoryInfo.GetFiles().Any(f => string.Equals(f.Extension, ".bsa", StringComparison.OrdinalIgnoreCase))
               && directoryInfo.GetDirectories().Any(d => PossibleBsaDirs.Any(e => string.Equals(e, d.Name, StringComparison.OrdinalIgnoreCase)))
               && SearchAnyFilesOkForArchive(directoryInfo))
            {

               //
               var espFile = directoryInfo.GetFiles().FirstOrDefault(f => string.Equals(f.Extension, ".esp", StringComparison.OrdinalIgnoreCase)
                  || string.Equals(f.Extension, ".esm", StringComparison.OrdinalIgnoreCase)
                  || string.Equals(f.Extension, ".esl", StringComparison.OrdinalIgnoreCase));
               //
               if (isCreateDummy && espFile == null)
               {

                  FileInfo fDummy = new FileInfo("dummy.esp");
                  if (fDummy.Exists)
                  {
                     File.Copy(fDummy.FullName, Path.Combine(directoryInfo.FullName, string.Format("{0}.esp", directoryInfo.Name)));
                     espFile = directoryInfo.GetFiles().FirstOrDefault(f => string.Equals(f.Extension, ".esp", StringComparison.OrdinalIgnoreCase));
                  }
               }
               //
               if (espFile != null)
               {
                  bool isSound = directoryInfo.GetDirectories().Any(d =>
                     string.Equals(d.Name, "music", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(d.Name, "sound", StringComparison.OrdinalIgnoreCase));
                  bool isTextureOnly = directoryInfo.GetDirectories().Length == 1 && directoryInfo.GetDirectories()
                                          .Any(d => string.Equals(d.Name, "textures", StringComparison.OrdinalIgnoreCase));
                  var repackCfg = GetGoodRepackConfiguration(Path.GetFileNameWithoutExtension(espFile.FullName), gameParameter, isSound, isTextureOnly);
                  var listResult = PrepareForPacking(repackCfg, directoryInfo.FullName, gameParameter, false, isVerbose);
                  result.AddRange(listResult);
               }
            }
            //
         }
         //
         return result;
      }

      private static List<ConfigurationRepack> GetGoodRepackConfiguration(string espName, string gameParameter, bool isSound, bool isTextureOnly)
      {
         bool isFo4 = string.Equals(gameParameter, GameParameterExtensionList.Fo4.Parameter, StringComparison.OrdinalIgnoreCase) ||
                      string.Equals(gameParameter, GameParameterExtensionList.Fo4dds.Parameter, StringComparison.OrdinalIgnoreCase);

         if (isTextureOnly)
         {
            return new List<ConfigurationRepack>()
            {
               new ConfigurationRepack()
               {
                  BsaName = string.Format("{0}.bsa", espName),
                  IsCompressed = true,
                  Size = 88000000000,
                  GameParameter = isFo4?GameParameterExtensionList.Fo4dds.Parameter:gameParameter,
                  SourceNames = new List<ConfigurationString>()
                  {
                     new ConfigurationString("textures")
                  }
                  
               }
            };
         }
         if (isSound)
         {
            return new List<ConfigurationRepack>(){
               new ConfigurationRepack()
               {
                  BsaName = string.Format("{0} - Textures.bsa",espName),IsCompressed = true,Size = 88000000000,   
                  GameParameter = isFo4?GameParameterExtensionList.Fo4dds.Parameter:gameParameter,
                  SourceNames = new List<ConfigurationString>()
                  {
                     new ConfigurationString("textures")
                  }
               } , 
               new ConfigurationRepack()
               {
                  BsaName =  string.Format("{0}.bsa",espName),IsCompressed = false,Size = 88000000000, 
                  GameParameter = isFo4?GameParameterExtensionList.Fo4.Parameter:gameParameter,
                  SourceNames = new List<ConfigurationString>()
                  {
                     new ConfigurationString("meshes"),
                     new ConfigurationString("materials"),
                     new ConfigurationString("sound"),
                     new ConfigurationString("scripts"),
                     new ConfigurationString("seq"),
                     new ConfigurationString("Shadersfx"),
                     new ConfigurationString("grass"),
                     new ConfigurationString("Interface"),
                     new ConfigurationString("Music"),
                  }
               } ,
            
            };
         }
         else
         {
            return new List<ConfigurationRepack>(){
               new ConfigurationRepack()
               {
                  BsaName =  string.Format("{0} - Textures.bsa",espName),IsCompressed = true,Size = 88000000000,   
                  GameParameter = isFo4?GameParameterExtensionList.Fo4dds.Parameter:gameParameter,
                  SourceNames = new List<ConfigurationString>()
                  {
                     new ConfigurationString("textures")
                  }
               } , 
               new ConfigurationRepack()
               {
                  BsaName =  string.Format("{0}.bsa",espName),IsCompressed = true,Size = 88000000000,    
                  GameParameter = isFo4?GameParameterExtensionList.Fo4.Parameter:gameParameter,
                  SourceNames = new List<ConfigurationString>()
                  {
                     new ConfigurationString("meshes"),   
                     new ConfigurationString("scripts"),   
                     new ConfigurationString("materials"),
                     new ConfigurationString("seq"),
                     new ConfigurationString("Shadersfx"),
                     new ConfigurationString("grass"),
                     new ConfigurationString("Interface"),    
                  }
               } ,  
            };
         }
      }

      public static bool IsASCII(this string value)
      {
         // ASCII encoding replaces non-ascii with question marks, so we use UTF8 to see if multi-byte sequences are there
         return Encoding.UTF8.GetByteCount(value) == value.Length;
      }

      public static long MoveFilesUntilSizeReached(DirectoryInfo source, DirectoryInfo target, long currentSize, long maxSize)
      {
         //
         Directory.CreateDirectory(target.FullName);
         //
         // Get  each file into the new directory.
         foreach (FileInfo fileSource in source.GetFiles())
         {
            if (FileExtentionOkInArchive.All(s => !string.Equals(fileSource.Extension, s, StringComparison.OrdinalIgnoreCase)))
               continue;
            //    
            if (!IsASCII(fileSource.Name))
            {
               Logger.Log("This file will not be moved for bsa archiving since is name is not ASCII and will not be loaded by the game {0}", fileSource.FullName, TypeLog.Warning);
               continue;
            }
            //
            var path = Path.Combine(target.FullName, fileSource.Name);
            //          
            ExecuteMove(new InformationCopy(fileSource, path, true, false));
            //
            currentSize += fileSource.Length;
            //
            if (currentSize >= maxSize)
               return currentSize;
         }
         //
         // Copy each subdirectory using recursion.
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            //
            currentSize = MoveFilesUntilSizeReached(diSourceSubDir, nextTargetSubDir, currentSize, maxSize);
            //
            if (currentSize >= maxSize)
               return currentSize;
         }
         return currentSize;
      }

      public static void MoveFiles(DirectoryInfo source, DirectoryInfo target)
      {
         //
         Directory.CreateDirectory(target.FullName);
         //
         // Get  each file into the new directory.
         foreach (FileInfo fileSource in source.GetFiles())
         {
            if (FileExtentionOkInArchive.All(s => !string.Equals(fileSource.Extension, s, StringComparison.OrdinalIgnoreCase)))
               continue;
            //
            var path = Path.Combine(target.FullName, fileSource.Name);
            //          
            ExecuteMove(new InformationCopy(fileSource, path, true, false));
            //
         }
         //
         // Copy each subdirectory using recursion.
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            //
            MoveFiles(diSourceSubDir, nextTargetSubDir);
            //
         }
      }


      public static void TestSourceAndTargetDir(DirectoryInfo source, DirectoryInfo target, List<string> fileList)
      {
         foreach (FileInfo fileSource in source.GetFiles())
         {
            var path = Path.Combine(target.FullName, fileSource.Name);
            var fileTarget = new FileInfo(path);
            if (fileTarget.Exists)
            {
               //Logger.Log("File found {0} in directory {1}", fileSource.Name, target.FullName);
               fileList.Add(fileSource.FullName);
            }
         }
         //  
         foreach (DirectoryInfo subSource in source.GetDirectories())
         {
            var path = Path.Combine(target.FullName, subSource.Name);
            DirectoryInfo nextTargetSubDir = new DirectoryInfo(path);

            if (nextTargetSubDir.Exists)
               TestSourceAndTargetDir(subSource, nextTargetSubDir, fileList);
         }
      }

      public static void CheckAndCopyOver(DirectoryInfo source, DirectoryInfo target, List<string> fileList)
      {
         foreach (FileInfo fileSource in source.GetFiles())
         {
            var path = Path.Combine(target.FullName, fileSource.Name);
            var fileTarget = new FileInfo(path);
            if (fileTarget.Exists)
            {
               fileList.Add(fileSource.FullName);
               Logger.Log("File copy {0} over {1} in directory {2}", fileSource.Name, fileTarget.Name, target.FullName);
               fileSource.CopyTo(fileTarget.FullName, true);

            }
         }
         //  
         foreach (DirectoryInfo subSource in source.GetDirectories())
         {
            var path = Path.Combine(target.FullName, subSource.Name);
            DirectoryInfo nextTargetSubDir = new DirectoryInfo(path);

            if (nextTargetSubDir.Exists)
               CheckAndCopyOver(subSource, nextTargetSubDir, fileList);
         }
      }


      public static void ListAllFileInDirectory(DirectoryInfo source, List<string> fileList, string ext)
      {
         //  
         foreach (FileInfo fileSource in source.GetFiles())
         {
            if (string.Equals(fileSource.Extension, ext, StringComparison.OrdinalIgnoreCase))
               fileList.Add(fileSource.FullName);
         }
         //  
         foreach (DirectoryInfo subSource in source.GetDirectories())
         {
            ListAllFileInDirectory(subSource, fileList, ext);
         }
      }



      public static void CompareExistAllFiles(DirectoryInfo target, List<FileInfo> fileList, List<FileInfo> fileExists)
      {
         //  
         foreach (FileInfo fileSource in target.GetFiles())
         {
            foreach (FileInfo fileInfo in fileList)
            {
               if (string.Equals(fileInfo.Name, fileSource.Name, StringComparison.OrdinalIgnoreCase))
               {
                  if (fileInfo.Length == fileSource.Length)
                     fileExists.Add(fileInfo);
               }
            }
         }
         //  
         foreach (DirectoryInfo subTarget in target.GetDirectories())
         {
            CompareExistAllFiles(subTarget, fileList, fileExists);
         }
      }


      public static void CopyOverExistAllFiles(DirectoryInfo target, List<FileInfo> fileList, List<FileInfo> fileExists)
      {
         //  
         foreach (FileInfo fileSource in target.GetFiles())
         {
            foreach (FileInfo fileInfo in fileList)
            {
               if (string.Equals(fileInfo.Name, fileSource.Name, StringComparison.OrdinalIgnoreCase))
               {
                  fileExists.Add(fileInfo);
                  if (fileInfo.Length != fileSource.Length)
                  {
                     fileInfo.CopyTo(fileSource.FullName, true);
                  }
               }
            }
         }
         //  
         foreach (DirectoryInfo subTarget in target.GetDirectories())
         {
            CopyOverExistAllFiles(subTarget, fileList, fileExists);
         }
      }

      public static void CheckExistAllFiles(DirectoryInfo target, List<FileInfo> fileList, List<FileInfo> fileExists)
      {
         //  
         foreach (FileInfo fileSource in target.GetFiles())
         {
            foreach (FileInfo fileInfo in fileList)
            {
               if (string.Equals(fileInfo.Name, fileSource.Name, StringComparison.OrdinalIgnoreCase))
                  fileExists.Add(fileInfo);
            }
         }
         //  
         foreach (DirectoryInfo subTarget in target.GetDirectories())
         {
            CheckExistAllFiles(subTarget, fileList, fileExists);
         }
      }

      public static bool SearchAnyDirectory(DirectoryInfo source, Func<DirectoryInfo, bool> fct)
      {
         // Get each file     
         if (fct(source))
         {
            return true;
         }
         // See each subdirectory using recursion.
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            //
            if (SearchAnyDirectory(diSourceSubDir, fct))
            {
               return true;
            }
            //
         }
         return false;
      }

      public static bool SearchAnyFiles(DirectoryInfo source, Func<FileInfo, bool> fct)
      {
         // Get each file  
         foreach (FileInfo fileSource in source.GetFiles())
         {
            if (fct(fileSource))
            {
               return true;
            }
         }
         // See each subdirectory using recursion.
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            //
            if (SearchAnyFiles(diSourceSubDir, fct))
            {
               return true;
            }
            //
         }
         return false;
      }


      public static bool SearchAnyFilesOkForArchive(DirectoryInfo source)
      {
         // Get each file  
         foreach (FileInfo fileSource in source.GetFiles())
         {
            if (FileExtentionOkInArchive.Any(s => !string.Equals(fileSource.Extension, s, StringComparison.OrdinalIgnoreCase)))
            {
               return true;
            }
         }
         // See each subdirectory using recursion.
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            //
            if (SearchAnyFilesOkForArchive(diSourceSubDir))
            {
               return true;
            }
            //
         }
         return false;
      }


      private static string GetBsaTempName(DirectoryInfo source, string bsaFileTargetName)
      {
         return string.Format("{0}OctaTempDir", Path.Combine(source.FullName, RemoveSpecialCharacters(bsaFileTargetName)));
      }

      public static string RemoveSpecialCharacters(string str)
      {
         StringBuilder sb = new StringBuilder();
         foreach (char c in str)
         {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
            {
               sb.Append(c);
            }
         }
         return sb.ToString();
      }

      public static string GetBsaTempDirectory(FileInfo bsaFileInfo)
      {
         string directory = Environment.CurrentDirectory;
         if (bsaFileInfo.Directory != null)
         {
            directory = bsaFileInfo.Directory.FullName;
         }
         return Path.Combine(directory, RemoveSpecialCharacters(bsaFileInfo.Name));
      }

      public static void DeleteIfFound(DirectoryInfo source, DirectoryInfo target, List<InformationFileDeletion> deletes)
      {
         //
         Directory.CreateDirectory(target.FullName);
         //
         // Get  each file into the new directory.
         foreach (FileInfo fileSource in source.GetFiles())
         {
            var path = Path.Combine(target.FullName, fileSource.Name);
            //                       
            var fileTarget = new FileInfo(path);
            //
            if (fileTarget.Exists)
            {
               deletes.Add(new InformationFileDeletion(fileTarget));
            }
            //
         }
         //
         // Copy each subdirectory using recursion.
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            //
            DeleteIfFound(diSourceSubDir, nextTargetSubDir, deletes);
         }
      }

      public static void AnalyseSourceForDeleteWhenMerge(DirectoryInfo target, Dictionary<long, InformationCopy> copies, bool verbose)
      {
         if (verbose)
         {
            Logger.Log(string.Format("Parsing {0}", target.FullName));
         }
         //
         Directory.CreateDirectory(target.FullName);
         //
         // Get  each file into the new directory.
         foreach (FileInfo fileSource in target.GetFiles())
         {
            //                                   
            var key = GetInt64HashCode(fileSource.FullName.ToUpper(CultureInfo.InvariantCulture));
            copies.Add(key, new InformationCopy(null, fileSource.FullName, true, false));
            //
         }
         //recursion
         foreach (DirectoryInfo diSourceSubDir in target.GetDirectories())
         {
            AnalyseSourceForDeleteWhenMerge(diSourceSubDir, copies, verbose);
         }
      }

      public static void CopyDirContent(DirectoryInfo source, DirectoryInfo target, Dictionary<long, InformationCopy> copies, bool checkRename)
      {
         //
         Directory.CreateDirectory(target.FullName);
         //
         // Get  each file into the new directory.
         foreach (FileInfo fileSource in source.GetFiles())
         {
            var path = Path.Combine(target.FullName, fileSource.Name);
            //                       
            var fileTarget = checkRename ? new FileInfo(GetExactPathName(path)) : new FileInfo(path);
            //
            InformationCopy previousInfoCopy;
            var key = GetInt64HashCode(path.ToUpper(CultureInfo.InvariantCulture));
            if (copies.TryGetValue(key, out previousInfoCopy))
            {
               previousInfoCopy.ChangeFileSource(fileSource, IsConfirmed(fileTarget, fileSource), checkRename && IsRenameNeeded(fileTarget, fileSource));
            }
            else
            {
               copies.Add(key, new InformationCopy(fileSource, path, IsConfirmed(fileTarget, fileSource), checkRename && IsRenameNeeded(fileTarget, fileSource)));
            }
            //
         }
         //
         // Copy each subdirectory using recursion.
         foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
         {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            //
            CopyDirContent(diSourceSubDir, nextTargetSubDir, copies, checkRename);
         }
      }

      private static bool IsConfirmed(FileInfo fileTarget, FileInfo fileSource)
      {
         return !fileTarget.Exists || fileTarget.Length != fileSource.Length;
      }

      private static bool IsRenameNeeded(FileInfo fileTarget, FileInfo fileSource)
      {
         return fileTarget.Exists && fileTarget.Length == fileSource.Length && !string.Equals(fileTarget.Name, fileSource.Name, StringComparison.Ordinal);
      }

      public static string GetExactPathName(string pathName)
      {
         if (!(File.Exists(pathName) || Directory.Exists(pathName)))
            return pathName;

         var di = new DirectoryInfo(pathName);

         if (di.Parent != null)
         {
            return Path.Combine(
               GetExactPathName(di.Parent.FullName),
               di.Parent.GetFileSystemInfos(di.Name)[0].Name);
         }
         else
         {
            return di.Name.ToUpper();
         }
      }

      public static void ExecuteMove(InformationCopy copy)
      {
         copy.FileSource.Refresh();
         if (copy.FileSource.Exists)
         {
            if (File.Exists(copy.Target))
            {
               copy.FileSource.CopyTo(copy.Target, true);
            }
            else
            {
               File.Move(copy.FileSource.FullName, copy.Target);
            }
         }
      }

      public static void ExecuteCopyOrDelete(InformationCopy copy)
      {
         if (copy.Renamed)
         {
            var temporaryName = string.Format("{0}.renaming0", copy.Target);
            File.Move(copy.Target, temporaryName);
            File.Move(temporaryName, copy.Target);
         }
         else
         {
            if (copy.FileSource != null)
            {
               copy.FileSource.Refresh();
               if (copy.FileSource.Exists)
               {
                  copy.FileSource.CopyTo(copy.Target, true);
               }
            }
            else
            {
               //int retry = 3;
               //while (true)
               //{
               //   try
               //   {       
               FileDeleteReadOnlyAttribute(new FileInfo(copy.Target));
               File.Delete(copy.Target);
               //   }
               //   catch
               //   {
               //      if (retry-- < 0)
               //         break;
               //      continue;
               //   }
               //   break;
               //}
            }
         }
      }

   }
}