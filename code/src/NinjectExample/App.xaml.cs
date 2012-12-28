using System;
using System.Collections.Generic;
using System.Windows;
using Ninject;
using NLog;
using SimpleExample.UI.Shell;
using ZeroMVVM;

namespace NinjectExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Default.Logger = l => LogManager.GetLogger(l);

            Default.IoC = new StandardKernel();

            ZAppRunner.Start<ShellViewModel>();
        }
    }

    public class NinjectWrapper : IContainer
    {
        private readonly IKernel kernel;

        public NinjectWrapper()
        {
            var settings = new NinjectSettings();

            kernel = new StandardKernel(settings);
        }

        public void Setup(IEnumerable<Type> typesToRegister, IEnumerable<Type> viewModelTypesToRegister)
        {
        }

        public object GetInstance(Type type)
        {
            return kernel.Get(type);
        }
    }
}