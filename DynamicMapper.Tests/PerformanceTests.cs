/*
 Copyright © 2012 Arjen Post (http://arjenpost.nl)
 License: http://www.apache.org/licenses/LICENSE-2.0 
 */

namespace DynamicMapper.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public partial class PerformanceTests
    {
        private static readonly PropertyInfo Int1Property = typeof(Entity).GetProperty("Int1");
        private static readonly PropertyInfo Int2Property = typeof(Entity).GetProperty("Int2");
        private static readonly PropertyInfo Int3Property = typeof(Entity).GetProperty("Int3");
        private static readonly PropertyInfo Int4Property = typeof(Entity).GetProperty("Int4");
        private static readonly PropertyInfo Int5Property = typeof(Entity).GetProperty("Int5");
        private static readonly PropertyInfo String1Property = typeof(Entity).GetProperty("String1");
        private static readonly PropertyInfo String2Property = typeof(Entity).GetProperty("String2");
        private static readonly PropertyInfo String3Property = typeof(Entity).GetProperty("String3");
        private static readonly PropertyInfo String4Property = typeof(Entity).GetProperty("String4");
        private static readonly PropertyInfo String5Property = typeof(Entity).GetProperty("String5");
        private static readonly PropertyInfo Double1Property = typeof(Entity).GetProperty("Double1");
        private static readonly PropertyInfo Double2Property = typeof(Entity).GetProperty("Double2");
        private static readonly PropertyInfo Double3Property = typeof(Entity).GetProperty("Double3");
        private static readonly PropertyInfo Double4Property = typeof(Entity).GetProperty("Double4");
        private static readonly PropertyInfo Double5Property = typeof(Entity).GetProperty("Double5");

        private readonly Random random = new Random();

        [TestMethod]
        public void PerformanceComparison()
        {
            var entityCount = 1000;
            var testCount = 10;

            var entities = Enumerable.Range(0, entityCount)
                .Select(index => CreateEntity())
                .ToList();

            var properties = new List<PropertyInfo>()
            {
                typeof(Entity).GetProperty("Int1"),
                typeof(Entity).GetProperty("String2"),
                typeof(Entity).GetProperty("String4"),
                typeof(Entity).GetProperty("Double1"),
                typeof(Entity).GetProperty("Double2")
            };

            // Warming up...
            NormalMap(entities[0], properties);

            Trace.WriteLine("--- Normal map method");

            for (int i = 0; i < testCount; i++)
            {
                var stopwatch = Stopwatch.StartNew();

                entities
                    .ForEach(entity =>
                    {
                        NormalMap(entity, properties);
                    });

                stopwatch.Stop();

                Trace.WriteLine(string.Format("{0} entities in {1} ms.", entityCount, stopwatch.ElapsedMilliseconds));
            }

            // Warming up...
            Mapper<Entity>.Map(entities[0], properties);

            Trace.WriteLine("--- DynamicMapper map method");

            for (int i = 0; i < testCount; i++)
            {
                var stopwatch = Stopwatch.StartNew();

                entities
                    .ForEach(entity =>
                    {
                        Mapper<Entity>.Map(entity, properties);
                    });

                stopwatch.Stop();

                Trace.WriteLine(string.Format("{0} entities in {1} ms.", entityCount, stopwatch.ElapsedMilliseconds));
            }
        }

        private dynamic NormalMap(Entity entity, List<PropertyInfo> properties)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            dynamic result = new ExpandoObject();

            if (properties.Contains(Int1Property))
            {
                result.Int1 = entity.Int1;
            }

            if (properties.Contains(Int2Property))
            {
                result.Int2 = entity.Int2;
            }

            if (properties.Contains(Int3Property))
            {
                result.Int3 = entity.Int3;
            }

            if (properties.Contains(Int4Property))
            {
                result.Int4 = entity.Int4;
            }

            if (properties.Contains(Int5Property))
            {
                result.Int5 = entity.Int5;
            }

            if (properties.Contains(String1Property))
            {
                result.String1 = entity.String1;
            }

            if (properties.Contains(String2Property))
            {
                result.String2 = entity.String2;
            }

            if (properties.Contains(String3Property))
            {
                result.String3 = entity.String3;
            }

            if (properties.Contains(String4Property))
            {
                result.String4 = entity.String4;
            }

            if (properties.Contains(String5Property))
            {
                result.String5 = entity.String5;
            }

            if (properties.Contains(Double1Property))
            {
                result.Double1 = entity.Double1;
            }

            if (properties.Contains(Double2Property))
            {
                result.Double2 = entity.Double2;
            }

            if (properties.Contains(Double3Property))
            {
                result.Double3 = entity.Double3;
            }

            if (properties.Contains(Double4Property))
            {
                result.Double4 = entity.Double4;
            }

            if (properties.Contains(Double5Property))
            {
                result.Double5 = entity.Double5;
            }

            return result;
        }

        private Entity CreateEntity()
        {
            var entity = new Entity()
            {
                Int1 = random.Next(),
                Int2 = random.Next(),
                Int3 = random.Next(),
                Int4 = random.Next(),
                Int5 = random.Next(),
                String1 = Guid.NewGuid().ToString(),
                String2 = Guid.NewGuid().ToString(),
                String3 = Guid.NewGuid().ToString(),
                String4 = Guid.NewGuid().ToString(),
                String5 = Guid.NewGuid().ToString(),
                Double1 = random.NextDouble(),
                Double2 = random.NextDouble(),
                Double3 = random.NextDouble(),
                Double4 = random.NextDouble(),
                Double5 = random.NextDouble()
            };

            return entity;
        }
    }
}
