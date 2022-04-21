using UnityEngine;

namespace Lyrics.Game
{
    public interface IObjectPoolHandler<T>
    {
        bool IsAvailable();

        T Spawn();

        void Recycle();
    }

    public abstract class PoolableBehaviour : MonoBehaviour, IObjectPoolHandler<PoolableBehaviour>
    {
        public abstract bool IsAvailable();

        PoolableBehaviour IObjectPoolHandler<PoolableBehaviour>.Spawn()
        {
            return this;
        }

        /// <summary>
        /// This function should force the object to diappear (e.g. go back to the pool's position).
        /// </summary>
        public abstract void Recycle();
    }

}
