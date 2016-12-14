using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHtml.Core.Utils
{
    internal static class StorageUtils
    {
        internal static IFolder GetStorageFolder()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            // create a folder, if one does not exist already
            IFolder folder = rootFolder.CreateFolderAsync("UHtml", CreationCollisionOption.OpenIfExists).Result;

            return folder;
        }

        /// <summary>
        /// creates a temporary file and returns its path
        /// </summary>
        /// <returns></returns>
        internal static string GetTempFileName()
        {
            var storageFolder = GetStorageFolder();
            var tempFolder = storageFolder.CreateFolderAsync("Temp", CreationCollisionOption.OpenIfExists).Result;
            var fileName = Guid.NewGuid().ToString();
            var tmpFilePath = PortablePath.Combine(tempFolder.Path, fileName + ".tmp");
            tempFolder.CreateFileAsync(tmpFilePath, CreationCollisionOption.ReplaceExisting).Wait();

            return tmpFilePath;
        }

        internal static string GetTempPath()
        {
            var storageFolder = GetStorageFolder();
            var tempFolder = storageFolder.CreateFolderAsync("Temp", CreationCollisionOption.OpenIfExists).Result;
            return tempFolder.Path;
        }

        internal static void CreateTempDirIfNotExists(string tempDirName)
        {
            var root = GetStorageFolder();
            var tempFolder = root.CreateFolderAsync("Temp", CreationCollisionOption.OpenIfExists).Result;

            var exists = tempFolder.CheckExistsAsync(tempDirName).Result;
            if (exists != ExistenceCheckResult.FolderExists)
            {
                tempFolder.CreateFolderAsync(tempDirName, CreationCollisionOption.OpenIfExists).Wait();
            }
        }

        internal static bool FileExists(string src)
        {
            return FileSystem.Current.GetFileFromPathAsync(src).Result!=null;
        }

        internal static void Move(string src, string dst)
        {
            var sourceFile = FileSystem.Current.GetFileFromPathAsync(src).Result;
            sourceFile.MoveAsync(dst).Wait();
        }
    }
}
