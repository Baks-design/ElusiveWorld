using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Timers
{
    public abstract class Timer : IDisposable
    {
        protected float initialTime;
        bool disposed;
      
        public abstract bool IsFinished { get; }
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }
        public float Progress => Mathf.Clamp(CurrentTime / initialTime, 0f, 1f);

        public event Action OnTimerStart = delegate { };
        public event Action OnTimerStop = delegate { };

        protected Timer(float value) => initialTime = value;

        public void Start()
        {
            CurrentTime = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }

        public abstract void Tick();

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                TimerManager.DeregisterTimer(this);
                OnTimerStop.Invoke();
            }
        }

        public void Resume() => IsRunning = true;

        public void Pause() => IsRunning = false;

        public virtual void Reset() => CurrentTime = initialTime;

        public virtual void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }

        ~Timer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
                TimerManager.DeregisterTimer(this);

            disposed = true;
        }
    }
}