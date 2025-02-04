using PnP.Core.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PnP.Core.QueryModel
{
    internal class DataModelQueryTranslator<TModel> : ExpressionVisitor
    {
        private readonly ODataQuery<TModel> query = new ODataQuery<TModel>();
        private readonly List<List<ODataFilter>> filtersStack = new List<List<ODataFilter>>();

        internal DataModelQueryTranslator()
        {
            filtersStack.Add(query.Filters);
        }

        internal ODataQuery<TModel> Translate(Expression expression)
        {
            Visit(expression);
            return query;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) ||
                m.Method.DeclaringType == typeof(QueryableExtensions))
            {
                switch (m.Method.Name)
                {
                    case "Select":
                        VisitSelect(m);
                        return m;
                    case nameof(QueryableExtensions.QueryProperties):
                        VisitQueryProperties(m);
                        return m;
                    case "OrderBy":
                    case "ThenBy":
                        VisitOrderBy(m);
                        return m;
                    case "OrderByDescending":
                    case "ThenByDescending":
                        VisitOrderBy(m, ascending: false);
                        return m;
                    case "Where":
                        VisitWhere(m);
                        return m;
                    case "First":
                    case "FirstOrDefault":
                        VisitFirstOrDefault(m);
                        return m;
                    case "Take":
                        VisitTake(m);
                        return m;
                    case "Skip":
                        VisitSkip(m);
                        return m;
                }
            }
            throw new NotSupportedException(string.Format(PnPCoreResources.Exception_Unsupported_Method, m.Method.Name));
        }

        private void VisitSelect(MethodCallExpression m)
        {
            Visit(m.Arguments[0]);
            var propertySelector = m.Arguments[1] as UnaryExpression;
            if (propertySelector != null)
            {
                var lambda = propertySelector.Operand as LambdaExpression;
                if (lambda != null)
                {
                    var parameterExpression = lambda.Body as ParameterExpression;
                    // If the Select statement 
                    if (parameterExpression == null || // is trying to project a new anonymous type
                        lambda.Parameters[0].Name != parameterExpression.Name) // or is not projecting the whole input
                    {
                        throw new NotSupportedException(PnPCoreResources.Exception_Unsupported_Projection);
                    }
                }
            }
        }

        private void VisitQueryProperties(MethodCallExpression m)
        {
            Visit(m.Arguments[0]);
            if (m.Arguments[1] is UnaryExpression propertySelector)
            {
                if (propertySelector.Operand is Expression<Func<TModel, object>> lambda)
                {
                    query.Fields.Add(lambda);
                }
                else if (propertySelector.Operand is Expression<Func<TModel, object>>[] lambdas)
                {
                    query.Fields.AddRange(lambdas);
                }
            }
            else if (m.Arguments[1] is NewArrayExpression arrayExpression)
            {
                foreach (var expression in arrayExpression.Expressions)
                {
                    if (expression is UnaryExpression expressionPropertySelector)
                    {
                        if (expressionPropertySelector.Operand is Expression<Func<TModel, object>> lambda)
                        {
                            query.Fields.Add(lambda);
                        }
                        else if (expressionPropertySelector.Operand is Expression<Func<TModel, object>>[] lambdas)
                        {
                            query.Fields.AddRange(lambdas);
                        }
                    }
                }
            }

        }

        private void VisitOrderBy(MethodCallExpression m, bool ascending = true)
        {
            Visit(m.Arguments[0]);
            var propertySelector = m.Arguments[1] as UnaryExpression;
            if (propertySelector != null)
            {
                var lambda = propertySelector.Operand as LambdaExpression;
                if (lambda != null)
                {
                    switch (lambda.Body)
                    {
                        case UnaryExpression unary:
                            var unaryMember = unary.Operand as MemberExpression;
                            if (unaryMember != null)
                            {
                                query.OrderBy.Add(new OrderByItem
                                {
                                    Field = unaryMember.Member.Name,
                                    Direction = ascending ? OrderByDirection.Asc : OrderByDirection.Desc
                                });
                            }
                            break;
                        case MemberExpression member:
                            query.OrderBy.Add(new OrderByItem
                            {
                                Field = member.Member.Name,
                                Direction = ascending ? OrderByDirection.Asc : OrderByDirection.Desc
                            });
                            break;
                    }
                }
            }
        }

        private void VisitWhere(MethodCallExpression m)
        {
            Visit(m.Arguments[0]);
            LambdaExpression lambda = (LambdaExpression)m.Arguments[1].StripQuotes();

            switch (lambda.Body)
            {
                case BinaryExpression binary:
                    AddFilter(binary);
                    break;

                case MethodCallExpression methodCall:
                    if (methodCall.Type != typeof(bool))
                    {
                        throw new NotSupportedException(string.Format(PnPCoreResources.Exception_Unsupported_ExpressionMustReturnBoolean, methodCall));
                    }

                    string methodField = GetFilterField(methodCall);
                    // Should never happen
                    if (methodField == null)
                    {
                        throw new NotSupportedException(string.Format(PnPCoreResources.Exception_Unsupported_Expression, methodCall));
                    }

                    AddFilterToStack(new FilterItem
                    {
                        Field = methodField,
                        Criteria = FilteringCriteria.Equal,
                        Value = true
                    });

                    break;
            }
        }

        private void VisitFirstOrDefault(MethodCallExpression m)
        {
            Visit(m.Arguments[0]);

            // If the FirstOrDefault method includes a filtering expression
            if (m.Arguments.Count > 1)
            {
                LambdaExpression lambda = (LambdaExpression)m.Arguments[1].StripQuotes();

                switch (lambda.Body)
                {
                    case BinaryExpression binary:
                        AddFilter(binary);
                        break;

                    case MethodCallExpression methodCall:
                        if (methodCall.Type != typeof(bool))
                        {
                            throw new NotSupportedException(string.Format(PnPCoreResources.Exception_Unsupported_ExpressionMustReturnBoolean, methodCall));
                        }

                        string methodField = GetFilterField(methodCall);
                        // Should never happen
                        if (methodField == null)
                        {
                            throw new NotSupportedException(string.Format(PnPCoreResources.Exception_Unsupported_Expression, methodCall));
                        }

                        AddFilterToStack(new FilterItem
                        {
                            Field = methodField,
                            Criteria = FilteringCriteria.Equal,
                            Value = true
                        });

                        break;
                }
            }

            // FirstOrDefault corresponds to $take=1
            query.Top = 1;
        }

        private void VisitTake(MethodCallExpression m)
        {
            Visit(m.Arguments[0]);
            query.Top = GetConstantValue<int>(m.Arguments[1]);
        }

        private void VisitSkip(MethodCallExpression m)
        {
            Visit(m.Arguments[0]);
            query.Skip = GetConstantValue<int>(m.Arguments[1]);
        }

        private void AddFilter(BinaryExpression expression)
        {
            // Create a new group
            var filtersGroup = new FiltersGroup();
            // Now filters must be added into the current group
            AddFilterToStack(filtersGroup);

            string filterField = GetFilterField(expression.Left);
            object filterValue = GetFilterValue(expression.Right);

            RemoveFilterStack();

            // If field and value are not null it means that they are primitive values
            if (filterField != null && filterValue != null)
            {
                AddFilterToStack(new FilterItem
                {
                    Field = filterField,
                    Criteria =
                        (FilteringCriteria)Enum.Parse(typeof(FilteringCriteria), expression.NodeType.ToString()),
                    Value = filterValue
                });
            }
            else
            {
                // A new FiltersGroup has been added
                FilteringConcatOperator concat;
                switch (expression.NodeType)
                {
                    case ExpressionType.AndAlso:
                    case ExpressionType.And:
                        concat = FilteringConcatOperator.AND;
                        break;
                    case ExpressionType.OrElse:
                    case ExpressionType.Or:
                        concat = FilteringConcatOperator.OR;
                        break;
                    default:
                        throw new NotSupportedException(string.Format(PnPCoreResources.Exception_Unsupported_NodeType, expression.NodeType, expression));
                }

                // Set the contact operators to the filters (should be two)
                foreach (ODataFilter filter in filtersGroup.Filters)
                {
                    filter.ConcatOperator = concat;
                }
            }
        }

        private string GetFilterField(Expression expression)
        {
            string memberName;
            string functionCall;

            switch (expression)
            {
                case BinaryExpression binary:
                    AddFilter(binary);
                    return null;
                case MemberExpression member:

                    // Get the target member name, if any
                    memberName = GetMemberName(member.Expression.StripQuotes(), false);

                    // Member is null, probably expression is a normal property access
                    if (memberName == null)
                    {
                        break;
                    }

                    // Try to get the OData function
                    if (FunctionMapping.TryMapMember(member.Member, memberName, Array.Empty<object>(), out functionCall))
                    {
                        return functionCall;
                    }

                    // Raise an error
                    var validMembers = FunctionMapping.SupportedMembers.Where(m => !(m is MethodInfo)).Select(m => $"{m.DeclaringType.Name}.{m.Name}");
                    var validMembersString = string.Join(", ", validMembers);
                    throw new NotSupportedException(
                        string.Format(PnPCoreResources.Exception_Unsupported_ExpressionOnlyMembers,
                        expression, validMembersString));

                case MethodCallExpression methodCall:
                    // Get the target member name, if any
                    memberName = GetMemberName(methodCall.Object.StripQuotes());

                    // Member is null, probably expression is a normal property access
                    if (memberName == null)
                    {
                        break;
                    }

                    // Convert all method call arguments to objects
                    object[] arguments = methodCall.Arguments.Select(p => p.GetConstantValue()).ToArray();

                    // Try to get the OData function
                    if (FunctionMapping.TryMapMember(methodCall.Method, memberName, arguments, out functionCall))
                    {
                        return functionCall;
                    }

                    // Raise an error
                    var validMethods = FunctionMapping.SupportedMembers.OfType<MethodInfo>().Select(m => $"{m.DeclaringType.Name}.{m.Name}");
                    var validMethodsString = string.Join(", ", validMethods);
                    throw new NotSupportedException(string.Format(PnPCoreResources.Exception_Unsupported_ExpressionOnlyMethods, expression, validMethodsString));
            }

            // Try with default resolver
            return GetMemberName(expression.StripQuotes());
        }

        private void AddFilterToStack(ODataFilter filter)
        {
            // Add to the last filters group
            filtersStack.Last().Add(filter);

            if (filter is FiltersGroup fg)
            {
                // Add a new list to the end of the stack
                filtersStack.Add(fg.Filters);
            }
        }

        private void RemoveFilterStack()
        {
            if (filtersStack.Count > 1)
            {
                List<ODataFilter> filters = filtersStack[filtersStack.Count - 1];

                // No filters has been added thus is useless
                if (filters.Count == 0)
                {
                    // We can remove this filters groups from parent
                    filtersStack[filtersStack.Count - 2].RemoveAll(f => f is FiltersGroup fg && fg.Filters == filters);
                }
                // Remove latest filters group
                filtersStack.RemoveAt(filtersStack.Count - 1);
            }
        }

        private static string GetMemberName(Expression expression, bool raiseError = true)
        {
            switch (expression)
            {
                case MemberExpression member:
                    return member.Member.Name;
                case MethodCallExpression methodCall:
                    // Support for Fields member
                    if (methodCall.Method.DeclaringType == typeof(TransientDictionary))
                    {
                        return GetConstantValue<string>(methodCall.Arguments[0]);
                    }

                    if (raiseError)
                    {
                        throw new NotSupportedException(string.Format(PnPCoreResources.Exception_Unsupported_ExpressionOnlyIndexer, expression, typeof(Dictionary<string, object>)));
                    }

                    break;
            }
            if (raiseError)
            {
                throw new NotSupportedException(string.Format(PnPCoreResources.Exception_Unsupported_ExpressionOnlyTypes, expression, typeof(MemberExpression), typeof(MethodCallExpression)));
            }

            return null;
        }

        private object GetFilterValue(Expression expression)
        {
            expression = expression.StripQuotes();
            if (expression is BinaryExpression binary)
            {
                AddFilter(binary);
                return null;
            }

            return expression.GetConstantValue();
        }

        private static T GetConstantValue<T>(Expression expression)
        {
            object value = expression.GetConstantValue();
            try
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (FormatException fe)
            {
                throw new NotSupportedException(string.Format(PnPCoreResources.Exception_Unsupported_ExpressionConstantNotValid, expression, typeof(T)), fe);
            }
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            throw new NotSupportedException(
                string.Format(PnPCoreResources.Exception_Unsupported_UnaryOperator,
                    u.NodeType));
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

    }
}
