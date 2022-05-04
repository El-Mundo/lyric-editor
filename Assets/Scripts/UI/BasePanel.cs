using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    public Image musicIcon;
    public RawImage videoRenderTarget;

    public Image playMediaButton;
    [SerializeField]
    private Sprite playingIcon, pausedIcon;

    public TimelineUI timeline;
    public Text timeIndicator;
    public Scrollbar timelineScroll;
    [SerializeField]
    private RectTransform basePanelRect;

    public Scrollbar timelineScaler;
    private readonly static float TIMELINE_SC_MIN = 10.0F, TIMELINE_SC_MAX = 120.0F;
    private readonly static float TIMELINE_SC_RANGE = TIMELINE_SC_MAX - TIMELINE_SC_MIN;

    public void InitBasePanel(bool mediaIsVideo)
    {
        if (mediaIsVideo)
        {
            videoRenderTarget.gameObject.SetActive(true);
            musicIcon.gameObject.SetActive(false);
        }
        else
        {
            videoRenderTarget.gameObject.SetActive(false);
            musicIcon.gameObject.SetActive(true);
        }
        UpdateTimelineScaler();
    }

    public void SetPlaybackPosition(float playbackPos, float mediaLength)
    {
        timeline.SetCurrentPosition(playbackPos, mediaLength);
        int lMin = (int)mediaLength / 60;
        int pMin = (int)playbackPos / 60;
        float lSec = mediaLength % 60;
        float pSec = playbackPos % 60;
        timeIndicator.text = pMin + ":" + PrintAsTimerFormat("" + pSec) + "/" + lMin + ":" + PrintAsTimerFormat("" + lSec);
    }

    private string PrintAsTimerFormat(string sec)
    {
        if (sec.Contains("."))
        {
            string[] dec = sec.Split('.');
            if(dec.Length > 0)
            {
                return dec[0] + ":" + (dec[1].Length > 1 ? dec[1].Substring(0, 2) : dec[1] + "0");
            }
            else
            {
                return dec[0] + ":00";
            }
        }
        else
        {
            return sec + ":00";
        }
    }

    public void SetMediaButtonState(bool isPlaying)
    {
        if (!isPlaying)
        {
            playMediaButton.sprite = playingIcon;
        }
        else
        {
            playMediaButton.sprite = pausedIcon;
        }
    }

    public void ForceUpdatePreview(UnityEngine.Video.VideoPlayer v)
    {
        if(v.texture != null)
        {
            videoRenderTarget.texture = v.texture;
        }
    }

    public void InitTimelineUI()
    {
        timeline.InitDisplay(GameManager.instance.GetMediaLength());
    }

    public void RescaleTimelineUI()
    {
        float scale = timelineScaler.value * TIMELINE_SC_RANGE + TIMELINE_SC_MIN;
        timeline.Rescale(scale, GameManager.instance.GetMediaLength(), GameManager.instance.GetMediaTime());
    }

    private void UpdateTimelineScaler()
    {
        timelineScaler.value = (TimelineUI.timelineScale - TIMELINE_SC_MIN) / TIMELINE_SC_RANGE;
    }

    public void LookAtPlayHead()
    {
        float pos = timeline.GetCurrentPosition();
        float timeWidth = timeline.GetTimelineWidth();
        float panelWidth = basePanelRect.rect.width;
        float progress = (pos - panelWidth * 0.5f) / (timeWidth - panelWidth);
        if (progress < 0.0f) progress = 0;
        else if (progress > 1.0f) progress = 1;
        timelineScroll.value = progress;
    }

    public void SetEntry()
    {
        timeline.SetEntry(timeline.GetCurrentPosition());
    }

    public float GetEntryAsMediaTime()
    {
        return timeline.GetEntryAsMediaTime();
    }

    public void SetExit()
    {
        timeline.SetExit(timeline.GetCurrentPosition());
    }

    public float GetExitAsMediaTime()
    {
        return timeline.GetExitAsMediaTime();
    }

}
