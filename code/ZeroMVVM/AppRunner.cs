using System.Linq;
using System.Windows;
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

            var viewType = ConventionManager.FindAll(Default.ViewConvention, typeof(T)).Single();

            var view = Default.GetInstance(viewType);

            ((Window)view).Show();
        }
    }
}