using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ZeroMVVM.Dynamic
{
    public class AutofacRegistrationHelper : IDynamicMetaObjectProvider
    {
        private class MetaObject : DynamicMetaObject
        {
            private readonly object instance;
            private readonly Type type;

            public MetaObject(Expression expression, object value, object instance)
                : base(expression, BindingRestrictions.Empty, value)
            {
                this.instance = instance;
                this.type = instance.GetType();
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                PropertyInfo prop = type.GetProperty(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
                if (prop == null)
                    return base.BindGetMember(binder);

                return new DynamicMetaObject(
                    Expression.Property(Expression.Constant(instance), prop),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                Type[] argTypes = args.Select(a => a.RuntimeType).ToArray();
                MethodInfo method = type.GetMethod(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public, null, argTypes, null);

                var invokeArgs = Expression.NewArrayInit(typeof(object), args.Select(a => Expression.Convert(a.Expression, typeof(object))));

                if (method == null)
                    if (binder.Name == "As")
                    {
                        method = type.GetMethod(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(Type[]) }, null);
                        invokeArgs = Expression.NewArrayInit(typeof(object), Expression.NewArrayInit(typeof(Type), args.Select(a => Expression.Convert(a.Expression, typeof(Type)))));
                    }

                if (method == null)
                    return base.BindInvokeMember(binder, args);

                var parameters = new Expression[] { Expression.Constant(instance), invokeArgs };

                return new DynamicMetaObject(
                    Expression.Call(
                        Expression.Constant(method),
                        typeof(MethodInfo).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[]) }),
                        parameters),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }
        }

        private readonly object instance;

        public AutofacRegistrationHelper(object instance)
        {
            this.instance = instance;
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new MetaObject(parameter, this, instance);
        }
    }
}