using System;
using System.Collections.Generic;

namespace ZeroMVVM.Dynamic
{
    internal class NinjectContainer : IContainer
    {
        private static dynamic resolutionExtensions = new StaticMembersDynamicWrapper(Type.GetType("Ninject.ResolutionExtensions, Ninject"));

        private readonly dynamic kernel;

        private HashSet<Type> viewModelTypes;

        public NinjectContainer(dynamic kernel)
        {
            this.kernel = kernel;
        }

        public void Setup(IEnumerable<Type> typesToRegister, IEnumerable<Type> viewModelTypesToRegister)
        {
            this.viewModelTypes = new HashSet<Type>(viewModelTypesToRegister);
        }

        public object GetInstance(Type type)
        {
            var instance = resolutionExtensions.Get(kernel, type, Array.CreateInstance(Type.GetType("Ninject.Parameters.IParameter, Ninject"), 0));

            if (viewModelTypes.Contains(type))
            {
                var attachments = ZAppRunner.ConventionManager.FindAll(Default.AttachmentConvention, type);
                if (attachments != null)
                    foreach (var a in attachments)
                        ((IAttachment)GetInstance(a)).AttachTo(instance);
            }

            return instance;
        }
    }
}