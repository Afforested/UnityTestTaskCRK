using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace App.Tests.Fakes
{
    public static class TestLogger
    {
        public static ILogger Create() => new Logger(new NullLogHandler());

        private sealed class NullLogHandler : ILogHandler
        {
            public void LogFormat(LogType logType, Object context, string format, params object[] args)
            {
            }

            public void LogException(Exception exception, Object context)
            {
            }
        }
    }
}
