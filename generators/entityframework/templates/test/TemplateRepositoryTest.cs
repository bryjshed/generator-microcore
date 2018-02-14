using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace <%= namespace %>.Tests
{
    public class <%= modelName %>RepositoryTest
    {
        private readonly DbContextOptions<<%= appname %>Context> _dbContextOptions;

        public <%= modelName %>RepositoryTest()
        {
            var servicesProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            _dbContextOptions = new DbContextOptionsBuilder<<%= appname %>Context>()
                .UseInMemoryDatabase("<%= appname %>").UseInternalServiceProvider(servicesProvider).Options;
        }

        [Fact]
        public async void SaveAsync()
        {
            // Arrange
            var toSave = BogusData.Get<%= modelName %>();

            using (var context = new <%= appname %>Context(_dbContextOptions))
            {
                var repository = new <%= modelName %>Repository(
                     context,
                     new Mock<ILogger<<%= modelName %>Repository>>().Object);

                // Act
                await repository.SaveAsync(toSave);
            }

            using (var context = new <%= appname %>Context(_dbContextOptions))
            {
                // Assert
                Assert.Equal(1, await context.<%= modelName %>s.CountAsync());

                var newItem = await context.<%= modelName %>s
                    .SingleOrDefaultAsync(p => p.Id == toSave.Id);

                Assert.Equal(toSave, newItem, new <%= modelName %>Comparer());
            }
        }

        [Fact]
        public async void UpdateAsync()
        {
            // Arrange
            var toSave = BogusData.Get<%= modelName %>();


            using (var context = new <%= appname %>Context(_dbContextOptions))
            {
                context.<%= modelName %>s.Add(toSave);
                context.SaveChanges();
            }

            using (var context = new <%= appname %>Context(_dbContextOptions))
            {
                var repository = new <%= modelName %>Repository(
                     context,
                     new Mock<ILogger<<%= modelName %>Repository>>().Object);

                var toUpdate = BogusData.Get<%= modelName %>();
                toUpdate.Id = toSave.Id;

                // Act
                await repository.UpdateAsync(toUpdate);

                var newItem = await context.<%= modelName %>s.FindAsync(toSave.Id);

                // Assert
                Assert.Equal(toUpdate.Id, newItem.Id);
            }
        }

        [Fact]
        public async void Get<%= modelName %>_WithValidId_Returns<%= modelName %>()
        {
            // Arrange
            var model = BogusData.Get<%= modelName %>();

            using (var context = new <%= appname %>Context(_dbContextOptions))
            {
                context.<%= modelName %>s.Add(model);
                context.SaveChanges();
            }

            using (var context = new <%= appname %>Context(_dbContextOptions))
            {
                var repository = new <%= modelName %>Repository(
                    context,
                    new Mock<ILogger<<%= modelName %>Repository>>().Object);

                // Act
                var found = await repository.GetByIdAsync(model.Id);

                // Assert
                Assert.NotNull(found);
                Assert.Equal(model, found, new <%= modelName %>Comparer());
            }
        }

        [Fact]
        public async void Get<%= modelName %>_WithNotFoundId_ReturnsNull()
        {
            // Arrange
            var notfoundId = 3;

            using (var context = new <%= appname %>Context(_dbContextOptions))
            {
                var repository = new <%= modelName %>Repository(
                    context,
                    new Mock<ILogger<<%= modelName %>Repository>>().Object);

                // Act
                var notFound = await repository.GetByIdAsync(notfoundId);

                // Assert
                Assert.Null(notFound);
            }
        }

        [Fact]
        public async void ListAsync_Returns<%= modelName %>s()
        {
            // Arrange
            var item1 = BogusData.Get<%= modelName %>();
            var item2 = BogusData.Get<%= modelName %>();
            var item3 = BogusData.Get<%= modelName %>();

            using (var context = new <%= appname %>Context(_dbContextOptions))
            {
                context.<%= modelName %>s.Add(item1);
                context.<%= modelName %>s.Add(item2);
                context.<%= modelName %>s.Add(item3);
                context.SaveChanges();
            }

            using (var context = new <%= appname %>Context(_dbContextOptions))
            {
                var repository = new <%= modelName %>Repository(
                    context,
                    new Mock<ILogger<<%= modelName %>Repository>>().Object);

                // Act
                var found = await repository.ListAsync();

                // Assert
                Assert.NotNull(found);
                Assert.Equal(found.Count, 3);
            }
        }

        [Theory]
        [InlineData(null, 1, 10, 4)]
        [InlineData(null, 1, 2, 2)]
        [InlineData(null, 2, 1, 1)]
        [InlineData("Bill", 1, 10, 2)]
        [InlineData("Jimines", 1, 10, 1)]
        [InlineData("Jimi", 1, 10, 1)]
        public async void FindAsync_Query_Returns<%= modelName %>s(string query, int pageNumber, int pageSize, int count)
        {
            // Arrange
            SeedListData();

            using (var context = new <%= appname %>Context(_dbContextOptions))
            {
                var repository = new <%= modelName %>Repository(
                    context,
                    new Mock<ILogger<<%= modelName %>Repository>>().Object);

                // Act
                var search = await repository.FindAsync(query, pageNumber, pageSize);

                // Assert
                Assert.Equal(count, search.Results.Count);
            }
        }

        private void SeedListData()
        {
            using (var context = new <%= appname %>Context(_dbContextOptions))
            {
                context.<%= modelName %>s.Add(BogusData.Get<%= modelName %>(new KeyValuePair<string, object>("CreatedByDisplayName", "Bill")));
                context.<%= modelName %>s.Add(BogusData.Get<%= modelName %>(new KeyValuePair<string, object>("CreatedByDisplayName", "Bill")));
                context.<%= modelName %>s.Add(BogusData.Get<%= modelName %>(new KeyValuePair<string, object>("CreatedByDisplayName", "Jimines")));
                context.<%= modelName %>s.Add(BogusData.Get<%= modelName %>(new KeyValuePair<string, object>("CreatedByDisplayName", "Jimi")));
                context.SaveChanges();
            }
        }
    }
}
