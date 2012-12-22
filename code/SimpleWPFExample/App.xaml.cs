﻿using System.Windows;
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

            Default.IoC = new ContainerBuilder();

            AppRunner.Start<ShellViewModel>();
        }
    }
}