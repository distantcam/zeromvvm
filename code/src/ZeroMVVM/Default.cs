using System;
using System.Collections.Generic;
using System.Linq;
using ZeroMVVM.Conventions;
using ZeroMVVM.Dynamic;

namespace ZeroMVVM
{
    public static class Default
    {
        private static Func<string, dynamic> logger;
        private static IContainer container;

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

        internal static T GetInstance<T>()
        {
            return (T)container.GetInstance(typeof(T));
        }

        internal static object GetInstance(Type type)
        {
            return container.GetInstance(type);
        }

        internal static void SetupIoC(IEnumerable<Type> typesToRegister, IEnumerable<Type> viewModelTypesToRegister)
        {
            if (IoC == null)
            {
                container = new Container(viewModelTypesToRegister);
                return;
            }

            container = IoC as IContainer;
            if (container != null)
                return;

            if (IoC.GetType().Namespace == "Autofac" && IoC.GetType().Name == "ContainerBuilder")
            {
                container = new AutofacContainer(Default.IoC, typesToRegister, viewModelTypesToRegister);
            }
        }

        private static void ConfigureDefaultAutofac(IEnumerable<Type> typesToRegister)
        {
            dynamic registrationExtensions = new StaticMembersDynamicWrapper(Type.GetType("Autofac.RegistrationExtensions, Autofac"));

            dynamic registration;
            Type limitType;

            foreach (var type in typesToRegister)
            {
                // IoC.RegisterType(type)
                registration = new AutofacRegistrationHelper(registrationExtensions.RegisterType(IoC, type));

                // AsSelf()
                limitType = registration.ActivatorData.Activator.LimitType;
                registration.As(limitType);
            }

            // IoC.RegisterType(typeof(WindowManager))
            registration = new AutofacRegistrationHelper(registrationExtensions.RegisterType(IoC, typeof(WindowManager)));

            // AsImplementedInterfaces()
            limitType = registration.ActivatorData.Activator.LimitType;
            var interfaces = limitType.GetInterfaces().Where((Func<Type, bool>)(t => t != typeof(IDisposable))).ToArray();
            registration = new AutofacRegistrationHelper(registration.As(interfaces));

            // SingleInstance()
            registration.SingleInstance();
        }
    }
}