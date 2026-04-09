using System;
using System.IO;
using UnityEngine;

//
// File Utilities
//
internal static class GameUtils
{
    /// <summary>
    /// Maximum time an async operation can block (in ms).
    /// </summary>
    internal const int ASYNCOP_BLOCK_MAX_TIME = 30000;

    internal static string[] GetAssetPathsForDirectory(string path, string extension)
    {
        DirectoryInfo info = new("Assets/Resources/" + path);

        // get all filenames
        FileInfo[] fileNames = info.GetFiles(extension);
        /* We don't need to use a list. Since the file names processed list is going to be the same size as the file name list we can 
         * simply create an array of a fixed size and store a variable. This is likely faster than using a list */
        string[] fileNamesProcessed = new string[fileNames.Length];

        int i = 0;

        foreach (FileInfo file in fileNames)
        {
            /* Unity requires you to not use an extension */
            string processedExtension = extension.Replace("*", ""); // you have to provide windows-style wildcards
            string fileName = file.FullName.Replace(processedExtension, "");

            /* we have to lob off the absolute path */

            fileName = fileName.Substring(fileName.IndexOf("Assets", StringComparison.InvariantCultureIgnoreCase));
            fileName = fileName.Replace("Assets" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar, ""); // unity assumes /assets/resources
            fileName = fileName.Replace("\\", "/"); // unity always uses unix-style path separators
            fileNamesProcessed[i] = fileName;

            i++;
        }

        return fileNamesProcessed;  
    }

    internal static string GetNonCloneName(string name)
    {
        // this creates an implicit dependency on both unity's clone naming scheme and on GameObject.Instantiate's behaviour. need to fix
        return name.Replace("(Clone)", "");
    }

}
