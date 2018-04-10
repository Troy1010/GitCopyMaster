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
            if (!HookTypeExt.TryGetHook(args[0], out HookType hookType))
            {
                System.Console.WriteLine("exiting..");
                return;
            }

            System.Console.WriteLine("start..");
            switch (hookType)
            {
                case HookType.Post_Checkout:
                case HookType.Post_Commit:
                    RunLogic();
                    break;
                default:
                    System.Console.WriteLine("Defaulted.");
                    return;
            }
        }

        private static void RunLogic()
        {
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
                System.Console.WriteLine($"BranchName:{BranchName} is not master.");
                return;
            }
            // Determine sourcePath, destPath
            var curDir = new DirectoryInfo(".");
            string sourcePath = curDir.FullName;
            string destPath;
            if (Properties.Settings.Default.DestinationFolder != "")
            {
                destPath = Properties.Settings.Default.DestinationFolder;
            }
            else
            {
                var oneUpDir = new DirectoryInfo("..");
                destPath = oneUpDir.FullName;
            }
            // Create folders in destPath
            foreach (string dirPath in Directory.GetDirectories(sourcePath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                // ignore any folders with .git
                if (dirPath.Contains(".git")) continue;
                CopyFolderContents(dirPath, Path.Combine(destPath, dirInfo.Name));
            }
            // Copy Files
            foreach (string filePath in Directory.GetFiles(sourcePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                File.Copy(filePath, Path.Combine(destPath, fileInfo.Name), true);
            }
        }


        // Functions that should be in an Extention Lib

        public static void CopyFolderContents(string sourcePath, string destPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                try
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destPath));
                    System.Console.WriteLine();
                }
                catch
                {
                    throw new ArgumentException ($"Tried to copy, but failed. path:{dirPath.Replace(sourcePath, destPath)}");
                }
            }
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
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
