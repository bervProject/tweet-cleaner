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

using MongoDB.Driver;
using TweetCleaner;
using Tweetinvi;

var mongoDbClient = new MongoClient(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"));

var startTime = DateTimeOffset.UtcNow;
var userClient = new TwitterClient(Environment.GetEnvironmentVariable("CONSUMER_KEY"),
    Environment.GetEnvironmentVariable("CONSUMER_SECRET"),
    Environment.GetEnvironmentVariable("ACCESS_TOKEN"),
    Environment.GetEnvironmentVariable("ACCESS_SECRET"));
var deletedTweetTask = userClient.CleanMyTimeLine(mongoDbClient);
var unfavTweetTask = userClient.Unfav(mongoDbClient);
Task.WaitAll(deletedTweetTask, unfavTweetTask);
Console.WriteLine($"Deleted {deletedTweetTask.Result}, unfav {unfavTweetTask.Result}");
var endTime = DateTimeOffset.UtcNow;
Console.WriteLine($"Elapsed time: {(endTime - startTime).TotalSeconds} s");




