using FluentAssertions;
using GamesService.DTOs;
using GamesService.Mappers;
using GamesService.Models;
using Xunit;

namespace GamesService.Tests.Mappers
{
    public class GameMapperTests
    {
        #region ToDto Tests

        [Fact]
        public void ToDto_MapsAllProperties_FromGameToGameDto()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Name = "Test Game",
                Genre = "Action",
                AgeRating = "M",
                Price = 59.99m,
                Description = "A test game description",
                Author = "Test Studio"
            };

            // Act
            var result = game.ToDto();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(game.Id);
            result.Name.Should().Be(game.Name);
            result.Genre.Should().Be(game.Genre);
            result.AgeRating.Should().Be(game.AgeRating);
            result.Price.Should().Be(game.Price);
            result.Description.Should().Be(game.Description);
            result.Author.Should().Be(game.Author);
        }

        [Fact]
        public void ToDto_HandlesEmptyStrings()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Name = string.Empty,
                Genre = string.Empty,
                AgeRating = string.Empty,
                Price = 0,
                Description = string.Empty,
                Author = string.Empty
            };

            // Act
            var result = game.ToDto();

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().BeEmpty();
            result.Genre.Should().BeEmpty();
            result.AgeRating.Should().BeEmpty();
            result.Description.Should().BeEmpty();
            result.Author.Should().BeEmpty();
        }

        [Fact]
        public void ToDto_HandlesZeroPrice()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Name = "Free Game",
                Genre = "Casual",
                Price = 0m,
                AgeRating = "E",
                Description = "Free to play",
                Author = "Indie Dev"
            };

            // Act
            var result = game.ToDto();

            // Assert
            result.Price.Should().Be(0m);
        }

        #endregion

        #region ToEntity Tests

        [Fact]
        public void ToEntity_MapsAllProperties_FromCreateGameDtoToGame()
        {
            // Arrange
            var createDto = new CreateGameDto
            {
                Name = "New Game",
                Genre = "RPG",
                AgeRating = "T",
                Price = 49.99m,
                Description = "An exciting new game",
                Author = "New Studio"
            };

            // Act
            var result = createDto.ToEntity();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(0); // ID not set yet
            result.Name.Should().Be(createDto.Name);
            result.Genre.Should().Be(createDto.Genre);
            result.AgeRating.Should().Be(createDto.AgeRating);
            result.Price.Should().Be(createDto.Price);
            result.Description.Should().Be(createDto.Description);
            result.Author.Should().Be(createDto.Author);
        }

        [Fact]
        public void ToEntity_CreatesNewInstance()
        {
            // Arrange
            var createDto = new CreateGameDto
            {
                Name = "Test",
                Genre = "Action",
                Price = 59.99m,
                AgeRating = "M",
                Description = "Test",
                Author = "Studio"
            };

            // Act
            var result1 = createDto.ToEntity();
            var result2 = createDto.ToEntity();

            // Assert
            result1.Should().NotBeSameAs(result2);
        }

        [Fact]
        public void ToEntity_HandlesEmptyStrings()
        {
            // Arrange
            var createDto = new CreateGameDto
            {
                Name = string.Empty,
                Genre = string.Empty,
                AgeRating = string.Empty,
                Price = 0,
                Description = string.Empty,
                Author = string.Empty
            };

            // Act
            var result = createDto.ToEntity();

            // Assert
            result.Name.Should().BeEmpty();
            result.Genre.Should().BeEmpty();
        }

        #endregion

        #region UpdateEntity Tests

        [Fact]
        public void UpdateEntity_UpdatesAllProperties_FromUpdateGameDtoToGame()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Name = "Original Name",
                Genre = "Original Genre",
                AgeRating = "E",
                Price = 29.99m,
                Description = "Original description",
                Author = "Original Author"
            };

            var updateDto = new UpdateGameDto
            {
                Name = "Updated Name",
                Genre = "Updated Genre",
                AgeRating = "M",
                Price = 59.99m,
                Description = "Updated description",
                Author = "Updated Author"
            };

            // Act
            updateDto.UpdateEntity(game);

            // Assert
            game.Id.Should().Be(1); // ID should not change
            game.Name.Should().Be(updateDto.Name);
            game.Genre.Should().Be(updateDto.Genre);
            game.AgeRating.Should().Be(updateDto.AgeRating);
            game.Price.Should().Be(updateDto.Price);
            game.Description.Should().Be(updateDto.Description);
            game.Author.Should().Be(updateDto.Author);
        }

        [Fact]
        public void UpdateEntity_PreservesId()
        {
            // Arrange
            var originalId = 42;
            var game = new Game
            {
                Id = originalId,
                Name = "Test",
                Genre = "Action",
                Price = 59.99m,
                AgeRating = "M",
                Description = "Test",
                Author = "Studio"
            };

            var updateDto = new UpdateGameDto
            {
                Name = "Updated",
                Genre = "RPG",
                Price = 49.99m,
                AgeRating = "T",
                Description = "Updated",
                Author = "New Studio"
            };

            // Act
            updateDto.UpdateEntity(game);

            // Assert
            game.Id.Should().Be(originalId);
        }

        [Fact]
        public void UpdateEntity_ModifiesSameInstance()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Name = "Original",
                Genre = "Action",
                Price = 59.99m,
                AgeRating = "M",
                Description = "Test",
                Author = "Studio"
            };

            var updateDto = new UpdateGameDto
            {
                Name = "Updated",
                Genre = "RPG",
                Price = 49.99m,
                AgeRating = "T",
                Description = "Updated",
                Author = "New Studio"
            };

            // Act
            var originalReference = game;
            updateDto.UpdateEntity(game);

            // Assert
            game.Should().BeSameAs(originalReference);
            game.Name.Should().Be("Updated");
        }

        [Fact]
        public void UpdateEntity_HandlesEmptyStrings()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Name = "Original",
                Genre = "Action",
                Price = 59.99m,
                AgeRating = "M",
                Description = "Test",
                Author = "Studio"
            };

            var updateDto = new UpdateGameDto
            {
                Name = string.Empty,
                Genre = string.Empty,
                AgeRating = string.Empty,
                Price = 0,
                Description = string.Empty,
                Author = string.Empty
            };

            // Act
            updateDto.UpdateEntity(game);

            // Assert
            game.Name.Should().BeEmpty();
            game.Genre.Should().BeEmpty();
            game.Price.Should().Be(0);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void MapperChain_CreateToEntityToDtoToUpdate_WorksCorrectly()
        {
            // Arrange - Create
            var createDto = new CreateGameDto
            {
                Name = "Chain Test",
                Genre = "Adventure",
                AgeRating = "E",
                Price = 39.99m,
                Description = "Testing mapper chain",
                Author = "Test Studio"
            };

            // Act - Create to Entity
            var entity = createDto.ToEntity();
            entity.Id = 1; // Simulate DB assignment

            // Act - Entity to DTO
            var dto = entity.ToDto();

            // Act - Update
            var updateDto = new UpdateGameDto
            {
                Name = dto.Name + " Updated",
                Genre = dto.Genre,
                AgeRating = dto.AgeRating,
                Price = dto.Price + 10,
                Description = dto.Description + " Updated",
                Author = dto.Author
            };
            updateDto.UpdateEntity(entity);

            // Assert
            entity.Name.Should().Be("Chain Test Updated");
            entity.Price.Should().Be(49.99m);
            entity.Description.Should().Contain("Updated");
        }

        #endregion
    }
}
