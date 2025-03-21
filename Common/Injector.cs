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
            Add(this);
        }

        public void Add<T>(T value) where T : class
        {
            Assert.IsNotNull(value);

            if (values.ContainsKey(typeof(T)))
                throw new Exception($"Value for type '{nameof(T)}' already registered");

            values[typeof(T)] = value;
        }

        public void Resolve<T>(T obj) where T : class
        {
            Assert.IsNotNull(obj);
            ResolveFields(obj);
            ResolveProperties(obj);
        }

        public void ResolveFields<T>(T obj)
        {
            System.Reflection.FieldInfo[] fields = obj
                .GetType()
                .GetFields(System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                InjectAttribute injectAttrribute = field.GetCustomAttribute<InjectAttribute>();
                if (injectAttrribute == null)
                    continue;

                if (values.TryGetValue(field.FieldType, out var injectValue))
                {
                    field.SetValue(obj, injectValue);
                }
                else
                {
                    throw new Exception($"Value for inject not found. Field: '{field.FieldType.Name} {field.DeclaringType.Name}.{field.Name}'");
                }
            }
        }

        public void ResolveProperties<T>(T obj)
        {
            var properties = obj
                .GetType()
                .GetProperties(System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

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