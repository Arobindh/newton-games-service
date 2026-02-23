using GamesService.DTOs;
using GamesService.Models;

namespace GamesService.Services.Interfaces
{
    public interface IGameService
    {
        Task<List<GameDto>> GetAllGamesAsync();
        Task<GameDto> GetGameByIdAsync(int id);
        Task<GameDto> CreateGameAsync(CreateGameDto createGameDto);
        Task<GameDto> UpdateGameAsync(int id, UpdateGameDto updateGameDto);
        Task DeleteGameAsync(int id);
    }
}

