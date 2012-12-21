using System.Windows;
using NLog;
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
            Default.Logger = l => LogManager.GetLogger(l);

            AppRunner.Start<ShellViewModel>();
        }
    }
}