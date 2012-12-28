using System;
using System.Collections.Generic;
using System.Linq;
using Conventional;
using ZeroMVVM.Dynamic;

namespace ZeroMVVM
{
    public static class ZAppRunner
    {
        private static IContainer container;

        public static IConventionManager ConventionManager { get; private set; }

        public static void Start<T>()
        {
            ConventionBuilder.Logger = Default.Logger;

            var builder = new ConventionBuilder();

            builder.Scan<T>()
                .For(Default.AttachmentConvention)
                .For(Default.ViewConvention)
                .For(Default.ViewModelConvention);

            ConventionManager = builder.Build();

            var typesToRegister = ConventionManager.FindAll(Default.ViewConvention)
                .Concat(ConventionManager.FindAll(Default.AttachmentConvention))
                ;

            SetupIoC(typesToRegister, ConventionManager.FindAll(Default.ViewModelConvention));

            var viewModel = GetInstance(typeof(T));

            WindowManager.ShowWindow(viewModel);
        }

        public static Logger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public static Logger GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        public static Logger GetLogger(string name)
        {
            return new LogWrapper(Default.Logger(name));
        }

        internal static T GetInstance<T>()
        {
            return (T)container.GetInstance(typeof(T));
        }

        internal static object GetInstance(Type type)
        {
            return container.GetInstance(type);
        }

        internal static object GetViewForViewModel(object viewModel)
        {
            var viewType = ConventionManager.FindAll(Default.ViewConvention, viewModel).Single();

            return GetInstance(viewType);
        }

        private static void SetupIoC(IEnumerable<Type> typesToRegister, IEnumerable<Type> viewModelTypesToRegister)
        {
            if (Default.IoC == null)
            {
                container = new Container(viewModelTypesToRegister);
                return;
            }

            container = Default.IoC as IContainer;
            if (container != null)
                return;

            if (Default.IoC.GetType().Namespace == "Autofac" && Default.IoC.GetType().Name == "ContainerBuilder")
            {
                container = new AutofacContainer(Default.IoC, typesToRegister, viewModelTypesToRegister);
            }
        }
    }
}