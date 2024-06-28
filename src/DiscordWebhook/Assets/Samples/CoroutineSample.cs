using System.Text;
using UnityEngine;

namespace DiscordWebhook.Samples {
	public class CoroutineSample : MonoBehaviour {
		// There must be a better way to deal with these secrets, but for the sake of simplicity, I'll just put it here.
		public string textChannelWebhookUrl = "your_webhook_url_here";
		public string forumWebhookUrl = "your_webhook_url_here";
		public string serverId = "not_necessary_server_id";
		
		private void OnGUI() {
			if (GUILayout.Button("Say hello world!")) {
				WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
					.SetContent("Hello WORLD")
					.ExecuteCoroutine(this, null);
			}
			if (GUILayout.Button("Screenshot!")) {
				SendScreenshotToChannel();
			}
			
			if (GUILayout.Button("Bug report!")) {
				CreateBugReportTheadToForum();
			}
		}
		

		private void SendScreenshotToChannel() {
			WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
				.SetContent("Look!")
				.SetCaptureScreenshot(true) // capture screenshot and attach it.
				.DisableLogging(true) // We handle the logging by ourselves in this case.
				.ExecuteCoroutine(this, OnComplete);

			void OnComplete(WebhookResponseResult result) {
				if (result.isSuccess) {
					Debug.Log($"Success! {result.response}");
					
					string url = result.GetMessageURL(serverId);
					Debug.Log(url);
					if (url != null) {
						Application.OpenURL(url);
					}
				} else {
					Debug.LogError(result.errorMessage);
				}
			}
		}
		
		private void CreateBugReportTheadToForum() {
			string markdownContent = "# Bug report from user\nHere is the description.\n" + SystemInfoHelper.GetSystemInfoInMarkdownList();
			WebhookBuilder.CreateForum(forumWebhookUrl)
				.SetThreadName("TITLE")
				.SetContent(markdownContent)
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
	}
}