using System.Linq;
using System.Windows;
using Conventional;
using ZeroMVVM.Conventions;

namespace ZeroMVVM
{
    public static class AppRunner
    {
        public static IConventionManager ConventionManager { get; private set; }

        public static void Start<T>()
        {
            var builder = new ConventionBuilder();

            builder.Scan<T>()
                .For<ViewModelConvention>()
                .For<ViewConvention>();

            ConventionManager = builder.Build();

            var viewType = ConventionManager.FindAll<ViewConvention>(typeof(T)).Single();

            var view = System.Activator.CreateInstance(viewType);

            ((Window)view).Show();
        }
    }
}