using System;
using System.Diagnostics;
using Sitecore.Diagnostics;

namespace Sitecore.Doppelganger
{
    public class LogPerformance : IDisposable
    {
        private readonly string _message;
        private readonly Stopwatch _stopwatch;

        public LogPerformance(string message)
        {
            _message = message;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            if (Log.IsDebugEnabled)
                Log.Debug($"{_message} took {_stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}