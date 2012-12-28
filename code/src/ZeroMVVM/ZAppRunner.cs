using System;
using System.Linq;
using Conventional;
using ZeroMVVM.Dynamic;

namespace ZeroMVVM
{
    public static class ZAppRunner
    {
        private static IContainer container;
        private static Logger Log;

        public static IConventionManager ConventionManager { get; private set; }

        public static void Start<T>()
        {
            Log = GetLogger(typeof(ZAppRunner));

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

            CreateIoC();

            container.Setup(typesToRegister, ConventionManager.FindAll(Default.ViewModelConvention));

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

        private static void CreateIoC()
        {
            container = Default.IoC as IContainer;
            if (container != null)
                return;

            if (Default.IoC == null)
            {
                container = new Container();
                return;
            }

            var iocType = (Type)Default.IoC.GetType();

            if (iocType.Namespace.StartsWith("Autofac"))
            {
                if (iocType.Name != "ContainerBuilder")
                {
                    Log.Error("To provide a default Autofac container set Default.IoC to a ContainerBuilder, or provide a custom IContainer.");
                    throw new NotSupportedException();
                }
                container = new AutofacContainer(Default.IoC);
                return;
            }

            if (iocType.Namespace.StartsWith("Ninject"))
            {
                if (iocType.Name != "IKernel" && iocType.GetInterface("IKernel") == null)
                {
                    Log.Error("To provide a default Ninject container set Default.IoC to an IKernel, or provide a custom IContainer.");
                    throw new NotSupportedException();
                }
                container = new NinjectContainer(Default.IoC);
                return;
            }

            if (container == null)
            {
                Log.Error("Could not set up IoC correctly. Consider providing a custom implementation of IContainer for your IoC.");
                throw new NotSupportedException();
            }
        }
    }
}