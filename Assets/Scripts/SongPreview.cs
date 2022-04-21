using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Lyrics.Songs
{
    [System.Serializable]
    public class SongPreview
    {
        public string jsonPath;
        public string mediaPath;
        public string fileName;

        /// <summary>
        /// The type of the stored binary file.
        /// <list type="table">
        /// <item><description>0-mp3</description></item>
        /// <item><description>1-ogg</description></item>
        /// <item><description>2-wav</description></item>
        /// <item><description>3-mp4</description></item>
        /// </list>
        /// </summary>
        public int mediaType;

        public string title, singer, composer, songwriter, date, description, language;

        /// <summary>
        /// Whether this song is stored as part of this game (in Unity's Resources folder).
        /// </summary>
        public bool isInternal;

        public static void DEBUG_PrintPreviewInfo(SongPreview preview)
        {
            string line = "";
            line += preview.title + "\n";
            line += "Singer: " + preview.singer + "\n";
            line += "Songwriter: " + preview.songwriter + "\n";
            line += "Composer: " + preview.composer + "\n";
            line += "Date:" + preview.date + "\n";
            line += "Description: " + preview.description + "\n";
            line += "Language: " + preview.language + "\n";
            line += "Media type: " + TypeIndexToString(preview.mediaType) + "\n";
            Debug.Log(line);
        }

        public static string TypeIndexToString(int type)
        {
            switch (type)
            {
                case 0:
                    return ".mp3";
                case 1:
                    return ".ogg";
                case 2:
                    return ".wav";
                case 3:
                    return ".mp4";
                default:
                    return "";
            }
        }

        public static int TypeStringToIndex(string type)
        {
            switch (type)
            {
                case ".mp3":
                case ".MP3":
                    return 0;
                case ".ogg":
                case ".OGG":
                    return 1;
                case ".wav":
                case ".WAV":
                    return 2;
                case ".mp4":
                case ".MP4":
                    return 3;
                default:
                    return -1;
            }
        }

    }

    [System.Serializable]
    public class SongPreviewList
    {
        public List<SongPreview> list;

        public SongPreviewList()
        {
            list = new List<SongPreview>();
        }

        public static SongPreviewList LoadInternalSongPreviewList(string jsonResourcePath)
        {
            SongPreviewList output = new SongPreviewList();
            JsonUtility.FromJsonOverwrite(Resources.Load<TextAsset>(jsonResourcePath).text, output);
            return output;
        }

    }

}
