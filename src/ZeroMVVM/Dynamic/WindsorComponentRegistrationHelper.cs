using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ZeroMVVM.Dynamic
{
    internal class WindsorComponentRegistrationHelper : IDynamicMetaObjectProvider
    {
        private class MetaObject : DynamicMetaObject
        {
            public MetaObject(Expression expression, object value)
                : base(expression, BindingRestrictions.Empty, value)
            {
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                Expression self = Expression.Convert(Expression, LimitType);

                Expression[] parameters = new Expression[2];
                parameters[0] = Expression.Constant(binder.Name);
                parameters[1] = Expression.NewArrayInit(typeof(object), args.Select(dmo => dmo.Expression).ToArray());

                return new DynamicMetaObject(
                    Expression.Call(self, typeof(WindsorComponentRegistrationHelper).GetMethod("InvokeMember", BindingFlags.NonPublic | BindingFlags.Instance), parameters),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }
        }

        private readonly dynamic instance;
        private readonly Type type;

        public WindsorComponentRegistrationHelper(dynamic instance)
        {
            this.instance = instance;
            this.type = instance.GetType();
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new MetaObject(parameter, this);
        }

        private static dynamic Cast(Delegate source, Type type)
        {
            Delegate[] delegates = source.GetInvocationList();
            if (delegates.Length == 1)
                return Delegate.CreateDelegate(type,
                    delegates[0].Target, delegates[0].Method);

            Delegate[] delegatesDest = new Delegate[delegates.Length];
            for (int nDelegate = 0; nDelegate < delegates.Length; nDelegate++)
                delegatesDest[nDelegate] = Delegate.CreateDelegate(type,
                    delegates[nDelegate].Target, delegates[nDelegate].Method);
            return Delegate.Combine(delegatesDest);
        }

        private object InvokeMember(string name, object[] args)
        {
            Type[] argTypes = args.Select(a => a.GetType()).ToArray();
            MethodInfo method = type.GetMethod(name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public, null, argTypes, null);

            if (method == null)
            {
                if (name == "OnCreate")
                {
                    var delegateType = Type.GetType("Castle.MicroKernel.LifecycleConcerns.LifecycleActionDelegate`1, Castle.Windsor");
                    delegateType = delegateType.MakeGenericType(typeof(object));
                    var delegateTypeArray = delegateType.MakeArrayType();

                    method = type.GetMethod(name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public, null, new Type[] { delegateTypeArray }, null);
                    var newargs = Array.CreateInstance(delegateType, args.Length);
                    for (int i = 0; i < args.Length; i++)
                        newargs.SetValue(Cast((Delegate)args[i], delegateType), i);
                    args = new object[] { newargs };
                }
            }

            if (method == null)
                throw new NotSupportedException();

            return method.Invoke(instance, args);
        }
    }
}