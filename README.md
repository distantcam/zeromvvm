ZeroMVVM
========

ZeroMVVM is a Model-View-ViewModel framework designed to just work with a minimum amount of setup.

ZeroMVVM is designed to work with [PropertyChanged.Fody](https://github.com/SimonCropp/PropertyChanged) and [PropertyChanging.Fody](https://github.com/SimonCropp/PropertyChanging)

[Fody Sample Usage](https://github.com/SimonCropp/Fody/wiki/SampleUsage)

Getting Started
----------------

First a simple example on how to start using ZeroMVVM.

    public partial class App : Application
    {
        public App()
        {
            ZAppRunner.Start<ShellViewModel>();
        }
    }

This is all you need to have ZeroMVVM start with the ShellViewModel. ZeroMVVM is ViewModel driven and thus you must provide a ViewModel first. It will go and find the matching View using conventions.

Logging
---------

ZeroMVVM has a built in logging system for all it's messages. But if for example you wanted to use NLog for logging you can tell ZeroMVVM to do that.

    public partial class App : Application
    {
        public App()
        {
            ZAppRunner.Default.Logger = l => LogManager.GetLogger(l);
        
            ZAppRunner.Start<ShellViewModel>();
        }
    }

ZeroMVVM will try to use whatever logger you give it, whether it's NLog, Log4Net or any other logger.

IoC Container
---------------

Internally ZeroMVVM uses an IoC container to loosely couple all the parts of your application (Views, ViewModels, etc.) together.

To use your own IoC simply provide it to ZeroMVVM.

    // Autofac
    ZAppRunner.Default.IoC = new Autofac.ContainerBuilder();
    
    // Ninject:
    ZAppRunner.Default.IoC = new Ninject.StandardKernel();
    
ZeroMVVM will use whatever IoC you give it in the most obvious way. If you wish to provide your own IoC container you can do so by providing an implementation of IContainer.

    ZAppRunner.Default.IoC = new MyCustomContainer();