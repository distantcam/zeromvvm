﻿using System.Linq;
using Conventional;

namespace ZeroMVVM
{
    public static class ZAppRunner
    {
        public static IConventionManager ConventionManager { get; private set; }

        public static void Start<T>()
        {
            ConventionBuilder.Logger = Default.Logger;

            var builder = new ConventionBuilder();

            builder.Scan<T>()
                .For(Default.AttachmentConvention)
                .For(Default.ViewConvention)
                .For(Default.ViewModelConvention);

            ConventionManager = builder.Build();

            var typesToRegister = ConventionManager.FindAll(Default.ViewConvention)
                .Concat(ConventionManager.FindAll(Default.AttachmentConvention))
                ;

            Default.SetupIoC(typesToRegister, ConventionManager.FindAll(Default.ViewModelConvention));

            var viewModel = Default.GetInstance(typeof(T));

            WindowManager.ShowWindow(viewModel);
        }
    }
}