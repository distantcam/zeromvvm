using System.Linq;
using Conventional;

namespace ZeroMVVM
{
    public static class AppRunner
    {
        public static IConventionManager ConventionManager { get; private set; }

        public static void Start<T>()
        {
            ConventionBuilder.Logger = Default.Logger;

            var builder = new ConventionBuilder();

            builder.Scan<T>()
                .For(Default.ViewConvention)
                .For(Default.ViewModelConvention);

            ConventionManager = builder.Build();

            var typesToRegister = ConventionManager.FindAll(Default.ViewConvention)
                .Concat(ConventionManager.FindAll(Default.ViewModelConvention))
                ;

            Default.SetupIoC(typesToRegister);

            var viewModel = Default.GetInstance(typeof(T));

            var windowManager = Default.GetInstance<IWindowManager>();

            windowManager.ShowWindow(viewModel);
        }
    }
}