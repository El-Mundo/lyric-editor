using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lyrics.Game;

public class TimeDegreeDisplay : MonoBehaviour, IObjectPoolHandler<TimeDegreeDisplay>
{
    private bool active = false;
    public Text text;
    [SerializeField]
    private RectTransform thisRect;

    public bool IsAvailable()
    {
        return !active;
    }

    public void Recycle()
    {
        active = false;
        gameObject.SetActive(false);
    }

    public TimeDegreeDisplay Spawn()
    {
        return this;
    }

    public void Shoot(int sec)
    {
        thisRect.anchoredPosition = new Vector2(TimelineUI.MediaTimeToPos(sec), 0);

        int min = sec / 60;
        sec = sec % 60;
        text.text = min + ":" + sec;

        gameObject.SetActive(true);
    }

}
