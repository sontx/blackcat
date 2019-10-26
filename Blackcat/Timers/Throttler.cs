using System;
using System.Threading;

namespace Blackcat.Timers
{
    public class Throttler
    {
        private Timer _throttleTimerInterval;
        private Action<object> _throttleAction;
        private object _lastObjectThrottle;

        /// <summary>
        /// Throttle give you last objcet when timer was ticked and invoke throttleAction callback.
        /// <exception cref="http://demo.nimius.net/debounce_throttle/">See this example for understanding what is RateLimiting and Throttle</exception>
        /// </summary>
        /// <param name="obj">Your object</param>
        /// <param name="interval">Milisecond interval</param>
        /// <param name="throttleAction">Invoked last object when timer ticked invoked</param>
        public void Throttle(object obj, int interval, Action<object> throttleAction)
        {
            _lastObjectThrottle = obj;
            _throttleAction = throttleAction;

            if (_throttleTimerInterval == null)
            {
                _throttleTimerInterval = new Timer(ThrottleTimerIntervalOnTick, obj, interval, interval);
            }
        }

        /// <summary>
        /// DispatchTimer tick event for throttle
        /// </summary>
        /// <param name="state"></param>
        private void ThrottleTimerIntervalOnTick(object state)
        {
            _throttleTimerInterval?.Dispose();
            _throttleTimerInterval = null;

            if (_lastObjectThrottle != null)
            {
                _throttleAction?.Invoke(_lastObjectThrottle);
            }
        }
    }
}