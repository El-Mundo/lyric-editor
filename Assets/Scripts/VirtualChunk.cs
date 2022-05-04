using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lyrics.Game;
using Lyrics.Songs;
using UnityEngine.UI;

public class VirtualChunk : MonoBehaviour, IObjectPoolHandler<VirtualChunk>
{
    public bool active = false;
    public static readonly float TIME_OFFSET = 0.001F;

    [HideInInspector]
    public Chunk chunk;

    public bool sentenceEnd;

    [SerializeField]
    private Button button;
    [SerializeField]
    private Text text;
    [SerializeField]
    private RectTransform buttonRect;

    public bool IsAvailable()
    {
        return !active;
    }

    public void Recycle()
    {
        active = false;
        gameObject.SetActive(false);
    }

    public VirtualChunk Spawn()
    {
        gameObject.SetActive(true);
        active = true;
        return this;
    }

    public void SetChunk(Chunk chunk)
    {
        active = true;
        this.chunk = chunk;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if(chunk.unit != null)
        {
            if (chunk.eventType == 2)
            {
                //LANGUAGE
                text.text = "(歌曲信息)";
            }
            else if (chunk.eventType == 1)
            {
                text.text = "(等待)...";
            }
            else
            {
                text.text = chunk.unit.content;
            }
        }
        else
        {
            text.text = "";
        }

        float st = TimelineUI.MediaTimeToPos(chunk.startPosition);
        float en = TimelineUI.MediaTimeToPos(chunk.endPosition);
        buttonRect.anchoredPosition = new Vector2(st, buttonRect.anchoredPosition.y);
        buttonRect.sizeDelta = new Vector2(en - st, buttonRect.sizeDelta.y);
    }

    public void SelectThis()
    {
        GameManager.instance.SelectChunk(this);
    }

    public float GetEndInMediaTime()
    {
        return chunk.endPosition;
    }

}
