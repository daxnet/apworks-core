using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Querying.Parsers.Irony
{
    [Language("Apworks Sort Specification Language", "1.0", "The language for describing the sorting in an Apworks query.")]
    internal sealed class SortSpecificationGrammar : Grammar
    {
        public SortSpecificationGrammar()
            : base(false)
        {
            var identifier = new IdentifierTerminal("identifier");
            // Defines the property reference on the given target object.
            var propertyReference = new NonTerminal("property");
            propertyReference.Rule = identifier;

            // Defines the sorting order
            var sortingOrder = ToTerm("A") | "D";
            sortingOrder.Name = "sorting-order";

            // Defines the concatenate
            var concatenate = ToTerm("AND");
            concatenate.Name = "concatenate";

            // Defines a single sort specification
            var sortSpecification = new NonTerminal("sort-specification");
            sortSpecification.Rule = propertyReference + sortingOrder;

            // Defines the concatenated sort specification
            var specification = new NonTerminal("specification");
            var concatenatedSortSpecification = new NonTerminal("concatenated-sort-specification");
            specification.Rule = sortSpecification | concatenatedSortSpecification;
            concatenatedSortSpecification.Rule = specification + concatenate + specification;

            RegisterOperators(10, concatenate);
            MarkTransient(specification);

            this.Root = specification;
        }
    }
}
