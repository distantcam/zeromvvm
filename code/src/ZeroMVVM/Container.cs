using System;
using System.Collections.Generic;
using TinyIoC;

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
        private TinyIoCContainer container = TinyIoCContainer.Current;

        public void Setup(IEnumerable<Type> typesToRegister, IEnumerable<Type> viewModelTypesToRegister)
        {
            this.viewModelTypes = new HashSet<Type>(viewModelTypesToRegister);
        }

        public object GetInstance(Type type)
        {
            var instance = container.Resolve(type);

            if (viewModelTypes.Contains(type))
            {
                var attachments = ZAppRunner.ConventionManager.FindAll(ZAppRunner.Default.AttachmentConvention, type);
                if (attachments != null)
                    foreach (var a in attachments)
                        ((IAttachment)container.Resolve(a)).AttachTo(instance);
            }

            return instance;
        }
    }
}