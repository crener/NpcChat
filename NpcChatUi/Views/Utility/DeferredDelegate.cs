using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace NpcChat.Views.Utility
{
    /// <summary>
    /// This class is for calling a delegate callback in the future.
    /// </summary>
    public static class DeferredDelegate
    {
        private static ConcurrentDictionary<object, Timer> s_timers = new ConcurrentDictionary<object, Timer>();

        /// <summary>
        /// Call the <see cref="callback"/> after the amount of milliseconds specified in <see cref="delay"/>
        /// </summary>
        /// <param name="delay">delay in a number of milliseconds</param>
        /// <param name="callback">called after <see cref="delay"/></param>
        public static void Call(double delay, Action callback)
        {
            Timer timer = InternalCall(delay, callback);
            timer.Start();
        }

        /// <summary>
        /// Call the <see cref="callback"/> after the amount of milliseconds specified in <see cref="delay"/>
        /// </summary>
        /// <param name="delay">delay in a number of milliseconds</param>
        /// <param name="callback">called after <see cref="delay"/></param>
        /// <param name="reference">reference object to use for resetting or canceling the callback</param>
        public static void Call(double delay, Action callback, object reference)
        {
            if (s_timers.ContainsKey(reference))
            {
                Timer existingTimer = s_timers[reference];
                existingTimer.Stop();
            }

            Timer timer = InternalCall(delay, callback);
            timer.Elapsed += (o, args) =>
            {
                // remove the existing timer from the collection
                s_timers.TryRemove(timer, out Timer time);
            };
            timer.Start();

            s_timers[reference] = timer;
        }

        /// <summary>
        /// Cancel the deferred callback using the reference that was used to create it
        /// </summary>
        /// <param name="reference">reference object to use for resetting or canceling the callback</param>
        public static void CancelCall(object reference)
        {
            if (s_timers.ContainsKey(reference))
            {
                s_timers.TryRemove(s_timers[reference], out Timer time);
                time.Stop();
            }
        }

        private static Timer InternalCall(double delay, Action callback)
        {
            Timer timer = new Timer(delay) {AutoReset = false};
            timer.Elapsed += (o, e) => callback?.Invoke();

            return timer;
        }
    }
}
