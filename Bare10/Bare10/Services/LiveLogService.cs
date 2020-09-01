using Bare10.Services.Interfaces;
using System;
using System.Text;

namespace Bare10.Services
{
    public class LiveLogService : ILiveLogService
    {
        public event Action<string> LogUpdated;

        const int MAX_LINES = 2048;
        private int currentLines = 0;

        private static ILiveLogService current = null;
        public static ILiveLogService Current
        {
            get
            {
                if(current == null)
                {
                    current = new LiveLogService();
                }
                return current;
            }
        }

        private StringBuilder logBuilder = new StringBuilder();

        private LiveLogService()
        {
        }

        public string GetLog()
        {
            return logBuilder.ToString();
        }
        
        public void LogLine(string output)
        {
            currentLines++;
            logBuilder.AppendLine(output);
            UpdateLogAndLimit();
        }

        public void LogFormat(string output, params object[] args)
        {
            currentLines++;
            logBuilder.AppendFormat(output, args);
            UpdateLogAndLimit();
        }

        public void Clear()
        {
            currentLines = 0;
            logBuilder.Clear();
        }

        private void UpdateLogAndLimit()
        {
            if (currentLines >= MAX_LINES)
                Clear();
            LogUpdated?.Invoke(logBuilder.ToString());
        }
    }
}
