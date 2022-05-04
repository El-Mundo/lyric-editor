using System.Collections.Generic;

namespace Lyrics.Songs {

    /// <summary>
    /// The basic class containing a song.
    /// </summary>
    [System.Serializable]
    public class Song
    {
        /*--------------------------------------------------------------
         * Song info
         * --------------------------------------------------------------*/
        /// <summary>
        /// The author(s) of the song, divided by commas.
        /// </summary>
        public string composer, songwriter;
        /// <summary>
        /// The original singer(s) of this song file, divided by commas.
        /// </summary>
        public string singer;
        /// <summary>
        /// The title of the song.
        /// </summary>
        public string title;
        /// <summary>
        /// The date when the song was published (preferably YYYY/MM/DD or YYYY).
        /// </summary>
        public string date;
        /// <summary>
        /// The language of the lyrics, possibly "CHN", "CHN-TW", "ENG", or "JPN".
        /// </summary>
        public string language;
        /// <summary>
        /// Extra information to be shown at right bottom corner.
        /// </summary>
        public string description;

        /*--------------------------------------------------------------
         * Game data
         * --------------------------------------------------------------*/
        /// <summary>
        /// The type of the stored binary file.
        /// <list type="table">
        /// <item><description>0-mp3</description></item>
        /// <item><description>1-ogg</description></item>
        /// <item><description>2-wav</description></item>
        /// <item><description>3-mp4</description></item>
        /// </list>
        /// </summary>
        public int fileType;
        /// <summary>
        /// The time length for unit answers to run through the screen (the shorter the game is harder).
        /// </summary>
        public float span;
        /// <summary>
        /// The sentences of this song, divided by semantics.
        /// </summary>
        public Sentence[] sentences;

        [System.NonSerialized]
        public readonly static UnityEngine.Color DEFAULT_PLAYED_COLOUR = new UnityEngine.Color(0.1568628F, 0.1568628F, 0.8705882F, 1);
        [System.NonSerialized]
        public readonly static float CHUNK_MIN_LENGTH = 0.2F;
    }

    /// <summary>
    /// The sentences of a song, divided by semantics.
    /// </summary>
    [System.Serializable]
    public class Sentence
    {
        public Chunk[] chunks;
    }

    /// <summary>
    /// A chunk consists of several units. Each unit stands for a lyric answer of this chunk.
    /// </summary>
    [System.Serializable]
    public class Chunk
    {
        /// <summary>
        /// The temporal duration of the unit, counted in seconds.
        /// </summary>
        public float startPosition, endPosition;
        /// <summary>
        /// The true answer.
        /// </summary>
        public Unit unit;
        /// <summary>
        /// Wrong answers.
        /// </summary>
        public Unit[] alterUnits;
        /// <summary>
        /// Indicates whether the start of this Chunk will trigger an event.
        /// <list type="table">
        /// <item><description>0-No event</description></item>
        /// <item><description>1-Show waiting lyrics</description></item>
        /// <item><description>2-Show song info</description></item>
        /// <item><description>3-Show visual effect type 0/1/2... (effect type = eventType - 3)</description></item>
        /// </list>
        /// </summary>
        public int eventType;
        /// <summary>
        /// A hexadecimal string storing the specified colour of this chunk (can be null).
        /// Should start with '#' (e.g. #008000ff).
        /// </summary>
        public string colorCode;
        /// <summary>
        /// Force this chunk to be a question even if there is only no alternative units.
        /// </summary>
        public bool forceQuestion;
        /// <summary>
        /// A chunk should be selected repeatedly for bonus.
        /// </summary>
        public bool longChunk;

        /// <summary>
        /// Get whether this chunk has alternative units or is forced to be a question.
        /// </summary>
        /// <returns>Whether this chunk should be attached to a question.</returns>
        public bool IsQuestion()
        {
            if (forceQuestion)
            {
                return true;
            }
            else
            {
                if(alterUnits != null)
                {
                    return alterUnits.Length > 0;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The question attached to this question, which only exists in runtime and should not be stored.
        /// </summary>
        [System.NonSerialized]
        public Game.Question question;
    }

    /// <summary>
    /// The basic class for the game interaction, consisting of several music notes.
    /// </summary>
    [System.Serializable]
    public class Unit
    {
        /// <summary>
        /// This note indicates the true lyric of this unit.
        /// </summary>
        public Note[] notes;
        /// <summary>
        /// Can be used to store lyric answeres.
        /// </summary>
        public string content;
        /// <summary>
        /// The notation to be shown above the unit (typically designed for Japanese Kana).
        /// </summary>
        public string notation;
        /// <summary>
        /// The channel (string) number on which the unit will be spawned if it is a question.
        /// </summary>
        public int channel;

        public Unit(int channel, string content = "", params Note[] notes)
        {
            this.content = content;
            this.notes = notes;
            this.channel = channel;
        }
    }

    /// <summary>
    /// The essential class for compositing a song, c an be understood as a MIDI note with vocal info.
    /// </summary>
    [System.Serializable]
    public class Note
    {
        /// <summary>
        /// How this note is pronunciated, formatted in Japanese Romaji.
        /// </summary>
        public string vocal;
        /// <summary>
        /// The MIDI note key.
        /// </summary>
        public int key;
        /// <summary>
        /// The time duration of the MIDI note (in seconds).
        /// </summary>
        public float duration;

        /// <summary>
        /// Indexes indicating the vocal index (not serialized but will be generated before starting the game).
        /// </summary>
        public int consonant, vowel;

        public Note(float duration, int key = 72, string vocal = "a")
        {
            this.vocal = vocal;
            this.key = key;
            this.duration = duration;
        }

        public Note(float duration, int key = 72, int consonant = 0, int vowel = 0)
        {
            vocal = "a";
            this.key = key;
            this.duration = duration;
            this.consonant = consonant;
            this.vowel = vowel;
        }

    }

}
