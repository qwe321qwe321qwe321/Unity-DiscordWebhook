using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DiscordWebhook.Samples {
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
}