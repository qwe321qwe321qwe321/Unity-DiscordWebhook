# Unity-DiscordWebhook


Unity-DiscordWebhook is a library that allows you to easily send messages and files to Discord channels using webhooks. It provides a simple API for sending text messages, capturing and attaching screenshots, and even compressing and attaching log files for bug reports.

- [Setup](#setup)
	- [Requirement](#requirement)
	- [Installation](#installation)
- [Getting Started](#getting-started)
 
## Setup
### Requirement 
* [Cysharp/UniTask](https://github.com/Cysharp/UniTask)


### Installation
Install via package manager.

```
https://github.com/qwe321qwe321qwe321/Unity-DiscordWebhook.git?path=src/DiscordWebhook/Assets/DiscordWebhook
```

## Getting Started
DiscordWebhookSample.cs
```cs
public class DiscordWebhookSample : MonoBehaviour {
	private void OnGUI() {
		if (GUILayout.Button("Say hello world!")) {
			DiscordWebhooks.TextChannel1
				.SetContent("Hello WORLD")
				.ExecuteAsync().Forget();
		}
		if (GUILayout.Button("Screenshot!")) {
			DiscordWebhooks.TextChannel1
				.SetContent("Look!")
				.SetCaptureScreenshot(true) // capture screenshot and attach it.
				.ExecuteAsync()
				.Forget(); // Forget() is used to ignore the async operation in UniTask.
		}
		
		if (GUILayout.Button("Bug report!")) {
			string markdownContent = "# Bug report from user\nHere is the description.\n* 1\n* 2\n* 3";
			DiscordWebhooks.Forum1
				.SetThreadName("TITLE")
				.SetContent(markdownContent)
				.SetCaptureScreenshot(true) // capture screenshot and attach it.
				.AddFile(Application.consoleLogPath)
				.SetCompressAllFilesToZip(true, "LogFiles") // compress the log file to zip named "LogFiles.zip"
				.ExecuteAsync()
				.Forget(); // Forget() is used to ignore the async operation in UniTask.
		}
	}
}

public static class DiscordWebhooks {
	public static WebhookBuilder TextChannel1 => WebhookBuilder.CreateTextChannel("your_webhook_url_here");
	public static WebhookBuilder Forum1 => WebhookBuilder.CreateForum("your_webhook_url_here");
} 
```

| Text Channel | Forum |
|--|--|
| ![image](https://github.com/qwe321qwe321qwe321/Unity-DiscordWebhook/assets/23000374/613f729b-f738-48da-a37f-c5729cbe37f0)| ![image](https://github.com/qwe321qwe321qwe321/Unity-DiscordWebhook/assets/23000374/4b0a1b76-1059-4885-b19b-6409e4aecb29)|


