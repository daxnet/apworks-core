using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    /// <summary>
    /// Represents the data structure that holds both controller/action names and the types of method parameters
    /// that can uniquely identify a controller action.
    /// </summary>
    /// <seealso cref="System.IEquatable{Apworks.Integration.AspNetCore.Hal.ControllerActionSignature}" />
    public sealed class ControllerActionSignature : IEquatable<ControllerActionSignature>
    {
        /// <summary>
        /// Represents the <see cref="ControllerActionSignature"/> that represents any controller and action.
        /// </summary>
        public static readonly ControllerActionSignature Current = "*.*";

        public ControllerActionSignature(string actionName)
            : this(Current.ControllerName, actionName, Type.EmptyTypes)
        { }

        public ControllerActionSignature(string actionName, IEnumerable<Type> parameterTypes)
            : this(Current.ControllerName, actionName, parameterTypes)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerActionSignature"/> class.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        public ControllerActionSignature(string controllerName, string actionName)
            : this(controllerName, actionName, Type.EmptyTypes)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerActionSignature"/> class.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        public ControllerActionSignature(string controllerName, string actionName, IEnumerable<Type> parameterTypes)
        {
            this.ControllerName = controllerName;
            this.ActionName = actionName;
            this.ParameterTypes = new List<Type>(parameterTypes);
        }

        public string ControllerName { get; }

        public string ActionName { get; }

        public IEnumerable<Type> ParameterTypes { get; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ControllerActionSignature other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var equals = !string.IsNullOrEmpty(this.ControllerName) &&
                !string.IsNullOrEmpty(this.ActionName) &&
                (this.ControllerName.Equals(Current.ControllerName) || this.ControllerName.Equals(other.ControllerName, StringComparison.CurrentCultureIgnoreCase)) &&
                this.ActionName.Equals(other.ActionName, StringComparison.CurrentCultureIgnoreCase) &&
                CompareTypes(this.ParameterTypes, other.ParameterTypes);

            return equals;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var result = this.ControllerName?.GetHashCode() ^
                this.ActionName?.GetHashCode() ^
                this.ParameterTypes?.GetHashCode();
            return result != null && result.HasValue ? result.Value : base.GetHashCode();
        }

        private bool CompareTypes(IEnumerable<Type> origin, IEnumerable<Type> destination)
        {
            var originList = origin.ToList();
            var destinationList = destination.ToList();
            if (originList.Count != destinationList.Count)
            {
                return false;
            }

            var result = true;
            for (var i = 0; i < originList.Count; i++)
            {
                if (originList[i].Equals(typeof(Nil)) || destinationList[i].Equals(typeof(Nil)))
                {
                    continue;
                }

                result = originList[i].Equals(destinationList[i]);
                if (!result)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator == (ControllerActionSignature a, ControllerActionSignature b)
        {
            if ((object)a == null)
            {
                return (object)b == null;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator != (ControllerActionSignature a, ControllerActionSignature b)
        {
            return !(a == b);
        }

        public static implicit operator ControllerActionSignature (string src)
        {
            const string pattern = @"(?<ctrl>\w+|\*)\.(?<action>\w+|\*)(?<parms>\(.+(\s*\,\s*.+)*\))?";
            var regex = new Regex(pattern);
            var match = regex.Match(src);
            if (!match.Success)
            {
                throw new InvalidCastException("Cannot cast the given string into a ControllerActionSignature object.");
            }

            var controllerName = match.Groups["ctrl"].Value;
            var actionName = match.Groups["action"].Value;
            var parametersName = match.Groups["parms"].Value;
            if (!string.IsNullOrEmpty(parametersName))
            {
                var parameterTokens = parametersName.Trim('(', ')').Split(',').Select(x => x.Trim());
                var parameterTypes = InferParameterTypes(parameterTokens);
                return new ControllerActionSignature(controllerName, actionName, parameterTypes);
            }

            return new ControllerActionSignature(controllerName, actionName);
        }

        private static IEnumerable<Type> InferParameterTypes(IEnumerable<string> parameterTokens)
        {
            foreach (var token in parameterTokens) yield return InferParameterType(token);
        }

        private static Type InferParameterType(string token)
        {
            switch (token.ToLowerInvariant())
            {
                case "int":
                case "int32":
                    return typeof(int);
                case "int16":
                    return typeof(short);
                case "int64":
                    return typeof(long);
                case "double":
                    return typeof(double);
                case "float":
                    return typeof(float);
                case "decimal":
                    return typeof(decimal);
                case "datetime":
                    return typeof(DateTime);
                case "bool":
                case "boolean":
                    return typeof(bool);
                case "string":
                    return typeof(string);
                case "int?":
                case "int32?":
                    return typeof(int?);
                case "int16?":
                    return typeof(short?);
                case "int64?":
                    return typeof(long?);
                case "double?":
                    return typeof(double?);
                case "float?":
                    return typeof(float?);
                case "decimal?":
                    return typeof(decimal?);
                case "datetime?":
                    return typeof(DateTime?);
                case "bool?":
                case "boolean?":
                    return typeof(bool?);
                case "*":
                    return typeof(Nil);
                default:
                    return Type.GetType(token);
            }
        }

        /// <summary>
        /// Represents an empty type that is used for checking a wildcard match
        /// on the parameter types.
        /// </summary>
        private class Nil { }
    }
}