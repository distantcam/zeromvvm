using System.Windows;
using Autofac;
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

            var builder = new ContainerBuilder();

            //Default.IoC = builder;

            AppRunner.Start<ShellViewModel>();
        }
    }
}