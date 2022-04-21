using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    public UIPanel parent;
    //[HideInInspector]
    public List<UIPanel> children = new List<UIPanel>();
    public List<UIPanel> defaultChildren = new List<UIPanel>();
    private bool active;
    public bool autoStart;
    public bool oneChildVisible;
    private Vector3 localPos;

    public int DefaultChildrenSize()
    {
        return defaultChildren.Count;
    }

    private void Awake()
    {
        SetParent(parent);
    }

    private void Start()
    {
        localPos = transform.localPosition;
        SetActive(false);
    }

    public void SetActive(bool active)
    {
        this.active = active;
        if (active == false)
        {
            foreach (UIPanel panel in children)
            {
                panel.SetActive(false);
            }
            Inactivate();
        }
        else
        {
            Activate();
            foreach (UIPanel panel in defaultChildren)
            {
                panel.SetActive(true);
            }
            if(parent != null)
            {
                if (parent.oneChildVisible)
                {
                    parent.InactivateOtherChildren(this);
                }
            }
        }
    }

    public void InactivateOtherChildren(UIPanel keptChild)
    {
        foreach(UIPanel panel in children)
        {
            if (panel == keptChild) continue;
            else panel.SetActive(false);
        }
    }

    void Inactivate()
    {
        transform.position = GameManager.instance.sleepPos;
        //gameObject.SetActive(false);
    }

    void Activate()
    {
        transform.localPosition = localPos;
        //gameObject.SetActive(true);
    }

    public bool IsActive()
    {
        return active;
    }

    public void AddDefaultChildren(UIPanel child)
    {
        defaultChildren.Add(child);
    }

    public void SetParent(UIPanel parent)
    {
        if (parent == null) return;

        if (this.parent.children.Contains(this))
        {
            this.parent.children.Remove(this);
        }
        if (this.parent.defaultChildren.Contains(this))
        {
            this.parent.defaultChildren.Remove(this);
        }
        this.parent = parent;
        if (!parent.children.Contains(this))
        {
            parent.children.Add(this);
        }

        if (autoStart && !parent.defaultChildren.Contains(this))
        {
            parent.defaultChildren.Add(this);
        }
    }

}
