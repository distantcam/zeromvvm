using System.Windows;
using Ninject;
using NLog;
using SimpleExample.UI.Shell;
using ZeroMVVM;

namespace NinjectExample
{
    public partial class App : Application
    {
        public App()
        {
            ZAppRunner.Default.Logger = l => LogManager.GetLogger(l);

            ZAppRunner.Default.IoC = new StandardKernel();

            ZAppRunner.Start<ShellViewModel>();
        }
    }
}