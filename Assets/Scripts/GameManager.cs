using Lyrics.Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
    public bool mediaPlaying = false;
    /*/// <summary>Enable this will force to load video file in next frame.</summary>
    private bool loadVideoFlag = false;*/
    private bool mediaLoaded = false;

    public MediaPanel mediaPanel;
    public BasePanel basePanel;
    public AlertPanel alertPanel;
    public SelectPanel selectPanel;
    public SavePanel savePanel;
    public EditArea editArea;
    public ChunkPanel chunkPanel;
    
    [Header("Chunks")]
    public List<VirtualChunk> chunks;
    public ObjectPool<VirtualChunk> chunkPool;
    [SerializeField]
    private GameObject chunkPrefeb;
    [SerializeField]
    private Transform chunkParent;

    public VirtualChunk selectedChunk;

    private void Awake()
    {
        instance = this;
        SongLoader.ClearCache();

        chunkPool = new ObjectPool<VirtualChunk>(chunkParent, chunkPrefeb);

        sleepPos = sleepPoint.position;
        basePanel.gameObject.SetActive(false);

        questionSpan = song.span > MINIMUM_SPAN ? song.span : DEFAULT_SPAN;
        shootOffset = questionSpan * SPAWN_OFFSET;
    }

    private void Update()
    {
        if (mediaLoaded)
        {
            if (IsMediaPlaying())
            {
                if (!mediaPlaying)
                {
                    mediaPlaying = true;
                    basePanel.SetMediaButtonState(true);
                }
                basePanel.SetPlaybackPosition(GetMediaTime(), GetMediaLength());
                LookAtPlayHead();
            }
            else
            {
                if (mediaPlaying)
                {
                    mediaPlaying = false;
                    basePanel.SetMediaButtonState(false);
                }
            }
        }
        /*else
        {
            if (loadVideoFlag)
            {
                Alert("Loading video file...", false);
                videoPlayer.Prepare();
                loadVideoFlag = false;
            }
        }*/
    }

    public void Alert(string message)
    {
        alertPanel.Alert(message);
    }

    public void Alert(string message, bool canCancel)
    {
        alertPanel.Alert(message, canCancel);
    }

    public void AlertFatal(string message)
    {
        alertPanel.AlertFatal(message);
    }

    public void SetMediaTime(float second)
    {
        if (second < 0) second = 0;
        else if (second > GetMediaLength()) second = GetMediaLength();

        if (mediaIsVideo)
        {
            videoPlayer.time = second;
        }
        else
        {
            audioPlayer.time = second;
        }
        basePanel.SetPlaybackPosition(second, GetMediaLength());
    }

    public bool IsMediaPlaying()
    {
        if (mediaIsVideo)
        {
            return videoPlayer.isPlaying;
        }
        else
        {
            return audioPlayer.isPlaying;
        }
    }

    public float GetMediaTime()
    {
        if (mediaIsVideo)
        {
            return (float)videoPlayer.time;
        }
        else
        {
            return audioPlayer.time;
        }
    }

    public float GetMediaLength()
    {
        if (mediaIsVideo)
        {
            return (float)videoPlayer.length;
        }
        else
        {
            return audioPlayer.clip.length;
        }
    }

    public void InitNewSongInfo()
    {
        mediaPanel.UpdateSongInfo(song);
    }

    public void PlayMedia()
    {
        if (mediaIsVideo)
        {
            videoPlayer.Play();
        }
        else
        {
            audioPlayer.Play();
        }
        mediaPlaying = true;
        basePanel.SetMediaButtonState(true);
    }

    public void PauseMedia()
    {
        if (mediaIsVideo)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
        }
        else
        {
            if (audioPlayer.isPlaying)
            {
                audioPlayer.Pause();
            }
        }
        mediaPlaying = false;
        basePanel.SetMediaButtonState(false);
    }

    public void EnterProject()
    {
        mediaIsVideo = song.fileType == 3;
        try { 
            if (mediaIsVideo)
            {
                string flag = "file://";
                if(SystemInfo.deviceType != DeviceType.Desktop) {
                    flag = "";
                }

                videoPlayer.url = flag + SongLoader.loadedMediaPath;
                //loadVideoFlag = true;
                videoPlayer.prepareCompleted += VideoPreparedEvent;
                videoPlayer.errorReceived += VideoErrorEvent;
                Alert("Loading video file...", false);
                videoPlayer.Prepare();
            }
            else
            {
                PlayExternalAudioFile("file://" + SongLoader.loadedMediaPath, song.fileType);
                mediaLoaded = true;
            }
        }
        catch(System.Exception e)
        {
            //LANGUAGE
            Alert("Failed to load media,error message:\n" + e.Message);
            return;
        }

        mediaPanel.gameObject.SetActive(false);
        basePanel.gameObject.SetActive(true);
        basePanel.InitBasePanel(mediaIsVideo);
        if (!mediaIsVideo) basePanel.InitTimelineUI();
    }

    private void VideoPreparedEvent(VideoPlayer s)
    {
        basePanel.InitTimelineUI();
        //basePanel.ForceUpdatePreview(videoPlayer);
        mediaLoaded = true;
        if (alertPanel.GetMessage().Equals("Loading video file..."))
        {
            alertPanel.Close();
        }
        s.Play();
        s.Pause();
        Debug.Log(s.length);
    }

    private void VideoErrorEvent(VideoPlayer s, string m)
    {
        //LANGUAGE
        AlertFatal("Video file has been corrupted, error message:\n" + m);
    }

    private void PlayExternalAudioFile(string filePath, int fileType)
    {
        AudioType type = fileType == 0 ? AudioType.MPEG : fileType == 1 ? AudioType.OGGVORBIS :
            fileType == 2 ? AudioType.WAV : AudioType.UNKNOWN;
        var www = UnityWebRequestMultimedia.GetAudioClip(filePath, type);
        www.SendWebRequest();
        while (!www.isDone)
        {
            //Wait for load ending
        }
        audioPlayer.clip = DownloadHandlerAudioClip.GetContent(www);
    }

    public void ReturnToHome()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Home");
    }

    public void SaveSong(string path)
    {
        if (!path.EndsWith(".imv4")) path = path + ".imv4";

        if (System.IO.File.Exists(path))
        {
            selectPanel.bufferedArg = path;
            //LANGUAGE
            selectPanel.Show("Duplicate filename has been detected.\nReplace the existing file?", 1);
        }
        else
        {
            SongLoader.SaveIMV4(song, SongLoader.loadedMediaPath, path, false);
        }
    }

    public void SaveSong(string path, bool replace)
    {
        if (!path.EndsWith(".imv4")) path = path + ".imv4";
        SongLoader.SaveIMV4(song, SongLoader.loadedMediaPath, path, replace);
    }

    public void LoadSongInfoInSavePanel()
    {
        PackVirtualChunks();
        savePanel.LoadInfo(song);
    }

    public void UpdateSongInfoFromSavePanel()
    {
        savePanel.UpdateSongInfo(song);
    }

    public void LookAtPlayHead()
    {
        basePanel.LookAtPlayHead();
    }

    public void SetEntry()
    {
        basePanel.SetEntry();
    }

    public void SetExit()
    {
        basePanel.SetExit();
    }

    private List<VirtualChunk> ReadProjectChunks()
    {
        List<VirtualChunk> chunks = new List<VirtualChunk>();
        int i = 0;
        foreach(Sentence s in song.sentences)
        {
            i = 0;
            foreach(Chunk c in s.chunks)
            {
                VirtualChunk vc = chunkPool.SpawnWithPrefab(chunkParent);
                vc.SetChunk(c);
                
                chunks.Add(vc);
                i++;
                if(i == s.chunks.Length)
                {
                    vc.sentenceEnd = true;
                    Debug.Log(i);
                }
            }
        }
        return chunks;
    }

    public void InitChunkList(bool isNewProj)
    {
        if (isNewProj)
        {
            chunks = new List<VirtualChunk>();
        }
        else
        {
            chunks = ReadProjectChunks();
        }
    }

    public void CreateEmptyVirtualChunk()
    {
        VirtualChunk vc = null;
        try {
            Chunk c = new Chunk();
            c.unit = new Unit(0);
            c.startPosition = basePanel.GetEntryAsMediaTime();
            c.endPosition = basePanel.GetExitAsMediaTime();

            if(c.startPosition > c.endPosition - VirtualChunk.TIME_OFFSET)
            {
                //throw (new System.Exception("入点（绿）不能在出点（红）之后。"));
                float rst = c.startPosition;
                c.startPosition = c.endPosition;
                c.endPosition = rst;
            }
            if(c.endPosition - c.startPosition < Song.CHUNK_MIN_LENGTH)
            {
                //LANGUAGE
                throw (new System.Exception("Chunk length must be longer than " + Song.CHUNK_MIN_LENGTH + "sec."));
            }

            c.colorCode = "#" + ColorUtility.ToHtmlStringRGBA(Song.DEFAULT_PLAYED_COLOUR);

            vc = chunkPool.SpawnWithPrefab(chunkParent);
            vc.SetChunk(c);
            chunks.Add(vc);

            chunks.Sort((a, b) =>
            {
                return a.chunk.startPosition.CompareTo(b.chunk.startPosition);
            });
            if(!(CheckChunkOverlap(vc, true) && CheckChunkOverlap(vc, false)))
            {
                //LANGUAGE
                throw (new System.Exception("Selected time duration overlays with another chunk."));
            }
            SelectChunk(vc);
        }
        catch(System.Exception e)
        {
            if(vc != null)
            {
                if (chunks.Contains(vc))
                {
                    chunks.Remove(vc);
                    vc.Recycle();
                }
            }
            //LANGUAGE
            Alert("Cannot create the Chunk.\n" + e.Message);
            Debug.LogException(e);
        }
    }

    public bool CheckChunkOverlap(VirtualChunk vc, bool left)
    {
        int index = 0;
        if (left)
        {
            index = chunks.IndexOf(vc) - 1;
        }
        else
        {
            index = chunks.IndexOf(vc) + 1;
        }

        if (IsInChunkArray(index))
        {
            VirtualChunk neighbour = chunks[index];
            float l1 = vc.chunk.startPosition;
            float r1 = vc.chunk.endPosition;
            float l2 = neighbour.chunk.startPosition + SPAWN_OFFSET;
            float r2 = neighbour.chunk.endPosition - SPAWN_OFFSET;
            if(l1 < l2)
            {
                return r1 < l2;
            }
            else
            {
                return l1 > r2;
            }
        }
        else
        {
            return true;
        }
    }

    private bool IsInChunkArray(int i)
    {
        return i >= 0 && i < chunks.Count;
    }

    public void ForceUpdateAllChunks()
    {
        foreach(VirtualChunk vc in chunks)
        {
            vc.UpdateDisplay();
        }
    }

    public void SelectChunk(VirtualChunk chunk)
    {
        selectedChunk = chunk;
        if (chunk != null)
        {
            editArea.LoadChunk(chunk);
            editArea.ResetView();
        }
        editArea.gameObject.SetActive(chunk != null);
    }

    public void SetSelectedChunkTime(float start, float end)
    { 
        if(selectedChunk != null)
        {
            float oldSta = selectedChunk.chunk.startPosition;
            float oldEnd = selectedChunk.chunk.endPosition;
            try
            {
                if (start > end)
                {
                    //LANGUAGE
                    throw (new System.Exception("The end time must be later than the start time."));
                }

                if(end - start < Song.CHUNK_MIN_LENGTH)
                {
                    //LANGUAGE
                    throw (new System.Exception("Chunk length must be longer than " + Song.CHUNK_MIN_LENGTH + "sec."));
                }

                selectedChunk.chunk.startPosition = start;
                selectedChunk.chunk.endPosition = end;

                chunks.Sort((a, b) =>
                {
                    return a.chunk.startPosition.CompareTo(b.chunk.startPosition);
                });
                if (!(CheckChunkOverlap(selectedChunk, true) && CheckChunkOverlap(selectedChunk, false)))
                {
                    //LANGUAGE
                    throw (new System.Exception("Selected time duration overlays with another chunk."));
                }

                selectedChunk.UpdateDisplay();
            }
            catch (System.Exception e)
            {
                selectedChunk.chunk.startPosition = oldSta;
                selectedChunk.chunk.endPosition = oldEnd;
                editArea.LoadChunk(selectedChunk);
                Alert(e.Message);
            }
        }
    }

    public void DeleteSelectedChunk()
    {
        if (selectedChunk == null) return;

        if (chunks.Contains(selectedChunk))
        {
            chunks.Remove(selectedChunk);
        }
        selectedChunk.Recycle();
        SelectChunk(null);
    }

    public void AlignSelectedChunk()
    {
        if (selectedChunk == null) return;
        if (!chunks.Contains(selectedChunk)) return;

        chunks.Sort((a, b) =>
        {
            return a.chunk.startPosition.CompareTo(b.chunk.startPosition);
        });
        int index = chunks.IndexOf(selectedChunk);
        if(IsInChunkArray(index - 1))
        {
            selectedChunk.chunk.startPosition = chunks[index - 1].GetEndInMediaTime();
            selectedChunk.UpdateDisplay();
        }
        else if(index == 0)
        {
            selectedChunk.chunk.startPosition = 0;
            selectedChunk.UpdateDisplay();
        }
    }

    private bool IsWaitingOrSongInfo(Chunk c)
    {
        return c.eventType == 1 || c.eventType == 2;
    }

    private bool NextIsWaitingOrSongInfo(VirtualChunk c)
    {
        int index = chunks.IndexOf(c) + 1;
        if (IsInChunkArray(index))
        {
            return IsWaitingOrSongInfo(chunks[index].chunk);
        }
        else
        {
            return false;
        }
    }

    private void PackVirtualChunks()
    {
        chunks.Sort((a, b) =>
        {
            return a.chunk.startPosition.CompareTo(b.chunk.startPosition);
        });

        List<List<Chunk>> cc = new List<List<Chunk>>();
        cc.Add(new List<Chunk>());
        int i = 0;

        foreach (VirtualChunk c in chunks)
        {
            cc[i].Add(c.chunk);
            if (c.sentenceEnd || IsWaitingOrSongInfo(c.chunk) || NextIsWaitingOrSongInfo(c))
            {
                cc.Add(new List<Chunk>());
                i++;
            }
        }

        for (int n=0; n<cc.Count; n++)
        {
            if (cc[n] == null)
            {
                continue;
            }
            else if (cc[n].Count == 0)
            {
                cc.Remove(cc[n]);
                n--;
            }

        }

        song.sentences = new Sentence[cc.Count];
        i = 0;

        foreach(List<Chunk> cs in cc)
        {
            if (cs == null) continue;
            if (cs.Count == 0) continue;
            song.sentences[i] = new Sentence();
            song.sentences[i].chunks = cs.ToArray();
            i++;
        }
        SongLoader.DEBUG_PrintSongInfo(song);
        SongLoader.DEBUG_PrintSongLyrics(song);
    }

    public void EnterUnitView()
    {
        if(selectedChunk != null)
        {
            chunkPanel.SetChunk(selectedChunk);
        }
    }

    public void ExitUnitView()
    {
        chunkPanel.ExitPanel();
    }

}
