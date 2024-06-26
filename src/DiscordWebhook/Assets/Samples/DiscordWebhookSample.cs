using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DiscordWebhook.Samples {
	public class DiscordWebhookSample : MonoBehaviour {
		private void OnGUI() {
			if (GUILayout.Button("Say hello world!")) {
				DiscordWebhooks.TextChannel1
					.SetContent("Hello WORLD")
					.SetErrorLogEnabled(true) // Log the error message since we don't catch the result.
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
			string markdownContent = "# Bug report from user\nHere is the description.\n* 1\n* 2\n* 3";
			WebhookResponseResult result = await DiscordWebhooks.Forum1
				.SetThreadName("TITLE")
				.SetContent(markdownContent)
				.SetCaptureScreenshot(true) // capture screenshot and attach it.
				.AddFile(Application.consoleLogPath)
				.SetCompressAllFilesToZip(true, "LogFiles") // compress the log file to zip named "LogFiles.zip"
				.ExecuteAsync();
			
			if (result.isSuccess) {
				Debug.Log($"Success! {result.response}");
				Debug.Log(result.GetMessageURL(DiscordWebhooks.ServerId));
				Application.OpenURL(result.GetMessageURL(DiscordWebhooks.ServerId));
			} else {
				Debug.LogError(result.errorMessage);
			}
		}
	}

	// There must be a better way to deal with these secrets, but for the sake of simplicity, I'll just put it here.
	public static class DiscordWebhooks {
		public static WebhookBuilder TextChannel1 => WebhookBuilder.CreateTextChannel("your_webhook_url_here");
		public static WebhookBuilder Forum1 => WebhookBuilder.CreateForum("your_webhook_url_here");
		public const string ServerId = "not_neccessary_server_id";
	} 
}