using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace ZeroMVVM.Dynamic
{
    internal class AutofacContainer : DynamicObject, IContainer
    {
        private dynamic internalContainer;
        private MethodInfo resolveMethod;

        public AutofacContainer(dynamic builder, IEnumerable<Type> typesToRegister, IEnumerable<Type> viewModelTypesToRegister)
        {
            SetupDefaults(typesToRegister, viewModelTypesToRegister);

            internalContainer = builder.Build();

            resolveMethod = Type.GetType("Autofac.ResolutionExtensions, Autofac")
                .GetMethod("Resolve", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static, null, new Type[] { internalContainer.GetType(), typeof(Type) }, null);
        }

        private void SetupDefaults(IEnumerable<Type> typesToRegister, IEnumerable<Type> viewModelTypesToRegister)
        {
            dynamic registrationExtensions = new StaticMembersDynamicWrapper(Type.GetType("Autofac.RegistrationExtensions, Autofac"));
            dynamic resolutionExtensions = new StaticMembersDynamicWrapper(Type.GetType("Autofac.ResolutionExtensions, Autofac"));

            dynamic registration;
            Type limitType;

            foreach (var type in typesToRegister)
            {
                // IoC.RegisterType(type)
                registration = new AutofacRegistrationHelper(registrationExtensions.RegisterType(Default.IoC, type));

                // AsSelf()
                limitType = registration.ActivatorData.Activator.LimitType;
                registration.As(limitType);
            }

            foreach (var type in viewModelTypesToRegister)
            {
                // IoC.RegisterType(type)
                registration = new AutofacRegistrationHelper(registrationExtensions.RegisterType(Default.IoC, type));

                // AsSelf()
                limitType = registration.ActivatorData.Activator.LimitType;
                registration.As(limitType);

                // Add attachments on activation
                registration.OnActivating((Action<dynamic>)(e =>
                {
                    var vmType = (Type)e.Instance.GetType();

                    var attachments = AppRunner.ConventionManager.FindAll(Default.AttachmentConvention, vmType)
                        .Where(t => resolutionExtensions.IsRegistered(e.Context, t))
                        .Select(t => (IAttachment)resolutionExtensions.Resolve(e.Context, t));

                    foreach (var a in attachments)
                        a.AttachTo(e.Instance);
                }));
            }

            // IoC.RegisterType(typeof(WindowManager))
            registration = new AutofacRegistrationHelper(registrationExtensions.RegisterType(Default.IoC, typeof(WindowManager)));

            // AsImplementedInterfaces()
            limitType = registration.ActivatorData.Activator.LimitType;
            var interfaces = limitType.GetInterfaces().Where((Func<Type, bool>)(t => t != typeof(IDisposable))).ToArray();
            registration = new AutofacRegistrationHelper(registration.As(interfaces));

            // SingleInstance()
            registration.SingleInstance();
        }

        public object GetInstance(Type type)
        {
            return resolveMethod.Invoke(null, new object[] { internalContainer, type });
        }
    }
}