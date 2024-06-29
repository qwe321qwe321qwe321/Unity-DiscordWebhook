# Unity-DiscordWebhook


Unity-DiscordWebhook is a library that allows you to easily send messages and files to Discord channels using [webhooks](https://discord.com/developers/docs/resources/webhook). It provides a simple API for sending text messages, capturing and attaching screenshots, and even compressing and attaching log files for bug reports.

## Features
* [Sending messages and create posts/threads to text channels and forums.](#getting-started)
* [Builder pattern for creating a message.](#getting-started)
* Awaitable API with [UniTask](https://github.com/Cysharp/UniTask)
* Unity Coroutine API. (IEnumerator and callback)
* A few Discord Bot API to query the server and channel information. 
  * See [DiscordBotApi.cs](/src/DiscordWebhook/Assets/DiscordWebhook/Utility/DiscordBotApi.cs).
* Helper methods for creating a bug report.
  * [Attach files by file path or byte array.](#create-a-post-in-a-forum-with-screenshots-and-log-files)
  * [Attach images by Texture2D.](/src/DiscordWebhook/Assets/DiscordWebhook/WebhookBuilder.cs#L120)
  * [Capture and attach screenshots by setting a flag.](#create-a-post-in-a-forum-with-screenshots-and-log-files)
  * [Compress all attached files to a zip file by setting a flag.](#create-a-post-in-a-forum-with-screenshots-and-log-files)
  * [Get the message URL after sending a message.](#create-a-post-in-a-forum-with-screenshots-and-log-files)

## Table of Contents
- [Setup](#setup)
  - [Requirement](#requirement)
  - [Installation](#installation)
- [Getting Started](#getting-started)
  - [Send a message to a text channel](#send-a-message-to-a-text-channel)
  - [Create a post in a forum with screenshots and log files](#create-a-post-in-a-forum-with-screenshots-and-log-files)
  - [How to get the forum tag IDs](#how-to-get-the-forum-tag-ids)
 
## Setup
### Requirement 
* (Optional) [Cysharp/UniTask](https://github.com/Cysharp/UniTask)
	* We recommend using UniTask for asynchronous programming. However, you can use the library without it, we provide Unity Coroutine API as well.
	* If your UniTask is not installed from [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html), add the scripting define symbol `DISCORD_WEBHOOK_UNITASK_SUPPORT` to Project Settings.

### Installation
Install via [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html).

```
https://github.com/qwe321qwe321qwe321/Unity-DiscordWebhook.git?path=src/DiscordWebhook/Assets/DiscordWebhook
```

## Getting Started

### Send a message to a text channel
[UniTaskSample.cs](/src/DiscordWebhook/Assets/Samples/UniTaskSample.cs)
```csharp
WebhookResponseResult result = await WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
	.SetUsername("MyBot") // username is optional.
	.SetContent("Hello WORLD") // content is required.
	.ExecuteAsync();
// handle the result.
```

[CoroutineSample.cs](/src/DiscordWebhook/Assets/Samples/CoroutineSample.cs)
```csharp
WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
	.SetUsername("MyBot") // username is optional.
	.SetContent("Hello WORLD") // content is required.
	.ExecuteCoroutine(this, (result) => {
    	// handle the result with callback.
	}));
```

### Create a post in a forum with screenshots and log files
[UniTaskSample.cs](/src/DiscordWebhook/Assets/Samples/UniTaskSample.cs)
```csharp
private async UniTaskVoid CreateBugReportTheadToForum() {
	string markdownContent = "# Bug report from user\nHere is the description.\n" + SystemInfoHelper.GetSystemInfoInMarkdownList();
	WebhookResponseResult result = await WebhookBuilder.CreateForum(forumWebhookUrl)
		.SetThreadName("TITLE")
		.SetContent(markdownContent)
		.AddTags(forumTagIds) // Add tags to the thread. (You have to get the tag IDs by DiscordBotApi upfront.)
		.SetCaptureScreenshot(true) // capture screenshot and attach it.
		.AddFile(Application.consoleLogPath) // add log file.
		.AddFile("systemInfo.txt", Encoding.UTF8.GetBytes(SystemInfoHelper.GetSystemInfoInMarkdownList())) // add system info.
		.SetCompressAllFilesToZip(true, "LogFiles") // compress the all files to a zip named "LogFiles.zip"
		.ExecuteAsync();
	
	if (result.isSuccess) {
		Debug.Log($"Success! {result.response}");
		
		string url = result.GetMessageURL(serverId);
		Debug.Log(url);
		if (url != null) {
			// Directly open the message in the browser.
			Application.OpenURL(url);
		}
	}
}
```

[CoroutineSample.cs](/src/DiscordWebhook/Assets/Samples/CoroutineSample.cs)
```csharp
private void CreateBugReportTheadToForum() {
	string markdownContent = "# Bug report from user\nHere is the description.\n" + SystemInfoHelper.GetSystemInfoInMarkdownList();
	WebhookBuilder.CreateForum(forumWebhookUrl)
		.SetThreadName("TITLE")
		.SetContent(markdownContent)
		.AddTags(forumTagIds) // Add tags to the thread. (You have to get the tag IDs by DiscordBotApi upfront.)
		.SetCaptureScreenshot(true) // capture screenshot and attach it.
		.AddFile(Application.consoleLogPath) // add log file.
		.AddFile("systemInfo.txt", Encoding.UTF8.GetBytes(SystemInfoHelper.GetSystemInfoInMarkdownList())) // add system info.
		.SetCompressAllFilesToZip(true, "LogFiles") // compress the all files to a zip named "LogFiles.zip"
		.ExecuteCoroutine(this, OnComplete);
	
	void OnComplete(WebhookResponseResult result) {
		if (result.isSuccess) {
			Debug.Log($"Success! {result.response}");
			
			string url = result.GetMessageURL(serverId);
			Debug.Log(url);
			if (url != null) {
				Application.OpenURL(url);
			}
		}
	}
}
```

### How to get the forum tag IDs
1. Create a Discord Bot and invite it to your server. (See [Discord Developer Poral: Getting Started](https://discord.com/developers/docs/quick-start/getting-started))
2. Get your bot token.
3. Get your forum channel ID.

Then you can dump the forum tag IDs by using the [DiscordBotApi](/src/DiscordWebhook/Assets/DiscordWebhook/Utility/DiscordBotApi.cs) we provide.
```csharp
var tags = await DiscordBotApi.GetForumAvailableTags("bot_token", "forum_channel_id");
if (tags != null) {
    foreach (var tagObject in tags) {
        Debug.Log($"Tag: {tagObject}");
    }
} else {
    Debug.LogError("Failed to get tags.");
}
```

## Screenshots
| Text Channel | Forum |
|--|--|
| ![image](https://github.com/qwe321qwe321qwe321/Unity-DiscordWebhook/assets/23000374/613f729b-f738-48da-a37f-c5729cbe37f0)| ![image](https://github.com/qwe321qwe321qwe321/Unity-DiscordWebhook/assets/23000374/4b0a1b76-1059-4885-b19b-6409e4aecb29)|

