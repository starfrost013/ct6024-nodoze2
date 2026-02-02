/* Non monobehaviour based timer using System.Diagnostics.Stopwatch with millisecond precision */

using System;
using System.Diagnostics;

internal class Timer
{
    internal Int64 length { get; private set; } 
    private Stopwatch stopwatch;
    private bool isDone;

    internal const Int64 TIMER_CONTINUE_FOREVER = -1;

    internal void Start(Int64 timerLength)
    {
        length = timerLength;
        stopwatch = Stopwatch.StartNew();
    }

    internal Int64 GetElapsedTime()
    {
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
}