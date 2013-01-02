using System.Windows;
using Castle.Windsor;
using SimpleExample.UI.Shell;
using ZeroMVVM;

namespace WindsorExample
{
    public partial class App : Application
    {
        public App()
        {
            ZAppRunner.Default.IoC = new WindsorContainer();

            ZAppRunner.Start<ShellViewModel>();
        }
    }
}