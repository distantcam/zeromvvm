using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ZeroMVVM.Dynamic
{
    public class StaticMembersDynamicWrapper : IDynamicMetaObjectProvider
    {
        private class MetaObject : DynamicMetaObject
        {
            private readonly Type type;

            public MetaObject(Expression expression, object value, Type type)
                : base(expression, BindingRestrictions.Empty, value)
            {
                this.type = type;
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                PropertyInfo prop = type.GetProperty(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
                if (prop == null)
                    return base.BindGetMember(binder);

                return new DynamicMetaObject(
                    Expression.Property(null, prop),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                PropertyInfo prop = type.GetProperty(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
                if (prop == null)
                    return base.BindSetMember(binder, value);

                return new DynamicMetaObject(
                    Expression.Assign(
                        Expression.Property(null, prop),
                        Expression.Convert(value.Expression, prop.PropertyType)),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                Type[] argTypes = args.Select(a => a.RuntimeType).ToArray();
                MethodInfo method = type.GetMethod(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public, null, argTypes, null);

                if (method == null)
                    return base.BindInvokeMember(binder, args);

                var parameters = new Expression[] { Expression.Constant(null), Expression.NewArrayInit(typeof(object), args.Select(a => Expression.Convert(a.Expression, typeof(object)))) };

                return new DynamicMetaObject(
                    Expression.Call(
                        Expression.Constant(method),
                        typeof(MethodInfo).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[]) }),
                        parameters),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }
        }

        private Type type;

        public StaticMembersDynamicWrapper(Type type)
        {
            this.type = type;
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new MetaObject(parameter, this, type);
        }
    }
}