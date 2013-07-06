using System;
using System.Diagnostics;

namespace ServiceBroker.Common
{
    /// <summary>
    /// Static class used for providing common code assertions
    /// </summary>
    public static class Ensure
    {
        /// <summary>
        /// Asserts that the parameter provided is not null
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName"></param>
        [DebuggerStepThrough]
        public static void ParameterIsNotNull(object parameter, string parameterName)
        {

            if (parameter == null)
            {
                throw new ArgumentNullException(
                    (parameterName ?? "Unknown"), 
                    string.Format("Parameter {0} cannot be null.", 
                    (parameterName ?? "Unknown")));
            }
        }

        /// <summary>
        /// Ensure 'that object is not null' :) Throws an exception containing the specified error message
        /// if it is
        /// </summary>
        /// <param name="objectToCheck"></param>
        /// <param name="messageIfNull"></param>
        [DebuggerStepThrough]
        public static void ThatObjectIsNotNull(object objectToCheck, string messageIfNull)
        {
            if(objectToCheck == null)
            {
                throw new NullReferenceException(messageIfNull);
            }
        }

        /// <summary>
        /// Ensures that 'the string is not null or empty'
        /// </summary>
        /// <param name="stringToCheck"></param>
        /// <param name="messageIfNullOrEmpty"></param>
        [DebuggerStepThrough]
        public static void TheStringIsNotNullOrEmpty(string stringToCheck, string messageIfNullOrEmpty)
        {
            if (string.IsNullOrEmpty(stringToCheck) || stringToCheck.Trim().Length == 0)
            {
                throw new ApplicationException(messageIfNullOrEmpty);
            }
        }

        /// <summary>
        /// Asserts that a parameter is of an expected type
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="expectedType"></param>
        /// <param name="parameterName"></param>
        [DebuggerStepThrough]
        public static void ObjectIsOfType(object parameter, Type expectedType, string parameterName)
        {
            ParameterIsNotNull(parameter, "parameter");
            ParameterIsNotNull(expectedType, "expectedType");
            ParameterIsNotNull(parameterName, "parameterName");

            if (parameter.GetType() != expectedType)
            {
                throw new ArgumentException(string.Format("Parameter '{0}' was of the wrong type. Expected {1} but was {2}", parameterName, expectedType.FullName, parameter.GetType().FullName));
            }
        }

        /// <summary>
        /// Ensures that a string parameter is not null and is not empty (ie Length != 0)
        /// </summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <remarks>Does not trim strings. ie " " will be evaluated as NOT empty (length == 1).</remarks>
        [DebuggerStepThrough]
        public static void StringIsNotEmptyOrNull(string stringToCheck, string parameterName)
        {
            ParameterIsNotNull(stringToCheck, parameterName);
            
            if(stringToCheck.Length == 0)
            {
                throw new ArgumentException("String cannot be empty.", parameterName);
            }
        }

        /// <summary>
        /// Ensures that any given date is in the future
        /// </summary>
        /// <param name="dateToCheck"></param>
        /// <param name="parameterName"></param>
        [DebuggerStepThrough]
        public static void DateIsInTheFuture(DateTime dateToCheck, string parameterName)
        {
            if(dateToCheck < DateTime.Now)
            {
                throw new ApplicationException(string.Format("Date {0} is not in the future. Parameter={1}", 
                    dateToCheck.ToString("dd/MM/yyyy hh:mm:ss:fff"), parameterName));
            }
        }

        /// <summary>
        /// Ensures that the candidateType implements IComparable
        /// </summary>
        /// <param name="candidateType"></param>
        [DebuggerStepThrough]
        public static void TypeIsComparable(Type candidateType)
        {
            ParameterIsNotNull(candidateType, "candidateType");

            if(!typeof(IComparable).IsAssignableFrom(candidateType))
            {
                throw new ApplicationException(String.Format("Type '{0}' does not implement IComparable.", candidateType.FullName));
            }
        }

        /// <summary>
        /// Ensures a boolean condition is met, and if not throws
        /// an assertation exception
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        [DebuggerStepThrough]
        public static void ConditionIsMet(bool condition, string message)
        {

            if (!condition)
            {
                throw new ApplicationException(
                    String.Format("Boolean Ensure condition not met. Detail = '{0}'.", message));
            }

        }

        /// <summary>
        /// Ensures a boolean condition is met (via a predicate), and if not throws
        /// an assertation exception
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="target"></param>
        /// <param name="message"></param>
        [DebuggerStepThrough]
        public static void ConditionIsMet<T>(Predicate<T> predicate, T target, string message)
        {
            ParameterIsNotNull(predicate, "predicate");

            if (!predicate(target))
            {
                throw new ApplicationException(
                    String.Format("Boolean Ensure condition not met. Detail = '{0}'.", message));
            }

        }
    }
}