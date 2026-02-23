namespace GamesService.DTOs
{
    public class GameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string AgeRating { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }
}
