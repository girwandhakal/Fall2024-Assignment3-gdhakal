using System.ClientModel;
using System.Text.Json.Nodes;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using VaderSharp2;
using System.Text.Json;


namespace Fall2024_Assignment3_gdhakal.Services
{


    public class AzureOpenAIService
    {
        private readonly string ApiKey;
        private const string ApiEndpoint = "https://fall2023-assignment3-mdmay1-openai.openai.azure.com/";
        private const string AiDeployment = "gpt-35-turbo";
        private ApiKeyCredential _apiCredential;

        public AzureOpenAIService(IConfiguration configuration)
        {
            ApiKey = configuration["ApiSettings:ApiKey"] ?? throw new Exception("OpenAI API key not found in the configuration.");
            _apiCredential = new(ApiKey);
        }

        public async Task<List<(string Review, double Sentiment)>> MovieReviewsMultipleCalls(string MovieName, int MovieYear)
        {

            var reviewWithSentiment = new List<(string Review, double Sentiment)>();
            ChatClient chatClient = new AzureOpenAIClient(new Uri(ApiEndpoint), _apiCredential).GetChatClient(AiDeployment);

            string[] personas = { "is harsh", "loves romance", "loves comedy", "loves thrillers", "loves fantasy", "is a sci-fi fan", "adores historical dramas", "enjoys indie films", "loves action-packed blockbusters", "appreciates artistic and experimental films " };
            var reviews = new List<string>();
            foreach (string persona in personas)
            {
                var messages = new ChatMessage[]
                {
                    new SystemChatMessage($"You are a film reviewer and film critic who {persona}."),
                    new UserChatMessage($"How would you rate the movie {MovieName} released in {MovieYear} out of 10 in less than 105 words?")
                };
                var chatCompletionOptions = new ChatCompletionOptions
                {
                    MaxOutputTokenCount = 200,
                };
                ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages, chatCompletionOptions);

                reviews.Add(result.Value.Content[0].Text);
                Thread.Sleep(TimeSpan.FromSeconds(10)); // Request throttle due to rate limit
            }

            //calculate the sentiment for each review
            var analyzer = new SentimentIntensityAnalyzer();
            foreach (var review in reviews)
            {
                var sentiment = analyzer.PolarityScores(review);
                reviewWithSentiment.Add((Review: review, Sentiment: sentiment.Compound));
            }

            return reviewWithSentiment;


        }
        public async Task<List<(string Username, string Tweet, double Sentiment)>> GenerateTweetsForActor(string actorName)
        {
            ChatClient client = new AzureOpenAIClient(new Uri(ApiEndpoint), _apiCredential).GetChatClient(AiDeployment);

            var messages = new ChatMessage[]
            {
        new SystemChatMessage("You represent the Twitter social media platform. Generate an answer with a valid JSON formatted array of objects containing the tweet and username. The response should start with [ and end with ]. No additional text."),
        new UserChatMessage($"Generate 20 tweets from a variety of users about the actor {actorName}.")
            };

            ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);

            string tweetsJsonString = result.Value.Content.FirstOrDefault()?.Text ?? "[]";
            //Console.WriteLine("Raw Response: " + tweetsJsonString);

            //  extract valid JSON 
            int startIndex = tweetsJsonString.IndexOf('[');
            int endIndex = tweetsJsonString.LastIndexOf(']');

            if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
            {
                throw new InvalidOperationException("The API response does not contain valid JSON.");
            }

            // Extract valid JSON 
            string cleanedJsonString = tweetsJsonString.Substring(startIndex, (endIndex - startIndex) + 1);
            Console.WriteLine("Cleaned JSON: " + cleanedJsonString);

            JsonArray json;
            try
            {
                json = JsonNode.Parse(cleanedJsonString)!.AsArray();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse the response as JSON: " + cleanedJsonString, ex);
            }

            var analyzer = new SentimentIntensityAnalyzer();
            var tweetsWithSentiment = new List<(string Username, string Tweet, double Sentiment)>();

            foreach (var tweetNode in json)
            {
                string username = tweetNode!["username"]?.ToString() ?? "Unknown";
                string tweet = tweetNode!["tweet"]?.ToString() ?? "";

                var sentiment = analyzer.PolarityScores(tweet);

                tweetsWithSentiment.Add((username, tweet, sentiment.Compound));

                //Console.WriteLine($"{username}: \"{tweet}\" (sentiment: {sentiment.Compound})");
            }

            return tweetsWithSentiment;
        }



    }
}

