using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DiscordWebhook.Samples {
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
					.SetContent(markdownContent)
					.SetCaptureScreenshot(true) // capture screenshot and attach it.
					.AddFile(Application.consoleLogPath)
					.SetCompressAllFilesToZip(true) // compress the log file to zip.
					.ExecuteAsync()
					.Forget(); // Forget() is used to ignore the async operation in UniTask.
			}
		}
	}

	public static class DiscordWebhooks {
		public static WebhookBuilder TextChannel1 => WebhookBuilder.CreateTextChannel("your_webhook_url_here");
		public static WebhookBuilder Forum1 => WebhookBuilder.CreateForum("your_webhook_url_here");
	} 
}