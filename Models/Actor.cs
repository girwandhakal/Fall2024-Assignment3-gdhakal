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

        [RegularExpression(@"^https:\/\/www\.imdb\.com\/.*$",
    ErrorMessage = "The IMDb link must start with 'https://www.imdb.com/'")]
        public string Imdb { get; set; }
        public byte[]? Photo { get; set; }
        public ICollection<MovieActor>? MovieActors { get; set; }

    }
}
