using Lyrics.Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public Song song;

    [SerializeField]
    private Transform sleepPoint;
    [HideInInspector]
    public Vector3 sleepPos;
    public static GameManager instance;

    [HideInInspector]
    public float questionSpan = 15.0f, shootOffset = 8.0f;
    private static readonly float DEFAULT_SPAN = 15.0F, MINIMUM_SPAN = 3.0F, SPAWN_OFFSET = 0.75F;

    //MEDIA ATTRIBUTE
    public bool mediaIsVideo = false;
    public AudioSource audioPlayer;
    public VideoPlayer videoPlayer;

    private void Awake()
    {
        instance = this;
        sleepPos = sleepPoint.position;

        questionSpan = song.span > MINIMUM_SPAN ? song.span : DEFAULT_SPAN;
        shootOffset = questionSpan * SPAWN_OFFSET;
    }

    private void Update()
    {

    }
}
