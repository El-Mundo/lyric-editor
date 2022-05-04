using System.Collections;
using Lyrics.Songs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditArea : MonoBehaviour
{
    public Scrollbar viewScroll;

    public InputField lyric, notation, startTime, endTime;
    public Scrollbar r, g, b;
    public Color selectedColour;
    public Toggle forceQuestion, longChunk;
    public Slider eventCode;
    public Image colorIndicator;
    public Text eventIndicator;
    public Toggle sentenceEnd;

    public VirtualChunk selectedChunk;
    private bool canUpdate = true;

    public void ResetView()
    {
        viewScroll.value = 1;
    }

    public void LoadChunk(VirtualChunk chunk)
    {
        canUpdate = false;

        Chunk c = chunk.chunk;
        lyric.text = c.unit.content;
        notation.text = c.unit.notation;
        startTime.text = c.startPosition + "";
        endTime.text = c.endPosition + "";

        bool hasColour = ColorUtility.TryParseHtmlString(c.colorCode, out selectedColour);
        if (!hasColour)
        {
            selectedColour = new Color(Song.DEFAULT_PLAYED_COLOUR.r, Song.DEFAULT_PLAYED_COLOUR.g, Song.DEFAULT_PLAYED_COLOUR.b);
        }

        forceQuestion.isOn = c.forceQuestion;
        longChunk.isOn = c.longChunk;
        eventCode.value = c.eventType;
        r.value = selectedColour.r;
        g.value = selectedColour.g;
        b.value = selectedColour.b;
        UpdateEveneIndicator();
        colorIndicator.color = selectedColour;

        sentenceEnd.isOn = chunk.sentenceEnd;

        canUpdate = true;

        selectedChunk = chunk;
    }

    public void UpdateEveneIndicator()
    {
        string info = "";
        switch ((int)eventCode.value)
        {
            //LANGUAGE
            case 0:
                info = "no event";
                break;
            case 1:
                info = "waiting";
                break;
            case 2:
                info = "song info";
                break;
            default:
                info = "effect#" + ((int)eventCode.value - 3);
                break;
        }
        eventIndicator.text = (int)eventCode.value + "(" + info + ")";

        if (!canUpdate) return;

        if (selectedChunk != null)
        {
            selectedChunk.chunk.eventType = (int)eventCode.value;
            selectedChunk.UpdateDisplay();
        }
    }

    public void UpdateColorIndicator()
    {
        if (!canUpdate) return;

        selectedColour = new Color(r.value, g.value, b.value);
        colorIndicator.color = selectedColour;
        
        if (selectedChunk != null)
        {
            selectedChunk.chunk.colorCode = "#" + ColorUtility.ToHtmlStringRGBA(selectedColour);
        }
    }

    public void ChangeTimePosition()
    {
        if (!canUpdate) return;

        float start = -1;
        float end = -1;

        try
        {
            start = float.Parse(startTime.text);
            end = float.Parse(endTime.text);
        }
        catch
        {
            //LANGUAGE
            GameManager.instance.Alert("Please input a correct number.");
            return;
        }

        GameManager.instance.SetSelectedChunkTime(start, end);
    }

    public void ChangeLyric()
    {
        if (!canUpdate) return;
        if (selectedChunk != null)
        {
            selectedChunk.chunk.unit.content = lyric.text;
            selectedChunk.UpdateDisplay();
        }
    }

    public void ChangeNotation()
    {
        if (!canUpdate) return;
        if(selectedChunk != null)
        {
            selectedChunk.chunk.unit.notation = notation.text;
        }
    }

    public void ReturnDefaultColour()
    {
        selectedColour = new Color(Song.DEFAULT_PLAYED_COLOUR.r, Song.DEFAULT_PLAYED_COLOUR.g, Song.DEFAULT_PLAYED_COLOUR.b);
        canUpdate = false;
        r.value = selectedColour.r;
        g.value = selectedColour.g;
        b.value = selectedColour.b;
        canUpdate = true;
        colorIndicator.color = selectedColour;

        if (selectedChunk != null)
        {
            selectedChunk.chunk.colorCode = "#" + ColorUtility.ToHtmlStringRGBA(selectedColour);
            SongLoader.DEBUG_PrintChunkInfo(selectedChunk.chunk);
        }
    }

    public void DeleteChunk()
    {
        GameManager.instance.DeleteSelectedChunk();
    }

    public void ForceQuestion()
    {
        if (!canUpdate) return;
        selectedChunk.chunk.forceQuestion = forceQuestion.isOn;
    }

    public void LongChunk()
    {
        if (!canUpdate) return;
        selectedChunk.chunk.longChunk = longChunk.isOn;
    }

    public void SentenceEnd()
    {
        if (!canUpdate) return;
        selectedChunk.sentenceEnd = sentenceEnd.isOn;
    }

    public void AlignChunkToLeft()
    {
        GameManager.instance.AlignSelectedChunk();
        startTime.text = selectedChunk.chunk.startPosition + "";
    }

}
