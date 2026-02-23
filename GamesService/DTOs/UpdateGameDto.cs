using System.ComponentModel.DataAnnotations;

namespace GamesService.DTOs
{
    public class UpdateGameDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(100, ErrorMessage = "Genre cannot exceed 100 characters")]
        public string Genre { get; set; } = string.Empty;
        
        [MaxLength(10, ErrorMessage = "AgeRating cannot exceed 10 characters")]
        public string AgeRating { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal Price { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(200, ErrorMessage = "Author cannot exceed 200 characters")]
        public string Author { get; set; } = string.Empty;
    }
}
