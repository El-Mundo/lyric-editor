using Lyrics.Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MediaPanel : MonoBehaviour
{
    [Header("Media")]
    public InputField pathIndicator;
    [Header("Song Info")]
    public InputField titleIndicator;
    public InputField singerIndicator, composerIndicator, writerIndicator, dateIndicator, descriptionIndicator;
    public Dropdown language;

    public void SetPath(string path)
    {
        pathIndicator.text = path;
    }

    public string GetMediaPath()
    {
        return pathIndicator.text;
    }

    public void UpdateSongInfo(Song song)
    {
        song.title = titleIndicator.text;
        song.singer = singerIndicator.text;
        song.composer = composerIndicator.text;
        song.songwriter = writerIndicator.text;
        song.date = dateIndicator.text;
        song.description = descriptionIndicator.text;
        string lan = "";
        switch (language.value)
        {
            case 0:
                lan = "ENG";
                break;
            case 1:
                lan = "CHN";
                break;
            case 2:
                lan = "CHN-TW";
                break;
            case 3:
                lan = "JPN";
                break;
            default:
                lan = "UNKNOWN";
                break;
        }
        song.language = lan;
    }

}
