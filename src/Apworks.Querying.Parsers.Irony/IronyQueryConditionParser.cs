using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Apworks.Querying.Parsers.Irony
{
    public sealed class IronyQueryConditionParser : IQueryConditionParser
    {
        private static readonly LanguageData language = new LanguageData(new QueryConditionGrammar());
        private static readonly Parser parser = new Parser(language);

        public Expression<Func<T, bool>> Parse<T>(string input)
        {
            var syntaxTree = parser.Parse(input);
            if (syntaxTree.HasErrors())
            {
                var parseErrors = new List<string>();
                syntaxTree.ParserMessages.ToList().ForEach(m => parseErrors.Add(m.ToString()));
                throw new ParsingException($"Input text '{input}' parsed failed. See ParseErrors property for more details.", parseErrors);
            }

            var parameterExpression = Expression.Parameter(typeof(T), "p");
            var parsedExpression = ParseExpression<T>(syntaxTree.Root, parameterExpression);

            return Expression.Lambda<Func<T, bool>>(parsedExpression, parameterExpression);
        }

        private Expression ParseExpression<T>(ParseTreeNode node, ParameterExpression parameterExpression)
        {
            switch (node.Term.Name)
            {
                case "logical-operation":
                    var leftLogicalOperandNode = node.ChildNodes[0];
                    var logicalOperatorNode = node.ChildNodes[1];
                    var rightLogicalOprandNode = node.ChildNodes[2];
                    switch (logicalOperatorNode.ChildNodes[0].Term.Name)
                    {
                        case "AND":
                            return Expression.AndAlso(ParseExpression<T>(leftLogicalOperandNode, parameterExpression), ParseExpression<T>(rightLogicalOprandNode, parameterExpression));
                        case "OR":
                            return Expression.OrElse(ParseExpression<T>(leftLogicalOperandNode, parameterExpression), ParseExpression<T>(rightLogicalOprandNode, parameterExpression));
                    }
                    break;

                case "relational-operation":
                    var leftRelationalOperandNode = node.ChildNodes[0];
                    var relationalOperatorNode = node.ChildNodes[1];
                    var rightRelationalOperandNode = node.ChildNodes[2];

                    var leftExpression = ParseExpression<T>(leftRelationalOperandNode, parameterExpression);
                    var rightExpression = ParseExpression<T>(rightRelationalOperandNode, parameterExpression);

                    if (leftExpression.NodeType == ExpressionType.Constant || rightExpression.NodeType == ExpressionType.Constant)
                    {
                        FixExpressionType(ref leftExpression, ref rightExpression);
                    }

                    switch (relationalOperatorNode.ChildNodes[0].Term.Name)
                    {
                        case "EQ":
                            return Expression.Equal(leftExpression, rightExpression);
                        case "NE":
                            return Expression.NotEqual(leftExpression, rightExpression);
                        case "GE":
                            return Expression.GreaterThanOrEqual(leftExpression, rightExpression);
                        case "GT":
                            return Expression.GreaterThan(leftExpression, rightExpression);
                        case "LE":
                            return Expression.LessThanOrEqual(leftExpression, rightExpression);
                        case "LT":
                            return Expression.LessThan(leftExpression, rightExpression);
                    }
                    break;
                case "not-operation":
                    return Expression.Not(ParseExpression<T>(node.ChildNodes[1], parameterExpression));

                case "string-operation":
                    var leftStringFunctionOperandNode = node.ChildNodes[0];
                    var stringFunctionNode = node.ChildNodes[1];
                    var rightStringFunctionOperandNode = node.ChildNodes[2];

                    var leftStringFunctionExpression = ParseExpression<T>(leftStringFunctionOperandNode, parameterExpression);
                    var rightStringFunctionExpression = ParseExpression<T>(rightStringFunctionOperandNode, parameterExpression);
                    switch (stringFunctionNode.ChildNodes[0].Term.Name)
                    {
                        case "SW":
                            return Expression.Call(leftStringFunctionExpression, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), rightStringFunctionExpression);
                        case "EW":
                            return Expression.Call(leftStringFunctionExpression, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), rightStringFunctionExpression);
                        case "CT":
                            return Expression.Call(leftStringFunctionExpression, typeof(string).GetMethod("Contains"), rightStringFunctionExpression);
                    }
                    break;
                case "property":
                    var propertyName = InferPropertyName<T>(node.ChildNodes[0].Token.ValueString);
                    if (!string.IsNullOrEmpty(propertyName))
                    {
                        return Expression.Property(parameterExpression, node.ChildNodes[0].Token.ValueString);
                    }
                    throw new ParsingException("Expression parsed failed.", new[] { $"The property that has the name similar to {node.ChildNodes[0].Token.ValueString} does not exist." });

                case "string":
                    return Expression.Constant(node.Token.Value);

                case "number":
                    var expr = Expression.Constant(node.Token.Value);
                    return expr;
            }
            return null;
        }

        private static string InferPropertyName<T>(string reference) => 
            typeof(T).GetTypeInfo().GetProperties().Where(p => p.Name.ToLower().Equals(reference.ToLower())).FirstOrDefault()?.Name;

        private static void FixExpressionType(ref Expression left, ref Expression right)
        {
            var leftTypeCode = Type.GetTypeCode(left.Type);
            var rightTypeCode = Type.GetTypeCode(right.Type);

            if (leftTypeCode == rightTypeCode)
                return;

            if (leftTypeCode > rightTypeCode)
                right = Expression.Convert(right, left.Type);
            else
                left = Expression.Convert(left, right.Type);
        }
    }
}
