using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using Lyrics.Songs;

public class ButtonActions : MonoBehaviour
{
    private static readonly FileBrowser.Filter imv4Filter = new FileBrowser.Filter("IMV4", "imv4"),
        mediaFilter = new FileBrowser.Filter("Audio", ".mp3", ".ogg", ".wav"),
        videoFilter = new FileBrowser.Filter("Video", "mp4");

    public void SelectMediaFile()
    {
        FileBrowser.SetFilters(true, mediaFilter, videoFilter, imv4Filter);
        FileBrowser.ShowLoadDialog(ChangeMediaPath, EmptyFuntion, FileBrowser.PickMode.Files);
    }

    private void EmptyFuntion()
    {
        return;
    }

    private void ChangeMediaPath(string[] args)
    {
        if(args != null)
        {
            if(args.Length > 0)
            {
                GameManager.instance.mediaPanel.SetPath(args[0]);
            }
        }
    }

    public void CreateProject()
    {
        string path = GameManager.instance.mediaPanel.GetMediaPath();
        if (path.EndsWith(".imv4"))
        {
            LoadExistingProject(path);
            GameManager.instance.InitChunkList(false);
        }
        else
        {
            CreateNewProject(path);
            GameManager.instance.InitChunkList(true);
        }
        if (GameManager.instance.song != null)
        {
            GameManager.instance.EnterProject();
        }
    }

    private void LoadExistingProject(string imv4Path)
    {
        GameManager.instance.song = SongLoader.LoadSongFromIMV4(imv4Path);
    }

    private void CreateNewProject(string mediaPath)
    {
        GameManager.instance.song = SongLoader.LoadSongFromMedia(mediaPath);
        if (GameManager.instance.song != null) GameManager.instance.InitNewSongInfo();
    }

    public void PlayMedia()
    {
        if (GameManager.instance.mediaPlaying)
            GameManager.instance.PauseMedia();
        else
            GameManager.instance.PlayMedia();
    }

    public void Home()
    {
        //LANGUAGE
        GameManager.instance.selectPanel.Show("Confirm to quit? Unsaved progress will be lost.", 2);
    }

    public void Save()
    {
        Song song = GameManager.instance.song;
        if (song == null) return;

        GameManager.instance.UpdateSongInfoFromSavePanel();

        FileBrowser.SetFilters(false, imv4Filter);
        FileBrowser.ShowSaveDialog(SavePathChosen, EmptyFuntion, FileBrowser.PickMode.FilesAndFolders, false, 
            null, song.title + "-" + song.singer + ".imv4");
    }

    private void SavePathChosen(string[] args)
    {
        if (args != null)
        {
            if (args.Length > 0)
            {
                GameManager.instance.SaveSong(args[0]);
            }
        }
    }

    public void CloseSavePanel()
    {
        GameManager.instance.savePanel.gameObject.SetActive(false);
    }

    public void OpenSavePanel()
    {
        GameManager.instance.LoadSongInfoInSavePanel();
        GameManager.instance.savePanel.gameObject.SetActive(true);
    }

    public void AlignPlayhead()
    {
        GameManager.instance.LookAtPlayHead();
    }

    public void SetEntry()
    {
        GameManager.instance.SetEntry();
    }

    public void SetExit()
    {
        GameManager.instance.SetExit();
    }

    public void CreateEmptyChunk()
    {
        GameManager.instance.CreateEmptyVirtualChunk();
    }

    public void EnterUnitView()
    {
        GameManager.instance.EnterUnitView();
    }

    public void ExitUnitView()
    {
        GameManager.instance.ExitUnitView();
    }

}
