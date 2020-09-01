using System;

namespace Bare10.Services.Interfaces
{
    public interface ILiveLogService
    {
        event Action<string> LogUpdated;
        string GetLog();
        void LogLine(string output);
        void LogFormat(string output, params object[] args);
        void Clear();
    }
}
