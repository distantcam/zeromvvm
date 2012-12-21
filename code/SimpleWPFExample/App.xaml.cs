using System.Windows;
using SimpleWPFExample.UI.Shell;
using ZeroMVVM;

namespace SimpleWPFExample
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