using System;
using System.Collections.Generic;

namespace ServiceBroker.Common
{
    /// <summary>
    /// Various Type Utilities
    /// </summary>
    public static class TypeUtils
    {

        private static Dictionary<string,Type> typeCache = new Dictionary<string,Type>();

        /// <summary>
        /// Loads a Type from a qualified type name, then creates an instance of the type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qualifiedTypeName"></param>
        /// <returns></returns>
        /// <param name="args">Any arguments to pass to the type's constructor.</param>
        public static T LoadTypeAndCreateInstance<T>(string qualifiedTypeName, params object[] args)
            where T : class
        {
            Ensure.ParameterIsNotNull(qualifiedTypeName, "qualifiedTypeName");
            
            Type t = GetType(qualifiedTypeName);

            T instance =  Activator.CreateInstance(t,args) as T;

            if(instance == null)
            {
                throw new ServiceBrokerException(string.Format("Cannot cast type '{0}' to expected type '{1}'.", t.FullName, typeof (T).FullName));
            }

            return instance;
        }


        public static T CreateInstanceOfType<T>(Type type,params object[] args)
        {
            Ensure.ParameterIsNotNull(type, "type");

            if( typeof(T) != type &&  !typeof(T).IsAssignableFrom(type))
            {
                throw new ServiceBrokerException("Cannot cast type" + type.FullName + " to type " + typeof(T).FullName);
            }

            // the Conversion should be ok (guard clause asserts this)

            return (T) Activator.CreateInstance(type, args);

        }
        /// <summary>
        /// Resolves a Type From a qualified Type Name. Will throw an exception if the type is not resolvable.
        /// </summary>
        /// <param name="qualifiedTypeName"></param>
        /// <returns></returns>
        public static Type GetType(string qualifiedTypeName)
        {
            Ensure.ParameterIsNotNull(qualifiedTypeName, "qualifiedTypeName");

            if(!typeCache.ContainsKey(qualifiedTypeName))
            {
                Type myType = Type.GetType(qualifiedTypeName);

                if (myType == null)
                {
                    throw new ServiceBrokerException(string.Format("Unable to resolve type '{0}'. Check to ensure all required assemblies are deployed and that the type is qualified and spelled correctly.", qualifiedTypeName));
                }

                typeCache[qualifiedTypeName] = myType;
            }

            return typeCache[qualifiedTypeName];
        }
        /// <summary>
        /// Gets a Qualified Class Name in a standard manner and format
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetQualifiedClassName(Type type)
        {
            Ensure.ParameterIsNotNull(type, "type");

            return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
        }

        /// <summary>
        /// Gets the assembly part of a qualified type name
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetAssemblyComponent(string typeName)
        {
            Ensure.StringIsNotEmptyOrNull(typeName, "typeName");

            if(typeName.Contains(","))
            {
                return typeName.Substring(typeName.LastIndexOf(",") + 1).Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetTypeComponent(string typeName)
        {
            //System.String, mscorlib
            Ensure.StringIsNotEmptyOrNull(typeName, "typeName");

            if (typeName.Contains(","))
            {
                return typeName.Substring(0, typeName.IndexOf(",")).Trim();
            }
            else
            {
                return typeName;
            }
        }
    }
}