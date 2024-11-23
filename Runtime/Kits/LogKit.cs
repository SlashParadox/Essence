// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Diagnostics;
using UnityEngine;

namespace SlashParadox.Essence.Kits
{
    public static class LogKit
    {
        /// <summary>
        /// Throws a <see cref="NullReferenceException"/> if true.
        /// </summary>
        /// <param name="statement">The statement to check.</param>
        /// <param name="message">An optional exception message.</param>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="statement"/> is true.</exception>
        public static void ThrowNullIf(bool statement, string message = null)
        {
            if (statement)
                throw new NullReferenceException(message);
        }

        public static bool LogIfFalse(bool statement, string message = null)
        {
            if (statement)
                return true;

            UnityEngine.Debug.LogWarning(message);

            return false;
        }
        
        /// <summary>
        /// Logs a formatted message. Development only.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("UNITY_EDITOR")] [Conditional("DEVELOPMENT_BUILD")] [Conditional("ESSENCE_ALLOW_SHIPPING_LOGS")]
        public static void LogFormat(Logger log, string format, params object[] args)
        {
            SLogFormat(log, LogType.Log, format, args);
        }

        /// <summary>
        /// Logs a formatted message. Development only.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("UNITY_EDITOR")] [Conditional("DEVELOPMENT_BUILD")] [Conditional("ESSENCE_ALLOW_SHIPPING_LOGS")]
        public static void LogFormat(Logger log, UnityEngine.Object context, string format, params object[] args)
        {
            SLogFormat(log, LogType.Log, context, format, args);
        }

        /// <summary>
        /// Logs a formatted message. Development only.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("UNITY_EDITOR")] [Conditional("DEVELOPMENT_BUILD")] [Conditional("ESSENCE_ALLOW_SHIPPING_LOGS")]
        public static void LogFormat(Logger log, LogType logType, string format, params object[] args)
        {
            SLogFormat(log, logType, format, args);
        }

        /// <summary>
        /// Logs a formatted message. Development only.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("UNITY_EDITOR")] [Conditional("DEVELOPMENT_BUILD")] [Conditional("ESSENCE_ALLOW_SHIPPING_LOGS")]
        public static void LogFormat(Logger log, LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            SLogFormat(log, logType, context, format, args);
        }
        
        /// <summary>
        /// Logs a formatted message. Works even in non-development builds. Only use if required.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void SLogFormat(Logger log, string format, params object[] args)
        {
            SLogFormat(log, LogType.Log, format, args);
        }

        /// <summary>
        /// Logs a formatted message. Works even in non-development builds. Only use if required.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void SLogFormat(Logger log, UnityEngine.Object context, string format, params object[] args)
        {
            SLogFormat(log, LogType.Log, context, format, args);
        }
        
        /// <summary>
        /// Logs a formatted message. Works even in non-development builds. Only use if required.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void SLogFormat(Logger log, LogType logType, string format, params object[] args)
        {
            if (log == null)
                UnityEngine.Debug.LogFormat(format, args);
            else
                log.LogFormat(logType, format, args);
        }
        
        /// <summary>
        /// Logs a formatted message. Works even in non-development builds. Only use if required.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void SLogFormat(Logger log, LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            if (log == null)
                UnityEngine.Debug.LogFormat(context, format, args);
            else
                log.LogFormat(logType, context, format, args);
        }
        
        /// <summary>
        /// Logs a message. Development only.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("UNITY_EDITOR")] [Conditional("DEVELOPMENT_BUILD")] [Conditional("ESSENCE_ALLOW_SHIPPING_LOGS")]
        public static void Log(Logger log, object message, UnityEngine.Object context = null)
        {
            SLog(log, LogType.Log, message, context);
        }

        /// <summary>
        /// Logs a message. Development only.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("UNITY_EDITOR")] [Conditional("DEVELOPMENT_BUILD")] [Conditional("ESSENCE_ALLOW_SHIPPING_LOGS")]
        public static void Log(Logger log, string tag, object message, UnityEngine.Object context = null)
        {
            SLog(log, LogType.Log, tag, message, context);
        }
        
        /// <summary>
        /// Logs a message. Development only.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("UNITY_EDITOR")] [Conditional("DEVELOPMENT_BUILD")] [Conditional("ESSENCE_ALLOW_SHIPPING_LOGS")]
        public static void Log(Logger log, LogType logType, object message, UnityEngine.Object context = null)
        {
            SLog(log, logType, message, context);
        }

        /// <summary>
        /// Logs a message. Development only.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("UNITY_EDITOR")] [Conditional("DEVELOPMENT_BUILD")] [Conditional("ESSENCE_ALLOW_SHIPPING_LOGS")]
        public static void Log(Logger log, LogType logType, string tag, object message, UnityEngine.Object context = null)
        {
            SLog(log, logType, tag, message, context);
        }
        
        /// <summary>
        /// Logs a message. Works even in non-development builds. Only use if required.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void SLog(Logger log, object message, UnityEngine.Object context = null)
        {
            SLog(log, LogType.Log, message, context);
        }

        /// <summary>
        /// Logs a message. Works even in non-development builds. Only use if required.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void SLog(Logger log, string tag, object message, UnityEngine.Object context = null)
        {
            SLog(log, LogType.Log, tag, message, context);
        }
        
        /// <summary>
        /// Logs a message. Works even in non-development builds. Only use if required.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void SLog(Logger log, LogType logType, object message, UnityEngine.Object context = null)
        {
            if (log == null)
                UnityEngine.Debug.Log(message, context);
            else
                log.Log(logType, message, context);
        }

        /// <summary>
        /// Logs a message. Works even in non-development builds. Only use if required.
        /// </summary>
        /// <param name="log">The <see cref="UnityEngine.Logger"/> to use. If null, the standard <see cref="UnityEngine.Debug"/> method is used.</param>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void SLog(Logger log, LogType logType, string tag, object message, UnityEngine.Object context = null)
        {
            if (log == null)
                UnityEngine.Debug.Log($"{tag}: {message}", context);
            else
                log.Log(logType, tag, message, context);
        }
    }
}