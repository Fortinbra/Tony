using Abstractions.Repositories;
using Models;
using MongoDB.Driver;
using Moq;
using Repositories.Mongo;
using Xunit;

namespace Repositories.Tests.Mongo
{
    public class ReadOnlyRepositoryTests
    {
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoCollection<TestEntity>> _mockCollection;
        private readonly ReadOnlyRepository<TestEntity> _repository;

        public ReadOnlyRepositoryTests()
        {
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockCollection = new Mock<IMongoCollection<TestEntity>>();
            
            _mockDatabase.Setup(x => x.GetCollection<TestEntity>(It.IsAny<string>(), null))
                        .Returns(_mockCollection.Object);
            
            _repository = new ReadOnlyRepository<TestEntity>(_mockDatabase.Object);
        }

        [Fact]
        public void Constructor_WithValidDatabase_InitializesSuccessfully()
        {
            // Act & Assert - Constructor should complete without exception
            var repository = new ReadOnlyRepository<TestEntity>(_mockDatabase.Object);
            Assert.NotNull(repository);
        }

        [Fact]
        public async Task GetAsync_WithValidId_ReturnsEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var expectedEntity = new TestEntity { Id = entityId, Name = "Test Entity" };
            
            var mockCursor = new Mock<IAsyncCursor<TestEntity>>();
            mockCursor.Setup(x => x.Current).Returns(new List<TestEntity> { expectedEntity });
            mockCursor.SetupSequence(x => x.MoveNext(default)).Returns(true).Returns(false);
            mockCursor.SetupSequence(x => x.MoveNextAsync(default)).ReturnsAsync(true).ReturnsAsync(false);

            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<TestEntity>>(),
                It.IsAny<FindOptions<TestEntity>>(),
                default))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _repository.GetAsync(entityId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entityId, result.Id);
            Assert.Equal("Test Entity", result.Name);
        }

        [Fact]
        public async Task GetAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            
            var mockCursor = new Mock<IAsyncCursor<TestEntity>>();
            mockCursor.Setup(x => x.Current).Returns(new List<TestEntity>());
            mockCursor.Setup(x => x.MoveNext(default)).Returns(false);
            mockCursor.Setup(x => x.MoveNextAsync(default)).ReturnsAsync(false);

            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<TestEntity>>(),
                It.IsAny<FindOptions<TestEntity>>(),
                default))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _repository.GetAsync(entityId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ReturnsAllEntities()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new() { Id = Guid.NewGuid(), Name = "Entity 1" },
                new() { Id = Guid.NewGuid(), Name = "Entity 2" }
            };
            
            var mockCursor = new Mock<IAsyncCursor<TestEntity>>();
            mockCursor.Setup(x => x.Current).Returns(entities);
            mockCursor.SetupSequence(x => x.MoveNext(default)).Returns(true).Returns(false);
            mockCursor.SetupSequence(x => x.MoveNextAsync(default)).ReturnsAsync(true).ReturnsAsync(false);

            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<TestEntity>>(),
                It.IsAny<FindOptions<TestEntity>>(),
                default))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _repository.GetAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, e => e.Name == "Entity 1");
            Assert.Contains(result, e => e.Name == "Entity 2");
        }

        [Fact]
        public async Task GetAsync_WithPredicate_ReturnsMatchingEntities()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new() { Id = Guid.NewGuid(), Name = "Matching Entity" }
            };
            
            var mockCursor = new Mock<IAsyncCursor<TestEntity>>();
            mockCursor.Setup(x => x.Current).Returns(entities);
            mockCursor.SetupSequence(x => x.MoveNext(default)).Returns(true).Returns(false);
            mockCursor.SetupSequence(x => x.MoveNextAsync(default)).ReturnsAsync(true).ReturnsAsync(false);

            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<TestEntity>>(),
                It.IsAny<FindOptions<TestEntity>>(),
                default))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _repository.GetAsync(x => x.Name == "Matching Entity");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Matching Entity", result.First().Name);
        }

        // Test entity class for testing
        public class TestEntity : Base
        {
            public string? Name { get; set; }
        }
    }
}
