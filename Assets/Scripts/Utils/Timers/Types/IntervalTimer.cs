using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Timers.Types
{
    /// <summary>
    /// Countdown timer that fires an event every interval until completion.
    /// </summary>
    public class IntervalTimer : Timer
    {
        readonly float interval;
        float nextInterval;

        public event Action OnInterval = delegate { };

        public IntervalTimer(float totalTime, float intervalSeconds) : base(totalTime)
        {
            interval = intervalSeconds;
            nextInterval = totalTime - interval;
        }

        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0f)
            {
                CurrentTime -= Time.deltaTime;
                // Fire interval events as long as thresholds are crossed
                while (CurrentTime <= nextInterval && nextInterval >= 0f)
                {
                    OnInterval.Invoke();
                    nextInterval -= interval;
                }
            }

            if (IsRunning && CurrentTime <= 0f)
            {
                CurrentTime = 0f;
                Stop();
            }
        }

        public override bool IsFinished => CurrentTime <= 0f;

        public override void Reset()
        {
            base.Reset();
            nextInterval = initialTime - interval;
        }

        public override void Reset(float newTime)
        {
            base.Reset(newTime);
            nextInterval = initialTime - interval;
        }
    }
}