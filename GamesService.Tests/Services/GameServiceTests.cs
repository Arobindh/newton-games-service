using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using GamesService.DTOs;
using GamesService.Exceptions;
using GamesService.Models;
using GamesService.Repositories.Interfaces;
using GamesService.Services;
using Xunit;

namespace GamesService.Tests.Services
{
    public class GameServiceTests
    {
        private readonly Mock<IRepository<Game>> _mockRepository;
        private readonly Mock<ILogger<GameService>> _mockLogger;
        private readonly GameService _service;

        public GameServiceTests()
        {
            _mockRepository = new Mock<IRepository<Game>>();
            _mockLogger = new Mock<ILogger<GameService>>();
            _service = new GameService(_mockRepository.Object, _mockLogger.Object);
        }

        #region GetAllGamesAsync Tests

        [Fact]
        public async Task GetAllGamesAsync_ReturnsListOfGameDtos()
        {
            // Arrange
            var games = new List<Game>
            {
                new Game { Id = 1, Name = "Game 1", Genre = "Action", Price = 59.99m, AgeRating = "M", Description = "Test", Author = "Author 1" },
                new Game { Id = 2, Name = "Game 2", Genre = "RPG", Price = 49.99m, AgeRating = "T", Description = "Test 2", Author = "Author 2" }
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(games);

            // Act
            var result = await _service.GetAllGamesAsync();

            // Assert
            result.Should().HaveCount(2);
            result[0].Id.Should().Be(1);
            result[0].Name.Should().Be("Game 1");
            result[1].Id.Should().Be(2);
            result[1].Name.Should().Be("Game 2");
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllGamesAsync_ReturnsEmptyList_WhenNoGamesExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Game>());

            // Act
            var result = await _service.GetAllGamesAsync();

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region GetGameByIdAsync Tests

        [Fact]
        public async Task GetGameByIdAsync_ReturnsGameDto_WhenGameExists()
        {
            // Arrange
            var gameId = 1;
            var game = new Game 
            { 
                Id = gameId, 
                Name = "Test Game", 
                Genre = "Action", 
                Price = 59.99m,
                AgeRating = "M",
                Description = "A test game",
                Author = "Test Studio"
            };
            _mockRepository.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(game);

            // Act
            var result = await _service.GetGameByIdAsync(gameId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(gameId);
            result.Name.Should().Be("Test Game");
            result.Genre.Should().Be("Action");
            result.Price.Should().Be(59.99m);
        }

        [Fact]
        public async Task GetGameByIdAsync_ThrowsNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync((Game?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetGameByIdAsync(gameId));
            exception.Message.Should().Contain("Game");
            exception.Message.Should().Contain("999");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task GetGameByIdAsync_ThrowsBadRequestException_WhenIdIsInvalid(int invalidId)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _service.GetGameByIdAsync(invalidId));
            exception.Message.Should().Contain("positive value");
        }

        #endregion

        #region CreateGameAsync Tests

        [Fact]
        public async Task CreateGameAsync_ReturnsCreatedGameDto()
        {
            // Arrange
            var createDto = new CreateGameDto
            {
                Name = "New Game",
                Genre = "Adventure",
                Price = 39.99m,
                AgeRating = "E",
                Description = "A new game",
                Author = "New Studio"
            };

            var createdGame = new Game
            {
                Id = 1,
                Name = createDto.Name,
                Genre = createDto.Genre,
                Price = createDto.Price,
                AgeRating = createDto.AgeRating,
                Description = createDto.Description,
                Author = createDto.Author
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Game>()))
                .ReturnsAsync((Game g) => 
                {
                    g.Id = 1;
                    return g;
                });

            // Act
            var result = await _service.CreateGameAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(createDto.Name);
            result.Genre.Should().Be(createDto.Genre);
            result.Price.Should().Be(createDto.Price);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Once);
        }

        [Fact]
        public async Task CreateGameAsync_LogsInformation()
        {
            // Arrange
            var createDto = new CreateGameDto
            {
                Name = "New Game",
                Genre = "Adventure",
                Price = 39.99m,
                AgeRating = "E",
                Description = "Test",
                Author = "Studio"
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Game>()))
                .ReturnsAsync((Game g) => 
                {
                    g.Id = 1;
                    return g;
                });

            // Act
            await _service.CreateGameAsync(createDto);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Created game")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region UpdateGameAsync Tests

        [Fact]
        public async Task UpdateGameAsync_ReturnsUpdatedGameDto_WhenGameExists()
        {
            // Arrange
            var gameId = 1;
            var existingGame = new Game
            {
                Id = gameId,
                Name = "Old Name",
                Genre = "Action",
                Price = 59.99m,
                AgeRating = "M",
                Description = "Old description",
                Author = "Old Studio"
            };

            var updateDto = new UpdateGameDto
            {
                Name = "Updated Name",
                Genre = "RPG",
                Price = 49.99m,
                AgeRating = "T",
                Description = "Updated description",
                Author = "Updated Studio"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(existingGame);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Game>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateGameAsync(gameId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(gameId);
            result.Name.Should().Be(updateDto.Name);
            result.Genre.Should().Be(updateDto.Genre);
            result.Price.Should().Be(updateDto.Price);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Game>()), Times.Once);
        }

        [Fact]
        public async Task UpdateGameAsync_ThrowsNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = 999;
            var updateDto = new UpdateGameDto { Name = "Test", Genre = "Action", Price = 59.99m };
            _mockRepository.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync((Game?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateGameAsync(gameId, updateDto));
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Game>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task UpdateGameAsync_ThrowsBadRequestException_WhenIdIsInvalid(int invalidId)
        {
            // Arrange
            var updateDto = new UpdateGameDto { Name = "Test", Genre = "Action", Price = 59.99m };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateGameAsync(invalidId, updateDto));
        }

        [Fact]
        public async Task UpdateGameAsync_LogsInformation()
        {
            // Arrange
            var gameId = 1;
            var existingGame = new Game { Id = gameId, Name = "Test", Genre = "Action", Price = 59.99m };
            var updateDto = new UpdateGameDto { Name = "Updated", Genre = "RPG", Price = 49.99m };

            _mockRepository.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(existingGame);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Game>())).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateGameAsync(gameId, updateDto);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Updated game")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region DeleteGameAsync Tests

        [Fact]
        public async Task DeleteGameAsync_DeletesGame_WhenGameExists()
        {
            // Arrange
            var gameId = 1;
            var game = new Game { Id = gameId, Name = "Test Game", Genre = "Action", Price = 59.99m };
            _mockRepository.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(game);
            _mockRepository.Setup(r => r.DeleteAsync(game)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteGameAsync(gameId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(It.Is<Game>(g => g.Id == gameId)), Times.Once);
        }

        [Fact]
        public async Task DeleteGameAsync_ThrowsNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync((Game?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteGameAsync(gameId));
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Game>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task DeleteGameAsync_ThrowsBadRequestException_WhenIdIsInvalid(int invalidId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _service.DeleteGameAsync(invalidId));
        }

        [Fact]
        public async Task DeleteGameAsync_LogsInformation()
        {
            // Arrange
            var gameId = 1;
            var game = new Game { Id = gameId, Name = "Test", Genre = "Action", Price = 59.99m };
            _mockRepository.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(game);
            _mockRepository.Setup(r => r.DeleteAsync(game)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteGameAsync(gameId);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Deleted game")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion
    }
}
