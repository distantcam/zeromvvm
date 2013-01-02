using System.Windows;
using SimpleExample.UI.Shell;
using ZeroMVVM;

namespace SimpleExample
{
    public partial class App : Application
    {
        public App()
        {
            ZAppRunner.Start<ShellViewModel>();
        }
    }
}