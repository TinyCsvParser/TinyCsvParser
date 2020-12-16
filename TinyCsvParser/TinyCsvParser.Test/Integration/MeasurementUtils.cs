using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCsvParser.Test.Integration
{
    public static class MeasurementUtils
    {
        public static void MeasureElapsedTime(string description, Action action)
        {
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = MeasureElapsedTime(action);

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            TestContext.WriteLine("[{0}] Elapsed Time = {1}", description, elapsedTime);
        }

        public static void MeasureElapsedTime(string description, Action action, Func<TimeSpan, string> timespanFormatter)
        {
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = MeasureElapsedTime(action);

            string elapsedTime = timespanFormatter(ts);

            TestContext.WriteLine("[{0}] Elapsed Time = {1}", description, elapsedTime);
        }



        private static TimeSpan MeasureElapsedTime(Action action)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            action();
            stopWatch.Stop();

            return stopWatch.Elapsed;
        }

    }
}
