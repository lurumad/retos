using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectDumper
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            this Expression<Func<TSource, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("Expression refer to a method, not to a property.");
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new ArgumentException("Expression refer to a field, not to a property.");
            }

            return propertyInfo;
        }
    }
}