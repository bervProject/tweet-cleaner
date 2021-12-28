# Tweet Cleaner

Tweet Cleaner, remove your last like and tweet.

## Structure

* [TweetCleaner](./TweetCleaner) - Clean up my tweet and un-fav tweets
* [CleanerReport](./CleanerReport) - Report job result weekly

## Setup

You need some environment variables:

### TweetCleaner

Using Twitter API & Mongo DB Serverless

```env
ACCESS_TOKEN=
CONSUMER_SECRET=
CONSUMER_KEY=
ACCESS_SECRET=
MONGO_CONNECTION_STRING=
MONGO_DB_NAME=
```

### CleanerReport

Using AWS S3 & Mongo DB Serverless

```env
MONGO_CONNECTION_STRING=
MONGO_DB_NAME=
AWS_ACCESS_KEY_ID=
AWS_SECRET_ACCESS_KEY=
AWS_DEFAULT_REGION=
EMAIL_TARGET=
```

## LICENSE

Apache 2.0

```markdown
 Copyright 2021 Bervianto Leo Pratama

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
```