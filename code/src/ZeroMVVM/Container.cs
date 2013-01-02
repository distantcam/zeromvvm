using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeroMVVM
{
    public interface IContainer
    {
        void Setup(IEnumerable<Type> typesToRegister, IEnumerable<Type> viewModelTypesToRegister);

        object GetInstance(Type type);
    }

    internal class Container : IContainer
    {
        private HashSet<Type> viewModelTypes;

        public void Setup(IEnumerable<Type> typesToRegister, IEnumerable<Type> viewModelTypesToRegister)
        {
            this.viewModelTypes = new HashSet<Type>(viewModelTypesToRegister);
        }

        public object GetInstance(Type type)
        {
            var ctor = type.GetConstructors().OrderByDescending(ci => ci.GetParameters().Length).First();

            var args = ctor.GetParameters().Select(pi => GetInstance(pi.ParameterType)).ToArray();

            var instance = Activator.CreateInstance(type, args);

            if (viewModelTypes.Contains(type))
            {
                var attachments = ZAppRunner.ConventionManager.FindAll(ZAppRunner.Default.AttachmentConvention, type);
                if (attachments != null)
                    foreach (var a in attachments)
                        ((IAttachment)GetInstance(a)).AttachTo(instance);
            }

            return instance;
        }
    }
}