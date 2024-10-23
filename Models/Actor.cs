using System.ComponentModel.DataAnnotations;

namespace Fall2024_Assignment3_gdhakal.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Imdb { get; set; }
        public byte[]? Photo { get; set; }

    }
}
