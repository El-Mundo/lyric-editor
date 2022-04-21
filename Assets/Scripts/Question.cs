using Lyrics.Songs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lyrics.Game
{

    /// <summary>
    /// White is a preserved colour for long chunks.
    /// </summary>
    public enum QColor
    {
        Red,
        Orange,
        Yellow,
        Green,
        Cyan,
        Blue,
        Purple,
        White //White is a preserved colour for long chunks
    }

    public class Question
    {
        public Chunk chunk;
        public QColor colour;
        public float shootTime, chunkLength;
        public bool isLongQuestion;
        public List<LyricUnit> units;

        private string answer;
        private float pressTime;
        public bool answered = false;

        public bool spawned;

        /// <summary>
        /// How long has the right-answer unit of this Question been pressed (percentage of press time / chunk duration)?
        /// </summary>
        public float progress;

        public Question(Chunk chunk, QColor colour)
        {
            this.chunk = chunk;
            this.shootTime = chunk.startPosition - GameManager.instance.shootOffset;
            chunkLength = chunk.endPosition - chunk.startPosition;
            this.colour = colour;

            this.progress = 0.0f;
            this.spawned = false;
            units = new List<LyricUnit>();

            answer = chunk.unit.content;
            pressTime = 0.0f;

            chunk.question = this;
            isLongQuestion = false;
        }

        public bool IsRightAnswer(string answer)
        {
            return this.answer == answer;
        }

        public void UpdateProgress(float deltaTime)
        {
            pressTime += deltaTime;
            this.progress = pressTime / chunkLength;
            if (progress > 1.0f) progress = 1.0f;
            else if (progress < 0.0f) progress = 0.0f;
        }

    }

    public class LongQuestion : Question
    {
        private static readonly QColor LONG_CHUNK_COLOUR = QColor.White;

        public LongQuestion(Chunk chunk) : base(chunk, LONG_CHUNK_COLOUR)
        {
            isLongQuestion = true;
        }
    }
}
