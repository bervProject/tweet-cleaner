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

using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using MongoDB.Bson;
using MongoDB.Driver;

var mongoDbClient = new MongoClient(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"));
var database = mongoDbClient.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DB_NAME"));
var collections = database.GetCollection<BsonDocument>("unfav");
var now = DateTime.UtcNow;
var filterBuilder = Builders<BsonDocument>.Filter;
var filter = filterBuilder.Gt("InsertedAt", now.AddDays(-7)) & filterBuilder.Lt("InsertedAt", now);
var total = collections.CountDocuments(filter);
Console.WriteLine($"Total unvaf 7 Days before {now}: {total}");

var sendEmailTask = SendEmail(now, total);
sendEmailTask.Wait();

static async Task SendEmail(DateTime now, long total)
{
    using var emailClient = new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.APSoutheast1);
    var request = new SendEmailRequest()
    {
        ReplyToAddresses = new List<string> { "bervianto.leo@gmail.com" },
        Message = new Message()
        {
            Body = new Body(new Content($"Total unvaf 7 Days before {now}: {total}")),
            Subject = new Content($"{total} Unfav!")
        },
        Destination = new Destination(new List<string> { Environment.GetEnvironmentVariable("EMAIL_TARGET") ?? string.Empty }),
        Source = "support@berviantoleo.my.id"
    };
    var response = await emailClient.SendEmailAsync(request);
    string messageId = $"Message id: {response.MessageId}";
    Console.WriteLine(messageId);
}
