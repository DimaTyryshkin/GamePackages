using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Assertions;

namespace GamePackages.Core
{
    [AttributeUsage(validOn: AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class InjectAttribute : Attribute
    {
    }

    public class Injector
    {
        Dictionary<Type, object> values = new Dictionary<Type, object>();

        public Injector()
        {
            Register(this);
        }

        public T RegisterAndInject<T>(T value, params object[] parameters) where T : class
        {
            Register(value);
            Inject(value, parameters);
            return value;
        }

        public T Register<T>(T value) where T : class
        {
            Assert.IsNotNull(value);

            if (values.ContainsKey(typeof(T)))
                throw new Exception($"Value for type '{nameof(T)}' already registered");

            values[typeof(T)] = value;

            return value;
        }

        public T Inject<T>(T obj, params object[] parameters) where T : class
        {
            Assert.IsNotNull(obj);
            ResolveFields(obj, parameters);
            ResolveProperties(obj);

            return obj;
        }

        private void ResolveFields<T>(T obj, object[] parameters)
        {
            FieldInfo[] fields = obj
                .GetType()
                .GetFields(BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                InjectAttribute injectAttrribute = field.GetCustomAttribute<InjectAttribute>();
                if (injectAttrribute == null)
                    continue;

                if (parameters is not null)
                {
                    object injectValue1 = GetByType(field.FieldType, parameters);
                    if (injectValue1 is not null)
                    {
                        field.SetValue(obj, injectValue1);
                        continue;
                    }
                }

                if (values.TryGetValue(field.FieldType, out var injectValue2))
                {

                    field.SetValue(obj, injectValue2);
                    continue;
                }

                throw new Exception($"Value of type '<b>{field.FieldType.Name}</b>' not found for inject in member '<b>{field.DeclaringType.Name}.{field.Name}</b>'");
            }

            if (parameters is not null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i] is not null)
                        throw new Exception($"Parameter[{i}] with type '{parameters[i].GetType().Name}' was not used in inject to <b>{obj.GetType().Name}</b>");
                }
            }
        }

        private object GetByType(Type type, object[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                object item = parameters[i];
                if (item is not null && item.GetType() == type)
                {
                    parameters[i] = null;
                    return item;
                }
            }

            return null;
        }


        private void ResolveProperties<T>(T obj)
        {
            var properties = obj
                .GetType()
                .GetProperties(BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

            foreach (var p in properties)
            {
                InjectAttribute injectAttrribute = p.GetCustomAttribute<InjectAttribute>();
                if (injectAttrribute == null)
                    continue;

                if (values.TryGetValue(p.PropertyType, out var injectValue))
                {
                    p.SetValue(obj, injectValue);
                }
                else
                {
                    throw new Exception($"Value for inject not found. Field: '{p.PropertyType.Name} {p.DeclaringType.Name}.{p.Name}'");
                }
            }
        }
    }
}