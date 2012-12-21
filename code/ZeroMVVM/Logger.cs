using System;
using System.Diagnostics;

namespace ZeroMVVM
{
    public class Logger
    {
        public void Warn(string message)
        {
            Trace.WriteLine("[WARN] " + message);
        }

        public void Info(string message)
        {
            Trace.WriteLine("[INFO] " + message);
        }

        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] " + message);
        }
    }

    internal class LogWrapper
    {
        private readonly dynamic logger;

        public LogWrapper(dynamic logger)
        {
            this.logger = logger;
        }

        public void Warn(string message)
        {
            if (logger == null)
                return;

            if (logger.GetType().GetMethod("Warn", new Type[] { typeof(string) }) != null)
                logger.Warn(message);
        }

        public void Info(string message)
        {
            if (logger == null)
                return;

            if (logger.GetType().GetMethod("Info", new Type[] { typeof(string) }) != null)
                logger.Info(message);
        }

        public void Debug(string message)
        {
            if (logger == null)
                return;

            if (logger.GetType().GetMethod("Debug", new Type[] { typeof(string) }) != null)
                logger.Debug(message);
        }
    }
}