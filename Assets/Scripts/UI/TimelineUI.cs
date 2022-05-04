using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineUI : MonoBehaviour
{
    public Image currentPosition;
    private RectTransform currentPosRect;
    public RectTransform container;
    public RectTransform timeDegreeParent;

    public RectTransform entryPoint, exitPoint;

    public static float timelineScale = 10.0f;
    public bool inited = false;

    private ObjectPool<TimeDegreeDisplay> degreePool;

    [SerializeField]
    private GameObject timeDegreePrefeb;
    private readonly static int SEC_INTERVAL = 15, SHORT_INTERVAL = 5;

    void Awake()
    {
        currentPosRect = currentPosition.GetComponent<RectTransform>();

        degreePool = new ObjectPool<TimeDegreeDisplay>(timeDegreeParent, timeDegreePrefeb);
    }

    public void InitDisplay(float mediaLength)
    {
        if (!inited)
        {
            container.sizeDelta = new Vector2(mediaLength * timelineScale, container.sizeDelta.y);
            UpdateTimeDegrees();
            currentPosRect.anchoredPosition = new Vector2(0, 0);
            inited = true;
        }
    }

    public void SetCurrentPosition(float position, float length)
    {
        currentPosRect.anchoredPosition = new Vector2(position * timelineScale, currentPosRect.anchoredPosition.y);
    }

    public float GetCurrentPosition()
    {
        return currentPosRect.anchoredPosition.x;
    }

    public float GetTimelineWidth()
    {
        return container.sizeDelta.x;
    }

    public void Rescale(float scale, float mediaLength, float position)
    {
        float entryTime = TouchPosToMediaTime(entryPoint.anchoredPosition.x);
        float exitTime = TouchPosToMediaTime(exitPoint.anchoredPosition.x);
        
        timelineScale = scale;
        container.sizeDelta = new Vector2(mediaLength * timelineScale, container.sizeDelta.y);
        UpdateTimeDegrees();
        SetCurrentPosition(position, mediaLength);

        SetEntry(MediaTimeToPos(entryTime));
        SetExit(MediaTimeToPos(exitTime));

        GameManager.instance.ForceUpdateAllChunks();
        GameManager.instance.LookAtPlayHead();
    }

    public static float TouchPosToMediaTime(float localX)
    {
        return localX / timelineScale;
    }

    public static float MediaTimeToPos(float time)
    {
        return time * timelineScale;
    }

    public void SkipAtTime(float localX)
    {
        GameManager.instance.SetMediaTime(TouchPosToMediaTime(localX));
    }

    private void UpdateTimeDegrees()
    {
        degreePool.RecycleAll();
        int interval = timelineScale > 60 ? SHORT_INTERVAL : SEC_INTERVAL;
        int degNum = (int)(GameManager.instance.GetMediaLength() / interval);
        TimeDegreeDisplay[] d = degreePool.SpawnMultipleWithPrefab(timeDegreeParent, degNum);
        for (int i = 0; i < degNum; i++)
        {
            d[i].Shoot(i * interval + interval);
        }
    }

    public void SetEntry(float pos)
    {
        entryPoint.anchoredPosition = new Vector2(pos, entryPoint.anchoredPosition.y);
    }

    public float GetEntryAsMediaTime()
    {
        return TouchPosToMediaTime(entryPoint.anchoredPosition.x);
    }

    public void SetExit(float pos)
    {
        exitPoint.anchoredPosition = new Vector2(pos, entryPoint.anchoredPosition.y);
    }

    public float GetExitAsMediaTime()
    {
        return TouchPosToMediaTime(exitPoint.anchoredPosition.x);
    }

}
