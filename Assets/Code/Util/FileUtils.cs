using System;
using System.IO;
using UnityEngine;

//
// File Utilities
//
internal static class FileUtils
{
    internal static string[] GetAssetPathsForDirectory(string path, string extension)
    {
        DirectoryInfo info = new("Assets/Resources/Cars");

        // get all filenames
        FileInfo[] fileNames = info.GetFiles("*.txt");
        /* We don't need to use a list. Since the file names processed list is going to be the same size as the file name list we can 
         * simply create an array of a fixed size and store a variable. This is likely faster than using a list */
        string[] fileNamesProcessed = new string[fileNames.Length];

        int i = 0;

        foreach (FileInfo file in fileNames)
        {
            /* Unity requires you to not use an extension */

            string fileName = file.FullName.Replace(extension, "");
            fileNamesProcessed[i] = fileName;

            i++;
        }

        return fileNamesProcessed;  
    }
}
