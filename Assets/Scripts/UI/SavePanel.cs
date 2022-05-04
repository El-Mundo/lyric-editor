using Lyrics.Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePanel : MonoBehaviour
{
    [Header("Song Info")]
    public InputField titleIndicator;
    public InputField singerIndicator, composerIndicator, writerIndicator, dateIndicator, descriptionIndicator;
    public Dropdown language;
    public Slider velocity;
    public Text velocityIndicator;
    public Text statistics;

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
        song.span = velocity.value;
    }

    public void LoadInfo(Song song)
    {
        titleIndicator.text = song.title;
        singerIndicator.text = song.singer;
        composerIndicator.text = song.composer;
        writerIndicator.text = song.songwriter;
        dateIndicator.text = song.date;
        descriptionIndicator.text = song.description;
        int lan = 0;
        switch (song.language)
        {
            case "ENG":
                lan = 0;
                break;
            case "CHN":
                lan = 1;
                break;
            case "CHN-TW":
                lan = 2;
                break;
            case "JPN":
                lan = 3;
                break;
            default:
                lan = 0;
                break;
        }
        language.value = lan;
        velocity.value = song.span;
        //LANGUAGE
        statistics.text = "Chunks: " + GameManager.instance.chunks.Count + ", Sentences: " + GameManager.instance.song.sentences.Length;
        UpdateIndicator();
    }

    public void UpdateIndicator()
    {
        //LANGUAGE
        string rounded = velocity.value + "";
        if (rounded.Contains("."))
        {
            string[] dec = rounded.Split('.');
            if(dec.Length > 1)
            {
                rounded = dec[0] + "." + (dec[1].Length > 2 ? dec[1].Substring(0, 2) : dec[1]);
            }
        }
        velocityIndicator.text = "Displayed Time: " + rounded + "sec";
    }

}
