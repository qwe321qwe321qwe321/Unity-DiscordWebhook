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
[DiscordWebhookSample.cs](/src/DiscordWebhook/Assets/Samples/DiscordWebhookSample.cs)
```cs
public class DiscordWebhookSample : MonoBehaviour {
	private void OnGUI() {
		if (GUILayout.Button("Say hello world!")) {
			DiscordWebhooks.TextChannel1
				.SetContent("Hello WORLD")
				.ExecuteAsync()
				.Forget(); // Forget() is a helper method to ignore the result in UniTask.
		}
		if (GUILayout.Button("Screenshot!")) {
			SendScreenshotToChannel();
		}
		
		if (GUILayout.Button("Bug report!")) {
			CreateBugReportTheadToForum();
		}
	}

	private async void SendScreenshotToChannel() {
		WebhookResponseResult result = await DiscordWebhooks.TextChannel1
			.SetContent("Look!")
			.SetCaptureScreenshot(true) // capture screenshot and attach it.
			.DisableLogging(true) // We handle the logging by ourselves in this case.
			.ExecuteAsync();

		if (result.isSuccess) {
			Debug.Log($"Success! {result.response}");
			Debug.Log(result.GetMessageURL(DiscordWebhooks.ServerId));
			Application.OpenURL(result.GetMessageURL(DiscordWebhooks.ServerId));
		} else {
			Debug.LogError(result.errorMessage);
		}
	}
	
	private async void CreateBugReportTheadToForum() {
		string markdownContent = "# Bug report from user\nHere is the description.\n" + SystemInfoHelper.GetSystemInfoInMarkdownList();
		WebhookResponseResult result = await DiscordWebhooks.Forum1
			.SetThreadName("TITLE")
			.SetContent(markdownContent)
			.SetCaptureScreenshot(true) // capture screenshot and attach it.
			.AddFile(Application.consoleLogPath) // add log file.
			.AddFile("systemInfo.txt", Encoding.UTF8.GetBytes(SystemInfoHelper.GetSystemInfoInMarkdownList())) // add system info.
			.SetCompressAllFilesToZip(true, "LogFiles") // compress the all files to a zip named "LogFiles.zip"
			.ExecuteAsync();
		
		if (result.isSuccess) {
			Debug.Log($"Success! {result.response}");
			Debug.Log(result.GetMessageURL(DiscordWebhooks.ServerId));
			Application.OpenURL(result.GetMessageURL(DiscordWebhooks.ServerId));
		}
	}
}

// There must be a better way to deal with these secrets, but for the sake of simplicity, I'll just put it here.
public static class DiscordWebhooks {
	public static WebhookBuilder TextChannel1 => WebhookBuilder.CreateTextChannel("your_webhook_url_here");
	public static WebhookBuilder Forum1 => WebhookBuilder.CreateForum("your_webhook_url_here");
	public const string ServerId = "not_neccessary_server_id";
} 
```

| Text Channel | Forum |
|--|--|
| ![image](https://github.com/qwe321qwe321qwe321/Unity-DiscordWebhook/assets/23000374/613f729b-f738-48da-a37f-c5729cbe37f0)| ![image](https://github.com/qwe321qwe321qwe321/Unity-DiscordWebhook/assets/23000374/4b0a1b76-1059-4885-b19b-6409e4aecb29)|


