using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace App.Scripts.Core.Common
{
    public static class Log
    {
        [Conditional("UNITY_EDITOR"), Conditional("UNITY_INCLUDE_INSTRUMENTATION")]
        public static void Info(object message)
        {
            Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR"), Conditional("UNITY_INCLUDE_INSTRUMENTATION")]
        public static void Warning(object message)
        {
            Debug.LogWarning(message);
        }

        public static void Error(object message)
        {
            Debug.LogError(message);
        }

        public static void Exception(Exception exception)
        {
            Debug.LogException(exception);
        }

        public static void ExceptionUnlessCanceled(Exception exception)
        {
            if (exception is OperationCanceledException)
                return;

            Debug.LogException(exception);
        }
    }
}
