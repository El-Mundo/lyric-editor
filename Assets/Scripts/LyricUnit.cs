using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Lyrics.Songs;
using Lyrics.Game;

public class LyricUnit : MonoBehaviour, IObjectPoolHandler<LyricUnit>, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public Unit unit;
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text text;
    [SerializeField]
    private Image icon;
    private Image panel;
    private RectTransform buttonRect;

    /// <summary>
    /// <list type="table">
    /// <item><description>0-Sleeping in pool</description></item>
    /// <item><description>1-Running in game panel</description></item>
    /// <item><description>2-Running but disabled</description></item>
    /// </list>
    /// </summary>
    private int state;
    private Vector3 vel;
    private float leftBorder, rightBorder, canvasScale;
    private float disappearX;

    public Question question;
    private string answer;

    void Start()
    {
        state = 0;

        /*buttonRect = button.GetComponent<RectTransform>();
        panel = button.GetComponent<Image>();

        leftBorder = GameManager.instance.leftBorder.position.x;
        rightBorder = GameManager.instance.rightBorder.position.x;

        vel = new Vector3((leftBorder - rightBorder) / GameManager.instance.questionSpan, 0, 0);
        transform.position = GameManager.instance.sleepPosition.position;*/
    }

    void Update()
    {
        /*switch (state)
        {
            case 1:
            case 2:
                disappearX = leftBorder - GetWidth();
                transform.position += vel * Time.deltaTime;
                if(transform.position.x < disappearX)
                {
                    Recycle();
                }
                break;
        }*/
    }

    /*public float GetWidth()
    {
        return GameManager.instance.canvasRect.localScale.x * LayoutUtility.GetPreferredWidth(buttonRect);
    }*/

    public void Shoot(Unit unit, Question question)
    {
        this.unit = unit;
        text.text = unit.content;
        this.question = question;
        this.answer = unit.content;
        button.interactable = true;
        //ResourcePreloader r = ResourcePreloader.instance;
        //icon.sprite = r.GetQuestionIcon(question.colour);
        //panel.color = r.GetRenderColour(question.colour);

        state = 1;
    }

    public void DisableInScene()
    {
        state = 2;
        button.interactable = false;
    }

    public bool IsRightAnswer()
    {
        return question.IsRightAnswer(this.answer);
    }

    public void SetFont(Font font)
    {
        text.font = font;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Unity's Pointer interface is not influenced by button's property, so manually set this.
        if (!button.interactable) return;
        //GameManager.instance.SelectLyricUnit(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //GameManager.instance.ReleaseLyricUnit(this);
    }

    public LyricUnit Spawn()
    {
        return this;
    }

    public bool IsAvailable()
    {
        return state == 0;
    }

    public void Recycle()
    {
        state = 0;
        button.interactable = false;
        //transform.position = GameManager.instance.sleepPosition.position;
        //A sleeping unit cannot be selected, so attempt to release it.
        //GameManager.instance.ReleaseLyricUnit(this);
    }

}
