using Irony.Parsing;
using System;
using System.Linq.Expressions;

namespace Apworks.Querying.Parsers.Irony
{
    public sealed class IronyQueryConditionParser : IQueryConditionParser
    {
        private static readonly LanguageData language = new LanguageData(new QueryConditionGrammar());
        private static readonly Parser parser = new Parser(language);

        public Expression<Func<T, bool>> Parse<T>(string input)
        {
            var syntaxTree = parser.Parse(input);
            throw new NotImplementedException();
        }
    }
}
