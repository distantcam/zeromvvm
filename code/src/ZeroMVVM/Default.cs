using System;
using ZeroMVVM.Conventions;
using ZeroMVVM.Dynamic;

namespace ZeroMVVM
{
    public static class Default
    {
        private static Func<string, dynamic> logger;

        static Default()
        {
            AttachmentConvention = typeof(AttachmentConvention);
            ViewConvention = typeof(ViewConvention);
            ViewModelConvention = typeof(ViewModelConvention);

            Logger = t => new Logger();
        }

        public static Type AttachmentConvention { get; set; }

        public static Type ViewModelConvention { get; set; }

        public static Type ViewConvention { get; set; }

        public static dynamic IoC { get; set; }

        public static Func<string, dynamic> Logger
        {
            get { return logger; }
            set
            {
                if (logger == value)
                    return;
                logger = value;
                SetupDefaultLoggerSettings();
            }
        }

        private static void SetupDefaultLoggerSettings()
        {
            var logger = Logger("Test");

            if (logger.GetType().Namespace.StartsWith("NLog"))
                ConfigureDefaultNLog();
        }

        private static void ConfigureDefaultNLog()
        {
            dynamic logManager = new StaticMembersDynamicWrapper(Type.GetType("NLog.LogManager, NLog"));

            if (logManager.Configuration != null)
                return;

            dynamic config = Activator.CreateInstance("NLog", "NLog.Config.LoggingConfiguration").Unwrap();
            dynamic target = Activator.CreateInstance("NLog", "NLog.Targets.ColoredConsoleTarget").Unwrap();
            dynamic loggingRule = Activator.CreateInstance("NLog", "NLog.Config.LoggingRule").Unwrap();

            target.Layout = "[${level:uppercase=true:padding=-5}] ${logger}: ${message}${onexception:inner=${newline}${exception:format=tostring}}";
            config.AddTarget("console", target);

            dynamic logLevel = new StaticMembersDynamicWrapper(Type.GetType("NLog.LogLevel, NLog"));

            loggingRule.LoggerNamePattern = "*";
            loggingRule.Targets.Add(target);
            for (int i = 2; i < 6; i++)
            {
                var level = logLevel.FromOrdinal(i);
                loggingRule.EnableLoggingForLevel(level);
            }

            config.LoggingRules.Add(loggingRule);

            logManager.Configuration = config;
        }
    }
}