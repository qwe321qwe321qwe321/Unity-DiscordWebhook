using UnityEngine;
#if DISCORD_WEBHOOK_UNITASK_SUPPORT
using System.Text;
using Cysharp.Threading.Tasks;
#else
#if UNITY_EDITOR
using UnityEditor;
#endif
#endif

namespace DiscordWebhook.Samples {
#if DISCORD_WEBHOOK_UNITASK_SUPPORT
	public class UniTaskSample : WebhookSample {
		protected override void SayHelloWorldToTextChannel() {
			WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
				.SetContent("Hello WORLD")
				.ExecuteAsync()
				.Forget(); // Forget() is a helper method to ignore the result in UniTask.
		}

		protected override void SendScreenshotToTextChannel() {
			SendScreenshotToChannel()
				.Forget();
		}
		
		protected override void SendPayloadToTextChannel() {
			WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
				.SetUsername(m_Payload.username)
				.SetContent(m_Payload.content)
				.ExecuteAsync()
				.Forget();
		}

		protected override void SendPayloadToForumAndReply() {
			CreateThreadToForumAndReplyIt(m_Payload.username, m_Payload.title, m_Payload.content)
				.Forget();
		}

		protected override void CreateBugReportToForum() {
			CreateBugReportTheadToForum(m_Payload.username, m_Payload.title)
				.Forget();
		}

		private async UniTaskVoid SendScreenshotToChannel() {
			WebhookResponseResult result = await WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
				.SetContent("Look!")
				.SetCaptureScreenshot(true) // capture screenshot and attach it.
				.DisableLogging(true) // We handle the logging by ourselves in this case.
				.ExecuteAsync();

			if (result.isSuccess) {
				Debug.Log($"Success! {result.response}");
				if (result.response.HasValue) {
					Debug.Log(result.response.Value.id.DumpSnowflake());
					Debug.Log(result.response.Value.channel_id.DumpSnowflake());
					Debug.Log(result.response.Value.webhook_id.DumpSnowflake());
				}
				
				string url = result.GetMessageURL(serverId);
				Debug.Log(url);
				if (url != null) {
					Application.OpenURL(url);
				}
			} else {
				Debug.LogError(result.errorMessage);
			}
		}
		
		private async UniTaskVoid CreateBugReportTheadToForum(string username, string title) {
			string markdownContent = "# Bug report from user\nHere is the description.\n" + SystemInfoHelper.GetSystemInfoInMarkdownList();
			WebhookResponseResult result = await WebhookBuilder.CreateForum(forumWebhookUrl)
				.SetUsername(username)
				.SetThreadName(title)
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
					Application.OpenURL(url);
				}
			}
		}
		
		private async UniTaskVoid CreateThreadToForumAndReplyIt(string username, string title, string content) {
			WebhookResponseResult result = await WebhookBuilder.CreateForum(forumWebhookUrl)
				.SetUsername(username)
				.SetThreadName(title)
				.SetContent(content)
				.ExecuteAsync();
			
			if (result.isSuccess) {
				Debug.Log($"Success! {result.response}");
				string url = result.GetMessageURL(serverId);
				Debug.Log(url);
				
				if (result.response.HasValue) {
					var channelId = result.response.Value.channel_id;
					WebhookBuilder.CreateForum(forumWebhookUrl)
						.SetUsername("Reply Bot")
						.SetRepliedThreadId(channelId)
						.SetContent("Thank you for your report!")
						.ExecuteAsync()
						.Forget();
				}
			}
		}
#else
	public class UniTaskSample : MonoBehaviour {
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

