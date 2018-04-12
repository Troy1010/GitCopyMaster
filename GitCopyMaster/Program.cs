using FishingWithGit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LibGit2Sharp;

namespace GitCopyMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("GitCopyMaster/Open");
            if (!HookTypeExt.TryGetHook(args[0], out HookType hookType))
            {
                return;
            }

            switch (hookType)
            {
                case HookType.Post_Merge:
                case HookType.Post_Checkout:
                case HookType.Post_Commit:
                    if (IsCurrentRepoDirty())
                    {
                        System.Console.WriteLine("GitCopyMaster/Repo was dirty. Exiting");
                        return;
                    }
                    RunLogic();
                    break;
                default:
                    return;
            }
        }

        private static void RunLogic()
        {
            System.Console.WriteLine("GitCopyMaster/RunLogic/Logic Start");
            // Determine Branchname
            string BranchName;
            using (var repo = new Repository("."))
            {
                BranchName = repo.Head.FriendlyName;
            }
            BranchName = TrimStart(BranchName, @"origin/");
            // Filter
            if (BranchName != "master")
            {
                System.Console.WriteLine($"GitCopyMaster/RunLogic/BranchName:{BranchName} is not master. Exiting");
                return;
            }
            else
            {
                System.Console.WriteLine($"GitCopyMaster/RunLogic/BranchName:{BranchName} is a master.");
            }
            //  Determine sourcePath, destPath
            var curDir = new DirectoryInfo(".");
            string sourcePath = curDir.FullName;
            string destPath;
            if (Properties.Settings.Default.FullDestinationFolder != "")
            {
                destPath = Properties.Settings.Default.FullDestinationFolder;
            }
            else
            {
                if (Properties.Settings.Default.PartialDestinationFolder != "")
                {

                    var oneUpDir = new DirectoryInfo("..");
                    destPath = oneUpDir.FullName + "/" + Properties.Settings.Default.PartialDestinationFolder;
                }
                else
                {
                    var oneUpDir = new DirectoryInfo("..");
                    destPath = oneUpDir.FullName + "/GitCopyMaster - Default Output Folder";
                }
                destPath += "/" + curDir.Name;
            }
            System.Console.WriteLine($"GitCopyMaster/RunLogic/sourcePath:{sourcePath} . destPath:{destPath} .");
            // Delete everything at destPath
            if (Directory.Exists(destPath))
            {
                Directory.Delete(destPath, true);
            }
            // if sourcePath is empty, don't continue
            System.Console.WriteLine("GitCopyMaster/RunLogic/Region for checking if source is empty..");
            bool bEmpty = true;
            foreach (string filePath in Directory.GetFiles(sourcePath))
            {
                if (filePath.Contains(".git")) continue;
                bEmpty = false;
            }
            if (bEmpty)
            {
                System.Console.WriteLine("GitCopyMaster/RunLogic/Empty. Exiting.");
                return;
            }
            System.Console.WriteLine("GitCopyMaster/RunLogic/Not Empty. Continuing.");
            // Copy Folders with content
            System.Console.WriteLine("GitCopyMaster/RunLogic/Region for copying folders..");
            Directory.CreateDirectory(destPath);
            foreach (string dirPath in Directory.GetDirectories(sourcePath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                // ignore any folders with .git
                if (dirPath.Contains(".git")) continue;
                System.Console.WriteLine($"GitCopyMaster/RunLogic/Opening CopyFolderContents..");
                CopyFolderContents(dirPath, Path.Combine(destPath, dirInfo.Name));
            }
            System.Console.WriteLine("GitCopyMaster/RunLogic/End of region for copying folders.");
            // Copy Files
            System.Console.WriteLine("GitCopyMaster/RunLogic/Region for copying files..");
            foreach (string filePath in Directory.GetFiles(sourcePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                System.Console.WriteLine($"GitCopyMaster/RunLogic/Copying file from path: {filePath}. To path:{Path.Combine(destPath, fileInfo.Name)}");
                File.Copy(filePath, Path.Combine(destPath, fileInfo.Name), true);
            }
            System.Console.WriteLine("GitCopyMaster/RunLogic/End of region for copying files..");
        }

        // Functions
        public static bool IsCurrentRepoDirty()
        {
            using (var repo = new Repository(Directory.GetCurrentDirectory()))
            {
                return repo.RetrieveStatus().IsDirty;
            }
        }


        public static void CopyFolderContents(string sourcePath, string destPath)
        {
            // Open
            System.Console.WriteLine($"GitCopyMaster/CopyFolderContents/Open. sourcePath:{sourcePath} destPath:{destPath}");
            // Create main destPath folder
            Directory.CreateDirectory(destPath);
            // Copy all folders
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                System.Console.WriteLine($"GitCopyMaster/CopyFolderContents/Creating Directory: {dirPath.Replace(sourcePath, destPath)}");
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destPath));
            }
            // Copy all files
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                System.Console.WriteLine($"GitCopyMaster/CopyFolderContents/Copying File from: {newPath} to: {newPath.Replace(sourcePath, destPath)}");
                File.Copy(newPath, newPath.Replace(sourcePath, destPath), true);
            }
        }

        public static bool TryTrimStart(string src, string item, out string result)
        {
            if (!src.StartsWith(item))
            {
                result = src;
                return false;
            }
            if (src.Length == item.Length)
            {
                result = string.Empty;
                return true;
            }
            result = src.Substring(item.Length);
            return true;
        }

        public static string TrimStart(string src, string item)
        {
            TryTrimStart(src, item, out string result);
            return result;
        }
    }
}
