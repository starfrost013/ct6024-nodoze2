using System.Collections.Generic;
using UnityEngine; 

// This is the same as a textasset but wrapped in our tracking system
// I would go my own way and inherit directly from unityengine.object and put postload in a base class etc but then i couldn't inherit frrom any unity behaviours
// Also, the unity TextAsset class, is partially written in c++
internal class GameAssetConfigFile : TextAsset 
{
    //
    // STRUCTS
    //

    internal struct CfgSection
    {
        internal string name;
        internal Dictionary<string, string> entries;

        public CfgSection(string newName)
        {
            name = newName;
            entries = new();
        }
    }

    // internal structure that can be retuedn
    internal struct CfgFile
    {
        // how fast IS this?
        internal List<CfgSection> sections;
    }

    //
    // FIELDS
    //

    internal string[] lines;     // The lines of the text
    internal CfgFile file;       // Internal file structure

    //
    // METHODS
    //

    public void ParseCfg()
    {
        lines = text.Split('\n');
        file = new()
        {
            sections = new()
        };

        // convert into a nicer representation. this also allows us to forgo sections since things go into a section

        CfgSection currentSection = new("Default Section");
        string trimmedLine;

        // try to write a fast loop
        foreach (string line in lines)
        {
            // skip nonsense
            if (string.IsNullOrWhiteSpace(line))
                continue;

            trimmedLine = line.Trim();

            // we guaranteed that there is at least *some* non-whitespace here 

            if (trimmedLine[0] == '[')
            {
                trimmedLine = line.Replace("]", "");
                currentSection.name = trimmedLine;
                file.sections.Add(currentSection);
                currentSection = new();
                continue; 
            }

            // any sections we need are created

            // strip away any comments
            trimmedLine = trimmedLine.Split(';')[0];

            // no equals, don't bother
            if (!trimmedLine.Contains("="))
                continue;

            string[] kv = trimmedLine.Split("=");

            if (kv.Length < 2)
                continue;

            // we don't care about non 0 or 1
            currentSection.entries.Add(kv[0], kv[1]); // and we're done
        }

        // add the default section if there are no functions
        if (file.sections.Count == 0)
            file.sections.Add(currentSection);

    }

    // I hope it's trivial
    internal string GetValue(string key)
    {
        foreach (CfgSection section in file.sections)
        {
            if (section.entries.ContainsKey(key))
                return key;
        }

        return null;

    }
}