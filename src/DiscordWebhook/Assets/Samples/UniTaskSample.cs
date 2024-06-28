using UnityEditor;
using UnityEngine;
#if UNITASK_SUPPORT
using System.Text;
using Cysharp.Threading.Tasks;
#endif

namespace DiscordWebhook.Samples {
	public class UniTaskSample : MonoBehaviour {
		// There must be a better way to deal with these secrets, but for the sake of simplicity, I'll just put it here.
		public string textChannelWebhookUrl = "your_webhook_url_here";
		public string forumWebhookUrl = "your_webhook_url_here";
		public string serverId = "not_necessary_server_id";
		
#if UNITASK_SUPPORT
		private void OnGUI() {
			if (GUILayout.Button("Say hello world! (UniTask)")) {
				WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
					.SetContent("Hello WORLD")
					.ExecuteAsync()
					.Forget(); // Forget() is a helper method to ignore the result in UniTask.
			}
			if (GUILayout.Button("Screenshot! (UniTask)")) {
				SendScreenshotToChannel()
					.Forget();
			}
			
			if (GUILayout.Button("Bug report! (UniTask)")) {
				CreateBugReportTheadToForum()
					.Forget();
			}
		}
		

		private async UniTaskVoid SendScreenshotToChannel() {
			WebhookResponseResult result = await WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
				.SetContent("Look!")
				.SetCaptureScreenshot(true) // capture screenshot and attach it.
				.DisableLogging(true) // We handle the logging by ourselves in this case.
				.ExecuteAsync();

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
		
		private async UniTaskVoid CreateBugReportTheadToForum() {
			string markdownContent = "# Bug report from user\nHere is the description.\n" + SystemInfoHelper.GetSystemInfoInMarkdownList();
			WebhookResponseResult result = await WebhookBuilder.CreateForum(forumWebhookUrl)
				.SetThreadName("TITLE")
				.SetContent(markdownContent)
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
					Application.OpenURL(url);
				}
			}
		}
#else
		private void Start() {
			// Show a dialog to install UniTask if it's not installed.
			Debug.LogError("cysharp/UniTask is not installed. Please install it to run this sample. https://github.com/Cysharp/UniTask");
			if (EditorUtility.DisplayDialog("UniTask not found", "cysharp/UniTask is not found.\nDo you want to install UniTask via Package Manager?",
				    "Yes", "No")) {
				UnityEditor.PackageManager.Client.Add("https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask");
				UnityEditor.PackageManager.Client.Resolve();
				EditorApplication.delayCall += () => {
					Debug.Log("Installing UniTask, exit playmode.");
					EditorApplication.ExitPlaymode();

				};
			}
		}
		private void OnGUI() {
			for (int i = 0; i < 50; i++) {
				GUILayout.Label("cysharp/UniTask is not installed. Please install it to run this sample. https://github.com/Cysharp/UniTask");
			}
		}
#endif
	}
}

