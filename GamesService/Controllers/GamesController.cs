using Microsoft.AspNetCore.Mvc;
using GamesService.DTOs;
using GamesService.Services;
using GamesService.Services.Interfaces;

namespace GamesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetAllGames()
        {
            var games = await _gameService.GetAllGamesAsync();
            return Ok(games);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<GameDto>> GetGame(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            return Ok(game);
        }

        [HttpPost]
        public async Task<ActionResult<GameDto>> CreateGame([FromBody] CreateGameDto createGameDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var game = await _gameService.CreateGameAsync(createGameDto);
            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<ActionResult<GameDto>> UpdateGame(int id, [FromBody] UpdateGameDto updateGameDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedGame = await _gameService.UpdateGameAsync(id, updateGameDto);
            return Ok(updatedGame);
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<ActionResult> DeleteGame(int id)
        {
            await _gameService.DeleteGameAsync(id);
            return NoContent();
        }
    }
}
