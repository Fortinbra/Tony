
using Models;
using Xunit;

namespace Models.Tests
{
    public class BaseTests
    {
        [Fact]
        public void Base_Constructor_SetsIdToNewGuid()
        {
            // Act
            var baseEntity = new TestBaseEntity();

            // Assert
            Assert.NotEqual(Guid.Empty, baseEntity.Id);
        }

        [Fact]
        public void Base_MultipleInstances_HaveUniqueIds()
        {
            // Act
            var entity1 = new TestBaseEntity();
            var entity2 = new TestBaseEntity();

            // Assert
            Assert.NotEqual(entity2.Id, entity1.Id);
        }

        [Fact]
        public void Base_Id_CanBeSetToSpecificValue()
        {
            // Arrange
            var baseEntity = new TestBaseEntity();
            var specificId = Guid.NewGuid();

            // Act
            baseEntity.Id = specificId;

            // Assert
            Assert.Equal(specificId, baseEntity.Id);
        }

        [Fact]
        public void Base_Id_HasBsonIdAttribute()
        {
            // Arrange
            var property = typeof(Base).GetProperty(nameof(Base.Id));

            // Act
            var bsonIdAttribute = property?.GetCustomAttributes(typeof(MongoDB.Bson.Serialization.Attributes.BsonIdAttribute), false);

            // Assert
            Assert.NotNull(bsonIdAttribute);
            Assert.Single(bsonIdAttribute);
        }

        // Helper class for testing the abstract Base class
        private class TestBaseEntity : Base
        {
            // Empty implementation just for testing Base functionality
        }
    }
}
