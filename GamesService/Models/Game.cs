using System.ComponentModel.DataAnnotations;

namespace GamesService.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Genre { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string AgeRating { get; set; } = string.Empty;
        
        public decimal Price { get; set; }
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string Author { get; set; } = string.Empty;
    }
}
