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
    /// <typeparam name="T">The type to map.</typeparam>
    public static class Mapper<T>
    {
        private static readonly MethodInfo DictionaryAddMethod = typeof(IDictionary<string, object>).GetMethod("Add", new[] { typeof(string), typeof(object) });
        
        private static readonly MethodInfo CollectionContainsMethod = typeof(ICollection<PropertyInfo>).GetMethod("Contains", new[] { typeof(PropertyInfo) });

        private static readonly Action<T, ExpandoObject, PropertyInfo[]> MapImpl;

        static Mapper()
        {
            var sourceExpression = Expression.Parameter(typeof(T), "source");
            var targetExpression = Expression.Parameter(typeof(ExpandoObject), "target");
            var propertiesExpression = Expression.Parameter(typeof(PropertyInfo[]), "properties");

            var expressions = new List<Expression>();

            typeof(T)
                .GetProperties()
                .Where(property => property.CanRead && !property.GetIndexParameters().Any())
                .ToList()
                .ForEach(property =>
                {
                    expressions.Add(
                        // if (properties.Contains({property}))
                        Expression.IfThen(Expression.Call(propertiesExpression, CollectionContainsMethod, Expression.Constant(property, typeof(PropertyInfo))),
                            // target.Add({property.Name}, (object)source.{property.Name});
                            Expression.Call(targetExpression, DictionaryAddMethod,
                                Expression.Constant(property.Name, typeof(string)),
                                Expression.Convert(Expression.Property(sourceExpression, property), typeof(object)))));
                });

            Mapper<T>.MapImpl = Expression.Lambda<Action<T, ExpandoObject, PropertyInfo[]>>(Expression.Block(expressions), sourceExpression, targetExpression, propertiesExpression).Compile();
        }

        /// <summary>
        /// Maps the <paramref name="properties"/> from <paramref name="source"/> to a dynamic object.
        /// </summary>
        /// <param name="source">The source object to map from.</param>
        /// <param name="properties">A <see cref="PropertyInfo[]"/> with properties to map.</param>
        /// <returns>The dynamic object containing the mapped properties.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="properties"/> is null.</exception>
        public static dynamic Map(T source, params PropertyInfo[] properties)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            dynamic target = new ExpandoObject();

            MapImpl(source, target, properties);

            return target;
        }
    }
}