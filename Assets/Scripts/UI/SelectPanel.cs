using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPanel : MonoBehaviour
{
    /// <summary>
    /// 1-Save with replacement or not.
    /// </summary>
    public int state = 0;
    [SerializeField]
    private Text text;
    [HideInInspector]
    public string bufferedArg;

    /// <summary>
    /// States:
    /// 0-Do nothing.
    /// 1-Save with replacement or not.
    /// 2-Quit editor or not.
    /// </summary>
    public void Show(string message, int state)
    {
        gameObject.SetActive(true);
        text.text = message;
        this.state = state;
    }

    public void Yes()
    {
        switch (state)
        {
            case 1:
                GameManager.instance.SaveSong(bufferedArg, true);
                break;
            case 2:
                GameManager.instance.ReturnToHome();
                break;
        }
        state = 0;
        gameObject.SetActive(false);
    }

    public void No()
    {
        switch (state)
        {
            case 1:
                GameManager.instance.SaveSong(bufferedArg, false);
                break;
            case 2:
                //Return canclled
                break;
        }
        state = 0;
        gameObject.SetActive(false);
    }

}
