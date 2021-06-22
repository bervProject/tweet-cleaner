using System;
using System.Threading.Tasks;
using Tweetinvi;

namespace TweetCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            var userClient = new TwitterClient(Environment.GetEnvironmentVariable("CONSUMER_KEY"), Environment.GetEnvironmentVariable("CONSUMER_SECRET"), Environment.GetEnvironmentVariable("ACCESS_TOKEN"), Environment.GetEnvironmentVariable("ACCESS_SECRET"));
            var deletedTweetTask = CleanMyTimeLine(userClient);
            var unfavTweetTask = Unfav(userClient);
            Task.WaitAll(deletedTweetTask, unfavTweetTask);
            Console.WriteLine($"Deleted {deletedTweetTask.Result}, unfav {unfavTweetTask.Result}");
        }

        static async Task<int> CleanMyTimeLine(ITwitterClient userClient)
        {
            var user = await userClient.Users.GetAuthenticatedUserAsync();
            var tweets = await userClient.Timelines.GetUserTimelineAsync(user);
            Console.WriteLine($"Getting tweets: {tweets.Length}");
            int deletedTweet = 0;
            foreach (var tweet in tweets)
            {
                if (!string.IsNullOrWhiteSpace(tweet.InReplyToScreenName) && !user.ScreenName.Equals(tweet.InReplyToScreenName))
                {
                    Console.WriteLine($"Skip reply to another person: ${tweet.Text}");
                    continue;
                }
                Console.WriteLine($"Tweet: {tweet.Text}, {tweet.CreatedBy}");
                await userClient.Tweets.DestroyTweetAsync(tweet);
                Console.WriteLine($"Deleted: {tweet.Id}");
                deletedTweet += 1;
            }
            return deletedTweet;
        }

        static async Task<int> Unfav(ITwitterClient userClient)
        {
            var user = await userClient.Users.GetAuthenticatedUserAsync();
            var tweets = await userClient.Tweets.GetUserFavoriteTweetsAsync(user);
            Console.WriteLine($"Getting tweets: {tweets.Length}");
            int unfavCount = 0;
            foreach (var tweet in tweets)
            {
                try
                {
                    Console.WriteLine($"Tweet: {tweet.Text}, {tweet.CreatedBy}");
                    await userClient.Tweets.UnfavoriteTweetAsync(tweet);
                    Console.WriteLine($"Unfaved: {tweet.Id}");
                    unfavCount += 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return unfavCount;
        }
    }
}
