using UnityEngine;

namespace SlashParadox.Essence
{
    internal static class Loggers
    {
        public static Logger LogScene = new Logger(Debug.unityLogger.logHandler);
    }
}
