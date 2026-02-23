using GamesService.DTOs;
using GamesService.Models;

namespace GamesService.Mappers
{
    public static class GameMapper
    {
        public static GameDto ToDto(this Game game)
        {
            return new GameDto
            {
                Id = game.Id,
                Name = game.Name,
                Genre = game.Genre,
                AgeRating = game.AgeRating,
                Price = game.Price,
                Description = game.Description,
                Author = game.Author
            };
        }

        public static Game ToEntity(this CreateGameDto dto)
        {
            return new Game
            {
                Name = dto.Name,
                Genre = dto.Genre,
                AgeRating = dto.AgeRating,
                Price = dto.Price,
                Description = dto.Description,
                Author = dto.Author
            };
        }

        public static void UpdateEntity(this UpdateGameDto dto, Game game)
        {
            game.Name = dto.Name;
            game.Genre = dto.Genre;
            game.AgeRating = dto.AgeRating;
            game.Price = dto.Price;
            game.Description = dto.Description;
            game.Author = dto.Author;
        }
    }
}
