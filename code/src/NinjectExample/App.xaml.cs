﻿using System.Windows;
using Ninject;
using SimpleExample.UI.Shell;
using ZeroMVVM;

namespace NinjectExample
{
    public partial class App : Application
    {
        public App()
        {
            ZAppRunner.Default.IoC = new StandardKernel();

            ZAppRunner.Start<ShellViewModel>();
        }
    }
}