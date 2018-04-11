using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandlerPostBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string dirPath in Directory.GetDirectories(Properties.Settings.Default.DestinationDir))
            {
                if (dirPath.Contains(Properties.Settings.Default.Filter))
                {
                    var curDir = new DirectoryInfo(".");
                    string sourcePath = curDir.FullName;
                    CopyFolderContents(sourcePath + @"\..\..\..\GitCopyMaster\bin\Debug", dirPath + @"\.git\hooks", Properties.Settings.Default.Exclude);
                }
            }
        }

        // Functions

        public static void CopyFolderContents(string sourcePath, string destPath)
        {
            CopyFolderContents(sourcePath, destPath, "");
        }

        public static void CopyFolderContents(string sourcePath, string destPath, string exclude)
        {
            // Open
            System.Console.WriteLine($"GitCopyMaster/CopyFolderContents/Open. sourcePath:{sourcePath} destPath:{destPath}");
            // Create main directory
            Directory.CreateDirectory(destPath);
            // Copy directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                if (exclude !="" && dirPath.Contains(exclude)) continue;
                System.Console.WriteLine($"GitCopyMaster/CopyFolderContents/Creating Directory: {dirPath.Replace(sourcePath, destPath)}");
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destPath));
            }
            // Copy files
            foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                if (exclude != "" && filePath.Contains(exclude)) continue;
                System.Console.WriteLine($"GitCopyMaster/CopyFolderContents/Copying File from: {filePath} to: {filePath.Replace(sourcePath, destPath)}");
                File.Copy(filePath, filePath.Replace(sourcePath, destPath), true);
            }
        }
    }
}
