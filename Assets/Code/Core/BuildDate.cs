// i stole this from the internet

using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

public class BuildDate : MonoBehaviour

#if UNITY_EDITOR
, IPreprocessBuildWithReport
#endif
{
    public const string BUILD_DATE_PATH = "BuildDate";

    private TextAsset BuildDateText;

    public string buildDate => BuildDateText.text;

#if UNITY_EDITOR
    public int callbackOrder => 0; // must be => because it is a *PROPERTY*

    public void OnPreprocessBuild(BuildReport report)
    {
        string buildDate = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");

        Debug.Log("Build Date: " + buildDate);

        File.WriteAllText("Assets/Resources/" + BUILD_DATE_PATH + ".txt", buildDate);

        AssetDatabase.Refresh();
    }
#endif
}
