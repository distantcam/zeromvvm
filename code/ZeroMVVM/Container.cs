using System;
using System.Dynamic;
using System.Reflection;

namespace ZeroMVVM
{
    internal interface IContainer
    {
        object GetInstance(Type type);
    }

    internal class Container : IContainer
    {
        public object GetInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }

    internal class AutofacContainer : DynamicObject, IContainer
    {
        private dynamic internalContainer;
        private MethodInfo resolveMethod;

        public AutofacContainer(dynamic builder)
        {
            internalContainer = builder.Build();

            resolveMethod = Type.GetType("Autofac.ResolutionExtensions, Autofac")
                .GetMethod("Resolve", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static, null, new Type[] { internalContainer.GetType(), typeof(Type) }, null);
        }

        public object GetInstance(Type type)
        {
            return resolveMethod.Invoke(null, new object[] { internalContainer, type });
        }
    }
}