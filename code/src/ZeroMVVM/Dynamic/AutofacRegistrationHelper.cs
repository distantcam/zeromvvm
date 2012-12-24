using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace ZeroMVVM.Dynamic
{
    public class AutofacRegistrationHelper : DynamicObject
    {
        private readonly dynamic instance;
        private readonly Type type;

        public AutofacRegistrationHelper(dynamic instance)
        {
            this.instance = instance;
            this.type = instance.GetType();
        }

        public dynamic Instance { get { return instance; } }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            PropertyInfo prop = type.GetProperty(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            if (prop == null)
            {
                result = null;
                return false;
            }

            result = prop.GetValue(instance, null);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Type[] argTypes = args
                .Select(a => a.GetType())
                .ToArray();
            MethodInfo method = type.GetMethod(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public, null, argTypes, null);

            if (method == null)
            {
                if (binder.Name == "As")
                {
                    method = type.GetMethod(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(Type[]) }, null);
                    args = new object[] { args.OfType<Type>().ToArray() };
                }
            }

            if (method == null)
            {
                result = null;
                return false;
            }

            result = method.Invoke(instance, args);
            return true;
        }
    }
}