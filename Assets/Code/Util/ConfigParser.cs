internal static class ConfigParser
{
    //
    // FIELDS
    //

    internal static string[] lines;     // The lines of the text
    internal static NAryDictionary<string, string, string> file;           // section,dict,pair

    //
    // METHODS
    //

    public static void Parse(string text)
    {
        lines = text.Split('\n');
        file = new();

        // convert into a nicer representation. this also allows us to forgo sections since things go into a section

        string currentSectionName = "Default Section";
        string trimmedLine;
        bool sectionsExist = false;

        // try to write a fast loop
        foreach (string line in lines)
        {
            trimmedLine = line.Trim();
            // strip away any comments
            trimmedLine = trimmedLine.Split(';')[0];

            // skip nonsense
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // we guaranteed that there is at least *some* non-whitespace here 

            if (trimmedLine[0] == '[')
            {
                trimmedLine = trimmedLine.Replace("[", "");
                trimmedLine = trimmedLine.Replace("]", "");

                currentSectionName = trimmedLine;
                file[currentSectionName] = new NAryDictionary<string, string>();
                sectionsExist = true;

                continue;
            }

            // any sections we need are created by now

            string[] kv = trimmedLine.Split("=");

            if (kv.Length < 2)
                continue;

            // we don't care about non 0 or 1
            kv[0] = kv[0].Trim();
            kv[1] = kv[1].Trim();

            file[currentSectionName][kv[0]] = kv[1];
        }

        // create a default section
        if (!sectionsExist)
            file[currentSectionName] = new NAryDictionary<string, string>();
    }

    // I hope it's trivial
    internal static string GetValue(string key)
    {
        foreach (NAryDictionary<string, string> section in file.Values)
        {
            return (section.ContainsKey(key)) ? section[key] : null;
        }

        return null;
    }

    internal static string GetValue(string section, string key)
    {
        return file[section].ContainsKey(key) ? file[section][key] : null;
    }
}