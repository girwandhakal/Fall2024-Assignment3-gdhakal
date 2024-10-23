namespace Fall2024_Assignment3_gdhakal.Models
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; }
        public List<(string Review, double Sentiment)> ReviewsWithSentiment { get; set; }
        public double AverageSentiment { get; set; }
    }

}
