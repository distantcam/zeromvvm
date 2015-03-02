using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeroMVVM.Dynamic
{
    internal class WindsorContainer : IContainer
    {
        public delegate void LifecycleActionDelegate<T>(dynamic kernel, T item);

        private readonly dynamic container;

        public WindsorContainer(dynamic container)
        {
            this.container = container;
        }

        public void Setup(IEnumerable<Type> typesToRegister, IEnumerable<Type> viewModelTypesToRegister)
        {
            dynamic component = new StaticMembersDynamicWrapper(Type.GetType("Castle.MicroKernel.Registration.Component, Castle.Windsor"));

            foreach (var item in typesToRegister)
                container.Register(component.For(item));

            foreach (var item in viewModelTypesToRegister)
            {
                dynamic registration = new WindsorComponentRegistrationHelper(component.For(item));
                container.Register(registration.OnCreate((LifecycleActionDelegate<object>)((kernel, instance) =>
                {
                    var vmType = (Type)instance.GetType();

                    var attachments = ZAppRunner.ConventionManager.FindAll(ZAppRunner.Default.AttachmentConvention, vmType)
                        .Where(t => kernel.HasComponent(t))
                        .Select(t => (IAttachment)kernel.Resolve(t));

                    foreach (var a in attachments)
                        a.AttachTo(instance);
                })));
            }
        }

        public object GetInstance(Type type)
        {
            return container.Resolve(type);
        }
    }
}