using GamesService.DTOs;
using GamesService.Exceptions;
using GamesService.Mappers;
using GamesService.Models;
using GamesService.Repositories.Interfaces;
using GamesService.Services.Interfaces;

namespace GamesService.Services
{
    public class GameService : IGameService
    {
        private readonly IRepository<Game> _gameRepository;
        private readonly ILogger<GameService> _logger;

        public GameService(IRepository<Game> gameRepository, ILogger<GameService> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<List<GameDto>> GetAllGamesAsync()
        {
            var games = await _gameRepository.GetAllAsync();
            return games.Select(g => g.ToDto()).ToList();
        }

        public async Task<GameDto> GetGameByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Game ID must be a positive value.");
            }

            var game = await _gameRepository.GetByIdAsync(id);
            
            if (game == null)
            {
                throw new NotFoundException("Game", id);
            }
            
            return game.ToDto();
        }

        public async Task<GameDto> CreateGameAsync(CreateGameDto createGameDto)
        {
            var game = createGameDto.ToEntity();
            
            await _gameRepository.AddAsync(game);
            
            _logger.LogInformation("Created game with ID: {GameId}", game.Id);
            return game.ToDto();
        }

        public async Task<GameDto> UpdateGameAsync(int id, UpdateGameDto updateGameDto)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Game ID must be a positive value.");
            }

            var game = await _gameRepository.GetByIdAsync(id);
            
            if (game == null)
            {
                throw new NotFoundException("Game", id);
            }
            
            updateGameDto.UpdateEntity(game);
            await _gameRepository.UpdateAsync(game);
            
            _logger.LogInformation("Updated game with ID: {GameId}", id);
            return game.ToDto();
        }

        public async Task DeleteGameAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Game ID must be a positive value.");
            }

            var game = await _gameRepository.GetByIdAsync(id);
            
            if (game == null)
            {
                throw new NotFoundException("Game", id);
            }
            
            await _gameRepository.DeleteAsync(game);
            
            _logger.LogInformation("Deleted game with ID: {GameId}", id);
        }
    }
}
