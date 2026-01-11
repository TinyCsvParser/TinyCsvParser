using NUnit.Framework;
using System;
using System.Diagnostics;

namespace TinyCsvParser.Test.Integration;

public static class MeasurementUtils
{
    public static void MeasureElapsedTime(string description, Action action)
    {
        // Get the elapsed time as a TimeSpan value.
        var ts = MeasureElapsedTime(action);

        // Format and display the TimeSpan value.
        var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";

        TestContext.WriteLine("[{0}] Elapsed Time = {1}", description, elapsedTime);
    }

    public static void MeasureElapsedTime(string description, Action action, Func<TimeSpan, string> timespanFormatter)
    {
        // Get the elapsed time as a TimeSpan value.
        var ts = MeasureElapsedTime(action);

        var elapsedTime = timespanFormatter(ts);

        TestContext.WriteLine("[{0}] Elapsed Time = {1}", description, elapsedTime);
    }


    private static TimeSpan MeasureElapsedTime(Action action)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        action();
        stopWatch.Stop();

        return stopWatch.Elapsed;
    }
}