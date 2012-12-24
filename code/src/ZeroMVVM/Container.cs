using System;
using System.Collections.Generic;

namespace ZeroMVVM
{
    internal interface IContainer
    {
        object GetInstance(Type type);
    }

    internal class Container : IContainer
    {
        private HashSet<Type> viewModelTypes;

        public Container(IEnumerable<Type> viewModelTypes)
        {
            this.viewModelTypes = new HashSet<Type>(viewModelTypes);
        }

        public object GetInstance(Type type)
        {
            if (type == typeof(IWindowManager))
                return new WindowManager();

            var instance = Activator.CreateInstance(type);

            if (viewModelTypes.Contains(type))
            {
                var attachments = AppRunner.ConventionManager.FindAll(Default.AttachmentConvention, type);
                if (attachments != null)
                    foreach (var a in attachments)
                        ((IAttachment)Activator.CreateInstance(a)).AttachTo(instance);
            }

            return instance;
        }
    }
}