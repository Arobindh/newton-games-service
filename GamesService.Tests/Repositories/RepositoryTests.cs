using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using GamesService.Data;
using GamesService.Models;
using GamesService.Repositories;
using Xunit;

namespace GamesService.Tests.Repositories
{
    public class RepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Repository<Game> _repository;

        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new Repository<Game>(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsEntity_WhenEntityExists()
        {
            // Arrange
            var game = new Game 
            { 
                Name = "Test Game", 
                Genre = "Action", 
                Price = 59.99m,
                AgeRating = "M",
                Description = "Test description",
                Author = "Test Studio"
            };
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(game.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(game.Id);
            result.Name.Should().Be("Test Game");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenEntityDoesNotExist()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Arrange
            var games = new List<Game>
            {
                new Game { Name = "Game 1", Genre = "Action", Price = 59.99m, AgeRating = "M", Description = "Test 1", Author = "Studio 1" },
                new Game { Name = "Game 2", Genre = "RPG", Price = 49.99m, AgeRating = "T", Description = "Test 2", Author = "Studio 2" },
                new Game { Name = "Game 3", Genre = "Strategy", Price = 39.99m, AgeRating = "E", Description = "Test 3", Author = "Studio 3" }
            };
            await _context.Games.AddRangeAsync(games);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(g => g.Name == "Game 1");
            result.Should().Contain(g => g.Name == "Game 2");
            result.Should().Contain(g => g.Name == "Game 3");
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoEntitiesExist()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region FindAsync Tests

        [Fact]
        public async Task FindAsync_ReturnsMatchingEntities()
        {
            // Arrange
            var games = new List<Game>
            {
                new Game { Name = "Action Game", Genre = "Action", Price = 59.99m, AgeRating = "M", Description = "Test", Author = "Studio" },
                new Game { Name = "RPG Game", Genre = "RPG", Price = 49.99m, AgeRating = "T", Description = "Test", Author = "Studio" },
                new Game { Name = "Another Action", Genre = "Action", Price = 39.99m, AgeRating = "E", Description = "Test", Author = "Studio" }
            };
            await _context.Games.AddRangeAsync(games);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindAsync(g => g.Genre == "Action");

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(g => g.Genre == "Action");
        }

        [Fact]
        public async Task FindAsync_ReturnsEmptyList_WhenNoMatches()
        {
            // Arrange
            var game = new Game { Name = "Test", Genre = "Action", Price = 59.99m, AgeRating = "M", Description = "Test", Author = "Studio" };
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindAsync(g => g.Genre == "RPG");

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_AddsEntityToDatabase()
        {
            // Arrange
            var game = new Game 
            { 
                Name = "New Game", 
                Genre = "Adventure", 
                Price = 39.99m,
                AgeRating = "E",
                Description = "A new game",
                Author = "New Studio"
            };

            // Act
            var result = await _repository.AddAsync(game);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            
            var savedGame = await _context.Games.FindAsync(result.Id);
            savedGame.Should().NotBeNull();
            savedGame!.Name.Should().Be("New Game");
        }

        [Fact]
        public async Task AddAsync_AssignsId_ToNewEntity()
        {
            // Arrange
            var game = new Game { Name = "Test", Genre = "Action", Price = 59.99m, AgeRating = "M", Description = "Test", Author = "Studio" };

            // Act
            var result = await _repository.AddAsync(game);

            // Assert
            result.Id.Should().BeGreaterThan(0);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_UpdatesExistingEntity()
        {
            // Arrange
            var game = new Game { Name = "Original", Genre = "Action", Price = 59.99m, AgeRating = "M", Description = "Test", Author = "Studio" };
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            // Modify
            game.Name = "Updated";
            game.Price = 49.99m;

            // Act
            await _repository.UpdateAsync(game);

            // Assert
            var updatedGame = await _context.Games.FindAsync(game.Id);
            updatedGame.Should().NotBeNull();
            updatedGame!.Name.Should().Be("Updated");
            updatedGame.Price.Should().Be(49.99m);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_RemovesEntityFromDatabase()
        {
            // Arrange
            var game = new Game { Name = "To Delete", Genre = "Action", Price = 59.99m, AgeRating = "M", Description = "Test", Author = "Studio" };
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
            var gameId = game.Id;

            // Act
            await _repository.DeleteAsync(game);

            // Assert
            var deletedGame = await _context.Games.FindAsync(gameId);
            deletedGame.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_DecreasesEntityCount()
        {
            // Arrange
            var games = new List<Game>
            {
                new Game { Name = "Game 1", Genre = "Action", Price = 59.99m, AgeRating = "M", Description = "Test", Author = "Studio" },
                new Game { Name = "Game 2", Genre = "RPG", Price = 49.99m, AgeRating = "T", Description = "Test", Author = "Studio" }
            };
            await _context.Games.AddRangeAsync(games);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(games[0]);

            // Assert
            var remainingGames = await _context.Games.ToListAsync();
            remainingGames.Should().HaveCount(1);
            remainingGames[0].Name.Should().Be("Game 2");
        }

        #endregion

        #region ExistsAsync Tests

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenEntityExists()
        {
            // Arrange
            var game = new Game { Name = "Test", Genre = "Action", Price = 59.99m, AgeRating = "M", Description = "Test", Author = "Studio" };
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(game.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenEntityDoesNotExist()
        {
            // Act
            var result = await _repository.ExistsAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        #endregion
    }
}
