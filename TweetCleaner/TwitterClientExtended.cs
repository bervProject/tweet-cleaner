// Copyright 2021 Bervianto Leo Pratama
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using MongoDB.Bson;
using MongoDB.Driver;
using Tweetinvi;

namespace TweetCleaner
{
    public record TweetDocument
    {
        public long TweetId { get; set; }
        public DateTimeOffset TweetTime { get; set; }
        public DateTime InsertedAt { get; set; } = DateTime.UtcNow;
        public string TweetCreator { get; set; }
    }

    public static class TwitterClientExtended
    {
        public static async Task<int> CleanMyTimeLine(this ITwitterClient userClient, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DB_NAME", EnvironmentVariableTarget.Machine));
            var collections = database.GetCollection<BsonDocument>("deleted");
            var user = await userClient.Users.GetAuthenticatedUserAsync();
            var tweets = await userClient.Timelines.GetUserTimelineAsync(user);
            Console.WriteLine($"Getting tweets: {tweets.Length}");
            var deletedIds = new List<TweetDocument>(tweets.Length);
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
                deletedIds.Add(new TweetDocument { TweetId = tweet.Id, TweetTime = tweet.CreatedAt, TweetCreator = tweet.CreatedBy.Name });

            }
            if (deletedIds.Count > 0)
            {
                var documents = deletedIds.Select(x => x.ToBsonDocument());
                await collections.InsertManyAsync(documents);
            }
            return deletedIds.Count;
        }

        public static async Task<int> Unfav(this ITwitterClient userClient, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DB_NAME", EnvironmentVariableTarget.Machine));
            var collections = database.GetCollection<BsonDocument>("unfav");
            var user = await userClient.Users.GetAuthenticatedUserAsync();
            var tweets = await userClient.Tweets.GetUserFavoriteTweetsAsync(user);
            Console.WriteLine($"Getting tweets: {tweets.Length}");
            var unfavIds = new List<TweetDocument>(tweets.Length);
            foreach (var tweet in tweets)
            {
                try
                {
                    Console.WriteLine($"Tweet: {tweet.Text}, {tweet.CreatedBy}");
                    await userClient.Tweets.UnfavoriteTweetAsync(tweet);
                    Console.WriteLine($"Unfaved: {tweet.Id}");
                    unfavIds.Add(new TweetDocument { TweetId = tweet.Id, TweetTime = tweet.CreatedAt, TweetCreator = tweet.CreatedBy.Name });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            if (unfavIds.Count > 0)
            {
                var documents = unfavIds.Select(x => x.ToBsonDocument());
                await collections.InsertManyAsync(documents);
            }
            return unfavIds.Count;
        }
    }
}
