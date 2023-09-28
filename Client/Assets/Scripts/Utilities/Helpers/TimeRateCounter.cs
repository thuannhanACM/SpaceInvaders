using UnityEngine;

namespace Core.Framework.Helpers
{
    public class TimeRateCounter
    {
        public float Rate { get; private set; }
        public float ExecuteTime { get; private set; }
        public bool IsPassedWithAlwaysPassedOnFirstTime(float currentTime)
        {
            bool passed = ExecuteTime <= currentTime;
            if (passed)
            {
                ExecuteTime = currentTime + Rate;
                return true;
            }
            return false;
        }
        public bool IsPassedWithCheckOnFirstTime(float currentTime)
        {
            ExecuteTime = ExecuteTime <= 0? currentTime + Rate : ExecuteTime;
            return IsPassedWithAlwaysPassedOnFirstTime(currentTime);
        }

        public TimeRateCounter(float rate)
        {
            Rate = rate;
            ExecuteTime = -1f;
        }
    }
}
