using Abstractions.Repositories;
using Models;
using MongoDB.Driver;
using Moq;
using Repositories.Mongo;
using Xunit;

namespace Repositories.Tests.Mongo
{
    public class RepositoryTests
    {
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoCollection<TestEntity>> _mockCollection;
        private readonly Repository<TestEntity> _repository;

        public RepositoryTests()
        {
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockCollection = new Mock<IMongoCollection<TestEntity>>();
            
            _mockDatabase.Setup(x => x.GetCollection<TestEntity>(It.IsAny<string>(), null))
                        .Returns(_mockCollection.Object);
            
            _repository = new Repository<TestEntity>(_mockDatabase.Object);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Constructor_WithValidDatabase_InitializesSuccessfully()
        {
            // Act & Assert - Constructor should complete without exception
            var repository = new Repository<TestEntity>(_mockDatabase.Object);
            Assert.NotNull(repository);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateAsync_WithValidEntity_CallsInsertOneAsync()
        {
            // Arrange
            var entity = new TestEntity { Name = "Test Entity" };

            // Act
            await _repository.CreateAsync(entity);

            // Assert
            _mockCollection.Verify(x => x.InsertOneAsync(entity, null, default), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateAsync_WithValidEntity_CallsReplaceOneAsync()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Updated Entity" };

            // Act
            await _repository.UpdateAsync(entity);

            // Assert
            _mockCollection.Verify(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<TestEntity>>(), 
                entity, 
                It.IsAny<ReplaceOptions>(), 
                default), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteAsync_WithValidId_CallsDeleteOneAsync()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var mockDeleteResult = new Mock<DeleteResult>();
            mockDeleteResult.SetupGet(x => x.DeletedCount).Returns(1);

            _mockCollection.Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<TestEntity>>(), 
                default))
                .ReturnsAsync(mockDeleteResult.Object);

            // Act
            var result = await _repository.DeleteAsync(entityId);

            // Assert
            _mockCollection.Verify(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<TestEntity>>(), 
                default), Times.Once);
            Assert.True(result);
        }

        // Test entity class for testing
        public class TestEntity : Base
        {
            public string? Name { get; set; }
        }
    }
}
