using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using GamesService.Controllers;
using GamesService.DTOs;
using GamesService.Exceptions;
using GamesService.Services.Interfaces;
using Xunit;

namespace GamesService.Tests.Controllers
{
    public class GamesControllerTests
    {
        private readonly Mock<IGameService> _mockGameService;
        private readonly GamesController _controller;

        public GamesControllerTests()
        {
            _mockGameService = new Mock<IGameService>();
            _controller = new GamesController(_mockGameService.Object);
        }

        [Fact]
        public async Task GetAllGames_ReturnsOkResult_WithListOfGames()
        {
            // Arrange
            var games = new List<GameDto>
            {
                new GameDto { Id = 1, Name = "Game 1", Genre = "Action", Price = 59.99m },
                new GameDto { Id = 2, Name = "Game 2", Genre = "RPG", Price = 49.99m }
            };
            _mockGameService.Setup(s => s.GetAllGamesAsync()).ReturnsAsync(games);

            // Act
            var result = await _controller.GetAllGames();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(games);
            _mockGameService.Verify(s => s.GetAllGamesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllGames_ReturnsEmptyList_WhenNoGamesExist()
        {
            // Arrange
            _mockGameService.Setup(s => s.GetAllGamesAsync()).ReturnsAsync(new List<GameDto>());

            // Act
            var result = await _controller.GetAllGames();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var games = okResult!.Value as List<GameDto>;
            games.Should().BeEmpty();
        }

        [Fact]
        public async Task GetGame_ReturnsOkResult_WithGame()
        {
            // Arrange
            var gameId = 1;
            var game = new GameDto { Id = gameId, Name = "Test Game", Genre = "Action", Price = 59.99m };
            _mockGameService.Setup(s => s.GetGameByIdAsync(gameId)).ReturnsAsync(game);

            // Act
            var result = await _controller.GetGame(gameId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(game);
            _mockGameService.Verify(s => s.GetGameByIdAsync(gameId), Times.Once);
        }

        [Fact]
        public async Task GetGame_ThrowsNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = 999;
            _mockGameService.Setup(s => s.GetGameByIdAsync(gameId))
                .ThrowsAsync(new NotFoundException("Game", gameId));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetGame(gameId));
            _mockGameService.Verify(s => s.GetGameByIdAsync(gameId), Times.Once);
        }

        [Fact]
        public async Task CreateGame_ReturnsCreatedAtAction_WithCreatedGame()
        {
            // Arrange
            var createDto = new CreateGameDto 
            { 
                Name = "New Game", 
                Genre = "Adventure", 
                Price = 39.99m,
                AgeRating = "T",
                Description = "A great game",
                Author = "Game Studio"
            };
            var createdGame = new GameDto 
            { 
                Id = 1, 
                Name = createDto.Name, 
                Genre = createDto.Genre, 
                Price = createDto.Price 
            };
            _mockGameService.Setup(s => s.CreateGameAsync(createDto)).ReturnsAsync(createdGame);

            // Act
            var result = await _controller.CreateGame(createDto);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(nameof(_controller.GetGame));
            createdResult.RouteValues!["id"].Should().Be(createdGame.Id);
            createdResult.Value.Should().BeEquivalentTo(createdGame);
            _mockGameService.Verify(s => s.CreateGameAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task UpdateGame_ReturnsOkResult_WithUpdatedGame()
        {
            // Arrange
            var gameId = 1;
            var updateDto = new UpdateGameDto 
            { 
                Name = "Updated Game", 
                Genre = "RPG", 
                Price = 49.99m,
                AgeRating = "M",
                Description = "Updated description",
                Author = "Updated Studio"
            };
            var updatedGame = new GameDto 
            { 
                Id = gameId, 
                Name = updateDto.Name, 
                Genre = updateDto.Genre, 
                Price = updateDto.Price 
            };
            _mockGameService.Setup(s => s.UpdateGameAsync(gameId, updateDto)).ReturnsAsync(updatedGame);

            // Act
            var result = await _controller.UpdateGame(gameId, updateDto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(updatedGame);
            _mockGameService.Verify(s => s.UpdateGameAsync(gameId, updateDto), Times.Once);
        }

        [Fact]
        public async Task UpdateGame_ThrowsNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = 999;
            var updateDto = new UpdateGameDto { Name = "Test", Genre = "Action", Price = 59.99m };
            _mockGameService.Setup(s => s.UpdateGameAsync(gameId, updateDto))
                .ThrowsAsync(new NotFoundException("Game", gameId));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateGame(gameId, updateDto));
        }

        [Fact]
        public async Task DeleteGame_ReturnsNoContent_WhenGameIsDeleted()
        {
            // Arrange
            var gameId = 1;
            _mockGameService.Setup(s => s.DeleteGameAsync(gameId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteGame(gameId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockGameService.Verify(s => s.DeleteGameAsync(gameId), Times.Once);
        }

        [Fact]
        public async Task DeleteGame_ThrowsNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = 999;
            _mockGameService.Setup(s => s.DeleteGameAsync(gameId))
                .ThrowsAsync(new NotFoundException("Game", gameId));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteGame(gameId));
        }
    }
}
