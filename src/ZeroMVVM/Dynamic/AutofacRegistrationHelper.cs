using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ZeroMVVM.Dynamic
{
    internal class AutofacRegistrationHelper : IDynamicMetaObjectProvider
    {
        private class MetaObject : DynamicMetaObject
        {
            public MetaObject(Expression expression, object value)
                : base(expression, BindingRestrictions.Empty, value)
            {
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                Expression self = Expression.Convert(Expression, LimitType);

                Expression[] parameters = new Expression[] { Expression.Constant(binder.Name) };

                return new DynamicMetaObject(
                    Expression.Call(self, typeof(AutofacRegistrationHelper).GetMethod("GetMember", BindingFlags.NonPublic | BindingFlags.Instance), parameters),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                Expression self = Expression.Convert(Expression, LimitType);

                Expression[] parameters = new Expression[2];
                parameters[0] = Expression.Constant(binder.Name);
                parameters[1] = Expression.NewArrayInit(typeof(object), args.Select(dmo => dmo.Expression).ToArray());

                return new DynamicMetaObject(
                    Expression.Call(self, typeof(AutofacRegistrationHelper).GetMethod("InvokeMember", BindingFlags.NonPublic | BindingFlags.Instance), parameters),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }
        }

        private readonly object instance;
        private readonly Type type;

        public AutofacRegistrationHelper(object instance)
        {
            this.instance = instance;
            this.type = instance.GetType();
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new MetaObject(parameter, this);
        }

        private object GetMember(string name)
        {
            PropertyInfo prop = type.GetProperty(name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            if (prop == null)
                throw new NotSupportedException();

            return prop.GetValue(instance, null);
        }

        private object InvokeMember(string name, object[] args)
        {
            Type[] argTypes = args.Select(a => a.GetType()).ToArray();
            MethodInfo method = type.GetMethod(name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public, null, argTypes, null);

            if (method == null)
            {
                if (name == "As")
                {
                    method = type.GetMethod(name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(Type[]) }, null);
                    args = new object[] { args.OfType<Type>().ToArray() };
                }
            }

            if (method == null)
                throw new NotSupportedException();

            return method.Invoke(instance, args);
        }
    }
}