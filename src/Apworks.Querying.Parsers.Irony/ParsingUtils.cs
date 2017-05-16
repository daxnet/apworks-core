using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Querying.Parsers.Irony
{
    /// <summary>
    /// Represents the utilities for parsing.
    /// </summary>
    internal static class ParsingUtils
    {
        /// <summary>
        /// Infers the name of the property from a given suggestion.
        /// </summary>
        /// <typeparam name="T">The type of the object from which the property name is inferring.</typeparam>
        /// <param name="suggestion">The suggestion.</param>
        /// <returns>The property name.</returns>
        public static string InferPropertyName<T>(string suggestion) =>
            typeof(T).GetTypeInfo().GetProperties().Where(p => p.Name.ToLower().Equals(suggestion.ToLower())).FirstOrDefault()?.Name;
    }
}
