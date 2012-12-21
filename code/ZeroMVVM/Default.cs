using System;
using System.Collections.Generic;
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

        public static object GetInstance(Type type)
        {
            return Activator.CreateInstance(type);
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

            target.Layout = "${level:uppercase=true} ${logger}: ${message}${onexception:inner=${newline}${exception:format=tostring}}";
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

        internal static void SetupIoC(IEnumerable<Type> typesToRegister)
        {
            if (IoC == null)
                return;

            if (IoC.GetType().Namespace == "Autofac" && IoC.GetType().Name == "ContainerBuilder")
                ConfigureDefaultAutofac(typesToRegister);
        }

        private static void ConfigureDefaultAutofac(IEnumerable<Type> typesToRegister)
        {
            dynamic registrationExtensions = new StaticMembersDynamicWrapper(Type.GetType("Autofac.RegistrationExtensions, Autofac"));

            foreach (var type in typesToRegister)
            {
                registrationExtensions.AsSelf(registrationExtensions.RegisterType(IoC, type));
            }

            //IoC.RegisterType(type)
            //    .AsSelf()
            //    .InstancePerDependency();
        }
    }
}