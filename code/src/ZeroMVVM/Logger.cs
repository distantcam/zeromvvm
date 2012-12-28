using System;
using System.Diagnostics;

namespace ZeroMVVM
{
    public class Logger
    {
        public virtual void Error(string message)
        {
            Trace.WriteLine("[ERROR] " + message);
        }

        public virtual void Warn(string message)
        {
            Trace.WriteLine("[WARN ] " + message);
        }

        public virtual void Info(string message)
        {
            Trace.WriteLine("[INFO ] " + message);
        }

        public virtual void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] " + message);
        }
    }

    internal class LogWrapper : Logger
    {
        private readonly dynamic logger;

        public LogWrapper(dynamic logger)
        {
            this.logger = logger;
        }

        public override void Error(string message)
        {
            if (logger == null)
            {
                base.Error(message);
                return;
            }

            if (logger.GetType().GetMethod("Error", new Type[] { typeof(string) }) != null)
                logger.Error(message);
        }

        public override void Warn(string message)
        {
            if (logger == null)
            {
                base.Warn(message);
                return;
            }

            if (logger.GetType().GetMethod("Warn", new Type[] { typeof(string) }) != null)
                logger.Warn(message);
        }

        public override void Info(string message)
        {
            if (logger == null)
            {
                base.Info(message);
                return;
            }

            if (logger.GetType().GetMethod("Info", new Type[] { typeof(string) }) != null)
                logger.Info(message);
        }

        public override void Debug(string message)
        {
            if (logger == null)
            {
                base.Debug(message);
                return;
            }

            if (logger.GetType().GetMethod("Debug", new Type[] { typeof(string) }) != null)
                logger.Debug(message);
        }
    }
}