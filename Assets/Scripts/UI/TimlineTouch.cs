using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimlineTouch : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private TimelineUI timeline;
    private RectTransform container;

    private void Start()
    {
        container = timeline.container;
    }

    private Vector2 PointerDataToRelativePos(PointerEventData eventData)
    {
        Vector2 result;
        Vector2 clickPosition = eventData.position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, clickPosition, null, out result);
        //result += container.sizeDelta / 2;

        return result;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        timeline.SkipAtTime(PointerDataToRelativePos(eventData).x);
    }
}
