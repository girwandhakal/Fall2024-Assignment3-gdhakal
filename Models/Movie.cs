using System.ComponentModel.DataAnnotations;

namespace Fall2024_Assignment3_gdhakal.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [RegularExpression(@"^https:\/\/www\.imdb\.com\/.*$",
    ErrorMessage = "The IMDb link must start with 'https://www.imdb.com/'")]
        public string Imdb { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public byte[]? Poster { get; set; }
        public ICollection<MovieActor>? MovieActors { get; set; }

    }
}



  