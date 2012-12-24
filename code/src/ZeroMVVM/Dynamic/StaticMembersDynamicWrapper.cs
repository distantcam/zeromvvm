using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace ZeroMVVM.Dynamic
{
    public class StaticMembersDynamicWrapper : DynamicObject
    {
        private Type _type;

        public StaticMembersDynamicWrapper(Type type)
        {
            _type = type;
        }

        // Handle static properties
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            PropertyInfo prop = _type.GetProperty(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
            if (prop == null)
            {
                result = null;
                return false;
            }

            result = prop.GetValue(null, null);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            PropertyInfo prop = _type.GetProperty(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
            if (prop == null)
                return false;

            prop.SetValue(null, value, null);
            return true;
        }

        // Handle static methods
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Type[] argTypes = args.Select(a => a.GetType()).ToArray();
            MethodInfo method = _type.GetMethod(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public, null, argTypes, null);

            if (method == null)
            {
                result = null;
                return false;
            }

            result = method.Invoke(null, args);
            return true;
        }
    }
}