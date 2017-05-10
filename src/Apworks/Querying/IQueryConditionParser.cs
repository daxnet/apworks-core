using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Querying
{
    /// <summary>
    /// Represents that the implemented classes are query condition parsers
    /// which will parse the input text into a Lambda expression that can
    /// be used as the condition of a query.
    /// </summary>
    public interface IQueryConditionParser
    {
        /// <summary>
        /// Parses the specified input into the Lambda expression.
        /// </summary>
        /// <typeparam name="T">The type of the parameter object in the parsed Lambda expression.</typeparam>
        /// <param name="input">The input string that will be parsed.</param>
        /// <returns>The lambda expression.</returns>
        Expression<Func<T, bool>> Parse<T>(string input);
    }
}
