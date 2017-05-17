using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Querying.Parsers.Irony
{
    public sealed class IronySortSpecificationParser : ISortSpecificationParser
    {
        private static readonly LanguageData language = new LanguageData(new SortSpecificationGrammar());
        private static readonly Parser parser = new Parser(language);

        public SortSpecification<TKey, TAggregateRoot> Parse<TKey, TAggregateRoot>(string input)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>
        {
            var syntaxTree = parser.Parse(input);
            if (syntaxTree.HasErrors())
            {
                var parseErrors = new List<string>();
                syntaxTree.ParserMessages.ToList().ForEach(m => parseErrors.Add(m.ToString()));
                throw new ParsingException($"Input text '{input}' parsed failed. See ParseErrors property for more details.", parseErrors);
            }

            var sortSpecification = new SortSpecification<TKey, TAggregateRoot>();
            ParseNode(syntaxTree.Root, ref sortSpecification);
            return sortSpecification;
        }

        private void ParseNode<TKey, TAggregateRoot>(ParseTreeNode node, ref SortSpecification<TKey, TAggregateRoot> sortSpecification)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>
        {
            switch(node.Term.Name)
            {
                case "sort-specification":
                    var suggestedPropertyName = node.ChildNodes[0].ChildNodes[0].Token.ValueString;
                    var suggestedSortingOrder = node.ChildNodes[1].ChildNodes[0].Token.ValueString;
                    var propertyName = ParsingUtils.InferProperty<TAggregateRoot>(suggestedPropertyName)?.Name;
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        throw new ParsingException("Expression parsed failed.", new[] { $"The property that has the name similar to {suggestedPropertyName} does not exist." });
                    }

                    var sortOrder = suggestedSortingOrder.ToUpper() == "A" ? SortOrder.Ascending : SortOrder.Descending;
                    sortSpecification.Add(propertyName, sortOrder);
                    break;
                case "concatenated-sort-specification":
                    var leftNode = node.ChildNodes[0];
                    var rightNode = node.ChildNodes[2];
                    ParseNode(leftNode, ref sortSpecification);
                    ParseNode(rightNode, ref sortSpecification);
                    break;
            }
        }
    }
}
