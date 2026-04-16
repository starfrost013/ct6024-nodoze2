/* Non monobehaviour based timer using System.Diagnostics.Stopwatch with millisecond precision */

using System;
using System.Diagnostics;

internal class Timer
{
    internal Int64 length { get; private set; } 
    private Stopwatch stopwatch;
    private bool isDone;

    /// <summary>
    /// Value used to tell a timer to never stop.
    /// </summary>
    internal const Int64 TIMER_CONTINUE_FOREVER = -1;

    internal void Start(Int64 timerLength)
    {
        length = timerLength;
        stopwatch = Stopwatch.StartNew();
    }

    internal Int64 GetElapsedTime()
    {
        // it's cleaner api design if we don't allow this and have a constructor above,
        // but you can't crearte a system.diagnostics.stopwatch without starnig it and then immediatelys topping it which takes time
        // so, we just do this -- we don't realy care if the inner stopwatch exists.
        if (stopwatch == null)
            return 0;

        if (stopwatch.ElapsedMilliseconds > length
            && length != TIMER_CONTINUE_FOREVER)
        {
            isDone = true;
            stopwatch.Stop();
        }
 
        return stopwatch.ElapsedMilliseconds;
    }

    /// <summary>
    /// Determines if the timer has ever been started.
    /// </summary>
    /// <returns>A boolean value determining if the timer has ever been started</returns>
    internal bool HasStarted()
    {
        return (stopwatch != null);
    }

    internal bool IsDone()
    {
        _ = GetElapsedTime();
        return isDone;
    }

    /// <summary>
    /// Stops the timer and restarts it to zero.
    /// </summary>
    internal void Reset()
    {
        stopwatch.Reset();
    }

    /// <summary>
    /// Restart the timer
    /// </summary>
    internal void Restart()
    {
        stopwatch.Restart();
    }


    /// <summary>
    /// Stop the timer
    /// </summary>
    internal void Stop()
    {
        // stop the tier
        stopwatch.Stop(); 
    }
}