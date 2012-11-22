/*
 Copyright © 2012 Arjen Post (http://arjenpost.nl)
 License: http://www.apache.org/licenses/LICENSE-2.0 
 */

namespace DynamicMapper.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TheMapMethod
    {
        private readonly Random random = new Random();

        [TestMethod]
        public void ShouldCopyProperties()
        {
            // Arrange
            var entity = CreateEntity();

            var properties = new[]
            {
                typeof(Entity).GetProperty("Int1"),
                typeof(Entity).GetProperty("String2"),
                typeof(Entity).GetProperty("String4"),
                typeof(Entity).GetProperty("Double1"),
                typeof(Entity).GetProperty("Double2")
            };

            // Act
            var result = Mapper<Entity>.Map(entity, properties);

            // Assert
            Assert.AreEqual(entity.Int1, result.Int1);
            Assert.AreEqual(entity.String2, result.String2);
            Assert.AreEqual(entity.String4, result.String4);
            Assert.AreEqual(entity.Double1, result.Double1);
            Assert.AreEqual(entity.Double2, result.Double2);

            Assert.AreEqual(5, ((IDictionary<String, object>)result).Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowAnArgumentNullExceptionIfPropertiesIsNull()
        {
            // Arrange
            var entity = new Entity();

            // Act
            Mapper<Entity>.Map(entity, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowAnArgumentNullExceptionIfEntityIsNull()
        {
            // Arrange
            var properties = new PropertyInfo[0];

            // Act
            Mapper<Entity>.Map(null, properties);
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
