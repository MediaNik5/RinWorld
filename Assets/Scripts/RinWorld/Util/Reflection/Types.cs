using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace RinWorld.Util.Reflection
{
    public static class Types
    {
        /// <summary>
        /// Searches for static methods named <c>methodName</c> of all <c>TType</c> subtypes
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="bindingFlags">Addition binding flags</param>
        /// <typeparam name="TType">Type of which subtypes to search for</typeparam>
        /// <typeparam name="TDelegate">Signature of method to search for</typeparam>
        /// <returns>Dictionary containing (type name, static method named <c>methodName</c> of that type) pairs</returns>
        
        public static Dictionary<string, TDelegate> GetStaticMethodsOfAllSubtypes<TType, TDelegate>(
            string methodName, 
            BindingFlags bindingFlags
        ) where TDelegate : Delegate => 
            GetStaticMethodsOfAllSubtypes<TType, string, TDelegate>(methodName, type => type.Name, bindingFlags);
        
        /// <summary>
        /// Searches for static methods named <c>methodName</c> of all <c>TType</c> subtypes
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="keySelector">Used to choose key for this key-value-pair with <c>TDelegate</c> of this type as value</param>
        /// <param name="bindingFlags">Addition binding flags</param>
        /// <typeparam name="TType">Type of which subtypes to search for</typeparam>
        /// <typeparam name="TKey">Key used in Dictionary</typeparam>
        /// <typeparam name="TDelegate">Signature of method to search for</typeparam>
        /// <returns>Dictionary containing (TKey from <c>keySelector</c>, static method named <c>methodName</c> of type used for <c>keySelector</c>)
        /// pairs</returns>
        public static Dictionary<TKey, TDelegate> GetStaticMethodsOfAllSubtypes<TType, TKey, TDelegate>(
            string methodName, 
            Func<Type, TKey> keySelector,
            BindingFlags bindingFlags
        ) where TDelegate : Delegate
        {
            var types = GetAllSubtypes<TType>();
            var staticMethods = new Dictionary<TKey, TDelegate>(types.Length);
            foreach (var type in types)
            {
                var @delegate = GetMethodAsDelegateOrNull<TDelegate>(
                    type,
                    methodName,
                    BindingFlags.Static | bindingFlags
                );
                if(@delegate != null)
                    staticMethods.Add(
                        keySelector(type),
                        @delegate
                    );
            }

            return staticMethods;
        }
        
        public static Dictionary<string, TValue> GetInstancesOfAllSubtypes<TValue>() =>
            GetInstancesOfAllSubtypes<TValue>(v => v.GetType().Name);
        
        public static Dictionary<string, TValue> GetInstancesOfAllSubtypes<TValue>(Func<TValue, string> keySelector) => 
            GetInstancesOfAllSubtypes<string, TValue>(keySelector);
        
        public static Dictionary<TKey, TValue> GetInstancesOfAllSubtypes<TKey, TValue>(Func<TValue, TKey> keySelector)
        {
            var types = GetAllSubtypes<TValue>();
            var instances = new Dictionary<TKey, TValue>(types.Length);
            for (int i = 0; i < types.Length; i++)
            {
                var instance = (TValue) Activator.CreateInstance(types[i], true);
                instances[keySelector(instance)] = instance;
            }

            return instances;
        }
        
        public static System.Type[] GetAllSubtypes<Type>() => 
            GetAllSubtypes(typeof(Type));
        
        public static Type[] GetAllSubtypes(Type superType)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where superType.IsInterface ? 
                        type.GetInterface(superType.FullName) != null : 
                        type.IsSubclassOf(superType)
                    select type).ToArray();
        }
        [CanBeNull]
        public static TDelegate GetMethodAsDelegateOrNull<TDelegate>(
            Type clazz, 
            string name, 
            BindingFlags flags
        ) where  TDelegate : Delegate
        {
            MethodInfo method = clazz.GetMethod(name, flags);
            
            if (ReferenceEquals(method, null))
                return null;

            return CreateStaticDelegate<TDelegate>(method);
        }
        [CanBeNull]
        public static MethodInfo GetMethodOrNull(
            Type clazz, 
            string name, 
            BindingFlags flags, 
            Type returnType, 
            params Type[] parameters
        )
        {
            MethodInfo method = clazz.GetMethod(name, flags);
            
            if (method == null ||
                method.ReturnType != returnType ||
                method.GetParameters().Length != parameters.Length ||
                !method.GetParameters().Select(info => info.ParameterType).SequenceEqual(parameters))
            {
                return null;
            }

            return method;
        }

        public static T CreateStaticDelegate<T>([NotNull] MethodInfo method) where T : Delegate
        {
            return Delegate.CreateDelegate(typeof(T), method,  false) as T;
        }
    }
}