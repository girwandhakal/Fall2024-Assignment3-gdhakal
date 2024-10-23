namespace Fall2024_Assignment3_gdhakal.Models
{
    public class ActorDetailsViewModel
    {
        public Actor Actor { get; set; }
        public List<(string Username, string Tweet, double Sentiment)> TweetsWithSentiment { get; set; }
        public double AverageSentiment { get; set; }
    }
}
