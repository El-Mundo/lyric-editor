using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertPanel : MonoBehaviour
{
    [SerializeField]
    private Text text;
    [SerializeField]
    private Button cancel;
    /// <summary>A fatal error forces the application to quit after showing this message.</summary>
    private bool fatalError;

    void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    public void Alert(string message)
    {
        fatalError = false;
        gameObject.SetActive(true);
        text.text = message;
        cancel.interactable = true;
    }

    public void Alert(string message, bool canCancel)
    {
        fatalError = false;
        gameObject.SetActive(true);
        text.text = message;
        cancel.interactable = canCancel;
    }

    public void AlertFatal(string message)
    {
        fatalError = true;
        gameObject.SetActive(true);
        text.text = message;
        cancel.interactable = true;
    }

    public string GetMessage()
    {
        return text.text;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (fatalError)
        {
            GameManager.instance.ReturnToHome();
        }
    }

}
