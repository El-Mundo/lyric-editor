using Lyrics.Game;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Lyrics.Songs{

    public enum SentenceType
    {
        Lyric,
        SongInfo,
        Waiting,
        Empty
    }

    public class SongLoader
    {
        public static string loadedMediaPath = "";

        public static readonly string cachePath = Path.Combine(Application.persistentDataPath, "_cache");
        public static readonly string IMV4_FLAG = "<---IMV4_END_OF_JSON--->";
        public static readonly System.Text.Encoding encoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// Load JSON files containing song data in Unity's Asset/Resources.
        /// </summary>
        /// <param name="jsonResourcePath">The JSON file's path relative to "Asset/Resources" under the Unity prooject.</param>
        /// <returns>The loaded song instance.</returns>
        public static Song LoadSongFromInternalJSON(string jsonResourcePath)
        {
            Song output = new Song();
            JsonUtility.FromJsonOverwrite(Resources.Load<TextAsset>(jsonResourcePath).text, output);
            return output;
        }

        public static Song LoadSongFromIMV4(string imv4Path)
        {
            try
            {
                if (!File.Exists(imv4Path)) {
                    throw new System.Exception("File doesn't exist!");
                }
                string content = File.ReadAllText(imv4Path, encoding);
                string[] flag = { IMV4_FLAG };
                FileInfo info = new FileInfo(imv4Path);

                string[] splited = content.Split(flag, System.StringSplitOptions.None);

                Song song = new Song();
                JsonUtility.FromJsonOverwrite(splited[0], song);

                //Directly converting UTF8 to bytes cause unexpectable errors, so use BASE64 as a medium
                byte[] base64 = Convert.FromBase64String(splited[1]);
                loadedMediaPath = CacheFile(info.Name + SongPreview.TypeIndexToString(song.fileType), base64);

                return song;
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                GameManager.instance.Alert("创建失败，\n错误信息：" + e.Message);
                return null;
            }
        }

        public static Song LoadSongFromMedia(string mediaPath)
        {
            try
            {
                if (!File.Exists(mediaPath))
                {
                    throw new System.Exception("File doesn't exist!");
                }
                FileInfo info = new FileInfo(mediaPath);
                Song song = new Song();

                song.fileType = SongPreview.TypeStringToIndex(info.Extension);
                if(song.fileType == -1)
                {
                    throw new System.Exception("Unrecoginizable file extension!");
                }
                song.span = 15.0f;

                string newMediaPath = CacheCopyFile(mediaPath, info.Name);

                loadedMediaPath = newMediaPath;

                return song;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                GameManager.instance.Alert("创建失败，\n错误信息：" + e.Message);
                return null;
            }
        }

        public static void SaveIMV4(Song song, string mediaPath, string target, bool replace)
        {
            try
            {
                if (File.Exists(target))
                {
                    if (!replace) {
                        target = GetNonExistingPath(target);
                        if (target == null)
                        {
                            throw new Exception("Cannot create file. There are too many overlapping files.");
                        }
                    }
                    else
                    {
                        File.Delete(target);
                    }
                }

                string jsonContent = JsonUtility.ToJson(song);
                byte[] mediaBytes = File.ReadAllBytes(mediaPath);
                string mediaContent = Convert.ToBase64String(mediaBytes);

                File.WriteAllText(target, jsonContent, encoding);
                File.AppendAllText(target, IMV4_FLAG, encoding);
                File.AppendAllText(target, mediaContent, encoding);

                GameManager.instance.Alert("Saved at: " + target);

                return;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                GameManager.instance.Alert("保存失败，\n错误信息：" + e.Message);
                return;
            }
        }

        public static string GetNonExistingPath(string oldPath)
        {
            FileInfo info = new FileInfo(oldPath);
            string nonEx = oldPath.Substring(0, oldPath.Length - info.Extension.Length);
            string path = nonEx + "(1)" + info.Extension;
            int i = 1;
            while (File.Exists(path))
            {
                i++;
                path = nonEx + "(" + i + ")" + info.Extension;
                //To prevent dead looping
                if (i > 256) return null;
            }
            return path;
        }

            /// <summary>
            /// Cache a file in persistent data.
            /// </summary>
            /// <param name="fileName">Wanted file name.</param>
            /// <param name="content">Content to write.</param>
            /// <returns>The position of the cached file.</returns>
        public static string CacheFile(string fileName, string content)
        {
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            string target = Path.Combine(cachePath, fileName);
            if (File.Exists(target))
            {
                File.Delete(target);
            }
            File.WriteAllText(target, content, encoding);
            return target;
        }

        public static string CacheFile(string fileName, byte[] bytes)
        {
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            string target = Path.Combine(cachePath, fileName);
            if (File.Exists(target))
            {
                File.Delete(target);
            }
            File.WriteAllBytes(target, bytes);
            return target;
        }

        public static string CacheCopyFile(string oldFilePath, string fileName)
        {
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            string target = Path.Combine(cachePath, fileName);
            if (File.Exists(target))
            {
                File.Delete(target);
            }
            File.Copy(oldFilePath, target);
            return target;
        }

        public static void ClearCache()
        {
            if (Directory.Exists(cachePath))
            {
                Directory.Delete(cachePath, true);
            }
        }

        /*public static Song LoadSongFromExternalJSON(string jsonFilePath)
        {
            
        }*/

        public static string GetSentenceText(Sentence sentence)
        {
            SentenceType t = GetSentenceType(sentence);
            if (t == SentenceType.Waiting) return "(Waiting...)";
            else if (t == SentenceType.SongInfo) return "(Song info)";

            string output = "";
            foreach (Chunk chunk in sentence.chunks)
            {
                output += chunk.unit.content;
            }
            return output;
        }

        /// <summary>
        /// Get the time range of a sentence.
        /// </summary>
        /// <returns>A Vector2 with x as start time and y as end.</returns>
        public static Vector2 GetSentenceRange(Sentence sentence)
        {
            if(sentence.chunks != null)
            {
                if(sentence.chunks.Length > 0)
                {
                    Chunk[] chunks = sentence.chunks;
                    return new Vector2(chunks[0].startPosition, chunks[chunks.Length - 1].endPosition);
                }
            }
            return Vector2.zero;
        }

        public static SentenceType GetSentenceType(Sentence sentence)
        {
            if (sentence.chunks != null)
            {
                if (sentence.chunks.Length > 0)
                {
                    switch (sentence.chunks[0].eventType)
                    {
                        case 1:
                            return SentenceType.Waiting;
                        case 2:
                            return SentenceType.SongInfo;
                        default:
                            return SentenceType.Lyric;
                    }
                }
                else
                {
                    return SentenceType.Empty;
                }
            }
            else
            {
                return SentenceType.Empty;
            }
        }

        private static readonly QColor[] QUESTION_COLOUR_SET = {
            QColor.Red, QColor.Orange, QColor.Yellow, QColor.Green, QColor.Cyan, QColor.Blue, QColor.Purple };

        public static List<Question> GetSongQuestions(Song song)
        {
            List<Question> questions = new List<Question>();
            int colourIndex = 0;

            foreach (Sentence sentence in song.sentences)
            {
                foreach (Chunk chunk in sentence.chunks)
                {
                    if (chunk.IsQuestion())
                    {
                        if (!chunk.longChunk)
                        {
                            questions.Add(new Question(chunk, QUESTION_COLOUR_SET[colourIndex]));
                            colourIndex++;
                            if (colourIndex >= QUESTION_COLOUR_SET.Length) colourIndex = 0;
                        }
                        else
                        {
                            questions.Add(new LongQuestion(chunk));
                        }
                    }
                }
            }
            return questions;
        }

        public static void DEBUG_PrintSongLyrics(Song song)
        {
            string line = "";
            foreach (Sentence s in song.sentences)
            {
                line += GetSentenceText(s) + "\n\n";
            }
            Debug.Log(line);
        }

        public static void DEBUG_PrintChunkInfo(Chunk chunk)
        {
            string line = "";
            line += chunk.unit.content + "\n";
            line += "Notation: " + chunk.unit.notation + "\n";
            line += "Time: " + chunk.startPosition + "-" + chunk.endPosition + "\n";
            line += "Long Chunk: " + chunk.longChunk + "\n";
            line += "Force Question:" + chunk.forceQuestion + "\n";
            line += "Color Code: " + chunk.colorCode + "\n";
            line += "Event: " + chunk.eventType + "\n";
            line += "Alter Unit Number:" + (chunk.alterUnits == null ? 0 : chunk.alterUnits.Length) + "\n\n";
            Debug.Log(line);
        }

        public static void DEBUG_PrintSongInfo(Song song)
        {
            string line = "";
            line += song.title + "\n";
            line += "Singer: " + song.singer + "\n";
            line += "Songwriter: " + song.songwriter + "\n";
            line += "Composer: " + song.composer + "\n";
            line += "Date:" + song.date + "\n";
            line += "Language: " + song.language + "\n";
            line += "Description: " + song.description + "\n";
            line += "File Type:" + song.fileType + "\n\n";
            Debug.Log(line);
        }

        public static void DEBUG_PrintUnitInfo(Unit unit)
        {
            string line = "";
            line += "Lyric: " + unit.content + "\n";
            line += "Notation: " + unit.notation + "\n";
            line += "Channel: " + unit.channel + "\n";
            line += "MIDI: " + UnitPanel.NotesToMIDICode(unit.notes) + "\n\n";
            Debug.Log(line);
        }

        public static void DEBUG_PrintChunkUnits(Chunk chunk)
        {
            DEBUG_PrintUnitInfo(chunk.unit);
            foreach(Unit c in chunk.alterUnits)
            {
                DEBUG_PrintUnitInfo(c);
            }
        }

    }

}
