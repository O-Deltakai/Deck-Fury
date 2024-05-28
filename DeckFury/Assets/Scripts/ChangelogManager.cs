using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class ChangelogEntry
{
    public string type;
    public string description;
}

[Serializable]
public class ChangelogVersion
{
    public string version;
    public string date;
    public List<ChangelogEntry> entries;
}

[Serializable]
public class Changelog
{
    public List<ChangelogVersion> changelog;
}


public class ChangelogManager : MonoBehaviour
{
    [SerializeField] TextAsset changelogFile;
    [SerializeField] TextMeshProUGUI changelogText;


    // Start is called before the first frame update
    void Start()
    {
        if(changelogFile == null)
        {
            Debug.LogError("Changelog file is not assigned!");
            return;
        }

        DisplayChangelog(changelogFile.text);

    }

    void DisplayChangelog(string json)
    {
        Changelog changelog = JsonUtility.FromJson<Changelog>(json);
        changelogText.text = FormatChangeLog(changelog);
    }

    string FormatChangeLog(Changelog changelog)
    {
        string formattedChangelog = "";

        foreach (var version in changelog.changelog)
        {
            formattedChangelog += $"<b>Version: {version.version}</b>\n";
            formattedChangelog += $"<i>Date: {version.date}</i>\n";
            foreach (var entry in version.entries)
            {
                formattedChangelog += $"• {entry.type}\n";
                formattedChangelog += $"    - {entry.description}\n\n";
            }
            formattedChangelog += "\n\n";  // Add some space between versions
        }

        return formattedChangelog;        
    }


}
