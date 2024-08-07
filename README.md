# Unity-DiscordWebhook


Unity-DiscordWebhook is a library that allows you to easily send messages and files to Discord channels using [webhooks](https://discord.com/developers/docs/resources/webhook). It provides a simple API for sending text messages, capturing and attaching screenshots, and even compressing and attaching log files for bug reports.

## Features
* [Sending messages and create posts/threads to text channels and forums.](#getting-started)
* [Builder pattern for creating a message.](#getting-started)
* Awaitable API with [UniTask](https://github.com/Cysharp/UniTask)
* Unity Coroutine and EditorCoroutine API. (IEnumerator and callback)
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
- [Use Cases](#use-cases)
 
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

[EditorCoroutineSample.cs](/src/DiscordWebhook/Assets/Samples/Editor/EditorCoroutineSample.cs)
```csharp
WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
	.SetUsername("MyBot") // username is optional.
	.SetContent("Hello WORLD") // content is required.
	.ExecuteEditorCoroutine((result) => { // No need a MonoBehaviour to run.
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
1. Create a Discord Bot and invite it to your server. (See [Discord Developer Portal: Getting Started](https://discord.com/developers/docs/quick-start/getting-started))
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

## Use Cases
***CAVEAT: You should never directly expose your webhook token in a public product, as this poses a risk of allowing anyone to send any message through the webhook. The use cases here are either a limited scope of confidential users or implementing thier own backends to prevent from exposing the token to clients.***

> If you want your use case added to or removed from this list just open an [issue](https://github.com/qwe321qwe321qwe321/qwe321qwe321qwe321.github.io/issues).

### [Bionic Bay](https://store.steampowered.com/app/1928690/Bionic_Bay/)
Bug reporter for the **in-house** level editor.

| In-game UI | Discord |
| ----------- | --------|
| ![bb-ui](https://github.com/user-attachments/assets/351b2051-d0a1-4394-ac8d-7b621640262b) | ![bb-dc](https://github.com/user-attachments/assets/71f4e41b-a4fe-4a6a-83ee-362dbd53c603) |

### [Minds Beneath Us](https://store.steampowered.com/app/1610440/Minds_Beneath_Us/)
In-game bug reporter for beta test.

| In-game UI | Discord |
| ----------- | --------|
| <video src="https://github.com/user-attachments/assets/3b589bbe-ca01-4d02-8418-27f3df014d73"/> | ![mbu-dc](https://github.com/user-attachments/assets/7a4041cc-60e6-47bd-b6ac-46ccc29ca953) |


### [Nine Sols](https://store.steampowered.com/app/1809540/_/)
In-game bug reporter with their own backend.

![image](https://github.com/user-attachments/assets/a1a938de-0e39-433b-b410-d09254f86bbc)

### [Autopanic Zero](https://store.steampowered.com/app/1423670/_/)
Patch notes informer in Unity Editor.

![image](https://github.com/user-attachments/assets/fc628088-06b1-42cc-801a-d02cb8922f39)

---

I actually wrote an article that introduces the implementation of bug reports using Discord webhooks, covering its advantages and caveats, but [it's in Chinese.](https://qwe321qwe321qwe321.github.io/posts/13673/).
