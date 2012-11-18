/*
 Copyright © 2012 Arjen Post (http://arjenpost.nl)
 License: http://www.apache.org/licenses/LICENSE-2.0 
 */

namespace DynamicMapper
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Provides a mapping method for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to map.</typeparam>
    public static class Mapper<T>
    {
        static Mapper()
        {
            var dictionaryAddMethod = typeof(IDictionary<string, object>).GetMethod("Add", new[] { typeof(string), typeof(object) });
            var collectionContainsMethod = typeof(ICollection<PropertyInfo>).GetMethod("Contains", new[] { typeof(PropertyInfo) });

            var sourceExpression = Expression.Parameter(typeof(T), "source");
            var propertiesExpression = Expression.Parameter(typeof(ICollection<PropertyInfo>), "properties");
            var destinationExpression = Expression.Variable(typeof(IDictionary<string, object>));
            var expressions = new List<Expression>();

            expressions.Add(
                Expression.IfThen(
                    Expression.Equal(sourceExpression, Expression.Constant(null)),
                    Expression.Throw(Expression.New(typeof(ArgumentNullException).GetConstructor(new[] { typeof(string) }), Expression.Constant("source")))));

            expressions.Add(
                Expression.IfThen(
                    Expression.Equal(propertiesExpression, Expression.Constant(null)),
                    Expression.Throw(Expression.New(typeof(ArgumentNullException).GetConstructor(new[] { typeof(string) }), Expression.Constant("properties")))));

            expressions.Add(Expression.Assign(destinationExpression, Expression.New(typeof(ExpandoObject))));

            typeof(T)
                .GetProperties()
                .Where(property => property.CanRead && !property.GetIndexParameters().Any())
                .ToList()
                .ForEach(property =>
                {
                    expressions.Add(
                        Expression.IfThen(
                            Expression.Call(propertiesExpression, collectionContainsMethod, Expression.Constant(property, typeof(PropertyInfo))),
                            Expression.Call(destinationExpression, dictionaryAddMethod,
                                Expression.Constant(property.Name, typeof(string)),
                                Expression.Convert(Expression.Property(sourceExpression, property), typeof(object)))));
                });
            
            expressions.Add(Expression.Label(Expression.Label()));
            expressions.Add(destinationExpression);

            var blockExpression = Expression.Block(new[] { destinationExpression }, expressions);

            Mapper<T>.Map = Expression.Lambda<Func<T, ICollection<PropertyInfo>, dynamic>>(blockExpression, sourceExpression, propertiesExpression).Compile();
        }

        /// <summary>
        /// Gets the mapping method associated with <typeparam name="T"/>.
        /// </summary>
        /// <remarks>
        /// The resulting method takes a <typeparamref name="T"/> and an <see cref="ICollection{PropertyInfo}"/> 
        /// and returns an ExpandoObject containing the values from <typeparamref name="T"/> using the 
        /// properties specified in the <see cref="ICollection{PropertyInfo}"/>.
        /// </remarks>
        public static Func<T, ICollection<PropertyInfo>, dynamic> Map { get; private set; }
    }
}