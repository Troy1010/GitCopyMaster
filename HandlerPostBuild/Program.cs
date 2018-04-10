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
            foreach (string dirPath in Directory.GetDirectories(@"C:\Dropbox\Oblivion Project\Oblivion Pipeline\7 0 Mod Production Repo - Troy"))
            {
                if (dirPath.Contains("Oblivion - "))
                {
                    CopyFolderContents(@"C:\TMinus1010\Projects\Coding\C#\GitCopyMaster\GitCopyMaster\bin\Debug", dirPath + @"\.git\hooks", ".exe.config");
                }
            }
        }

        // Functions

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

        public static void CopyFolderContents(string sourcePath, string destPath, string exclude)
        {
            // Open
            System.Console.WriteLine($"GitCopyMaster/CopyFolderContents/Open. sourcePath:{sourcePath} destPath:{destPath}");
            // Create main destPath folder
            Directory.CreateDirectory(destPath);
            // Copy all folders
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                if (dirPath.Contains(exclude)) continue;
                System.Console.WriteLine($"GitCopyMaster/CopyFolderContents/Creating Directory: {dirPath.Replace(sourcePath, destPath)}");
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destPath));
            }
            // Copy all files
            foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                if (filePath.Contains(exclude)) continue;
                System.Console.WriteLine($"GitCopyMaster/CopyFolderContents/Copying File from: {filePath} to: {filePath.Replace(sourcePath, destPath)}");
                File.Copy(filePath, filePath.Replace(sourcePath, destPath), true);
            }
        }
    }
}
