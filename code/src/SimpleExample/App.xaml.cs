using System.Windows;
using SimpleExample.UI.Shell;
using ZeroMVVM;

namespace SimpleExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppRunner.Start<ShellViewModel>();
        }
    }
}