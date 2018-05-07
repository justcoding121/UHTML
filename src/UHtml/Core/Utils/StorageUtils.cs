using PCLStorage;
using System;
using System.IO;
using FileNotFoundException = PCLStorage.Exceptions.FileNotFoundException;

namespace UHtml.Core.Utils
{
    internal static class StorageUtils
    {
        internal static IFolder GetStorageFolder()
        {
            return IocModule.Container.GetInstance<IFolder>();
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
            var filePath = Path.Combine(GetStorageFolder().Path, src);
            var file = FileSystem.Current.GetFileFromPathAsync(filePath).Result;

            return file != null;
        }

        internal static void Move(string src, string dst)
        {
            if (!FileExists(src))
            {
                throw new FileNotFoundException(src);
            }

            if (!FileExists(dst))
            {
                throw new FileNotFoundException(dst);
            }

            var sourceFile = FileSystem.Current.GetFileFromPathAsync(src).Result;
            sourceFile.MoveAsync(dst).Wait();
        }
    }
}
