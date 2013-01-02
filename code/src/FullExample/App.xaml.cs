﻿using System.Windows;
using Autofac;
using NLog;
using SimpleExample.UI.Shell;
using ZeroMVVM;

namespace FullExample
{
    public partial class App : Application
    {
        public App()
        {
            ZAppRunner.Default.Logger = l => LogManager.GetLogger(l);

            ZAppRunner.Default.IoC = new ContainerBuilder();

            ZAppRunner.Start<ShellViewModel>();
        }
    }
}