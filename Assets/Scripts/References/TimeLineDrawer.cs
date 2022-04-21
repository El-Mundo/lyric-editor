using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class TimeLineDrawer : MonoBehaviour
{
#if UNITY_EDITOR
    private void DrawfraemLine(Rect rect)
    {
        Handles.BeginGUI();
        Handles.color = new Color(0, 1, 0, 0.4f);
        float step = 8;
        int Index = 0;
        for (float i = rect.x + 2; i < rect.width; i += step)
        {
            if (Index % 5 == 0)
            {
                Handles.DrawLine(new Vector3(i, rect.y + rect.height - 20), new Vector3(i, rect.y + rect.height - 5));
                string str = Index.ToString();
                if (str.Length > 2)
                {
                    GUI.Label(new Rect(i - 15, rect.y + 12, 30, 12), str);
                }
                else if (str.Length > 1)
                {
                    GUI.Label(new Rect(i - 10, rect.y + 12, 20, 12), str);
                }
                else
                {
                    GUI.Label(new Rect(i - 5, rect.y + 12, 12, 12), str);
                }
                 
            }
            else
            {
                Handles.DrawLine(new Vector3(i, rect.y + rect.height - 15), new Vector3(i, rect.y + rect.height - 10));
            }
            Index++;

        }

        Handles.EndGUI();
    }
#endif
}
