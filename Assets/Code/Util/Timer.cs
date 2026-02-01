/* Non monobehaviour based timer using System.Diagnostics.Stopwatch with millisecond precision */

using System;
using System.Diagnostics;

internal class Timer
{
    private Int64 length;
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
            && length != -1)
        {
            isDone = true;
            stopwatch.Stop();
        }

        return stopwatch.ElapsedMilliseconds;
    }

    internal bool IsDone()
    {
        _ = GetElapsedTime();
        return isDone;
    }
}