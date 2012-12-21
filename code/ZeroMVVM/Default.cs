using System;
using System.Reflection;
using ZeroMVVM.Conventions;

namespace ZeroMVVM
{
    public static class Default
    {
        private static Func<string, dynamic> logger;

        static Default()
        {
            ViewConvention = typeof(ViewConvention);
            ViewModelConvention = typeof(ViewModelConvention);

            Logger = t => new Logger();
        }

        public static Type ViewModelConvention { get; set; }

        public static Type ViewConvention { get; set; }

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
            var logManagerConfigProperty = Type.GetType("NLog.LogManager, NLog").GetProperty("Configuration");

            if (logManagerConfigProperty.GetValue(null, null) != null)
                return;

            dynamic config = Activator.CreateInstance("NLog", "NLog.Config.LoggingConfiguration").Unwrap();
            dynamic target = Activator.CreateInstance("NLog", "NLog.Targets.ColoredConsoleTarget").Unwrap();
            dynamic loggingRule = Activator.CreateInstance("NLog", "NLog.Config.LoggingRule").Unwrap();

            target.Layout = "${level:uppercase=true} ${logger}: ${message}${onexception:inner=${newline}${exception:format=tostring}}";
            config.AddTarget("console", target);

            var logLevelType = Type.GetType("NLog.LogLevel, NLog");
            var enableLoggingForLevelMethod = Type.GetType("NLog.Config.LoggingRule, NLog").GetMethod("EnableLoggingForLevel");

            loggingRule.LoggerNamePattern = "*";
            loggingRule.Targets.Add(target);
            for (int i = 2; i < 6; i++)
            {
                var level = logLevelType.InvokeMember("FromOrdinal", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { i });
                enableLoggingForLevelMethod.Invoke(loggingRule, new object[] { level });
            }

            config.LoggingRules.Add(loggingRule);

            logManagerConfigProperty.SetValue(null, config, null);
        }
    }
}