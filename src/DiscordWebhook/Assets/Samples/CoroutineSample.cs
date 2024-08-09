using System.Text;
using UnityEngine;

namespace DiscordWebhook.Samples {
	public class CoroutineSample : WebhookSample {

		protected override void SayHelloWorldToTextChannel() {
			WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
				.SetContent("Hello WORLD")
				.ExecuteCoroutine(this, null);
		}

		protected override void SendScreenshotToTextChannel() {
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

		protected override void SendPayloadToTextChannel() {
			WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
				.SetUsername(m_Payload.username)
				.SetContent(m_Payload.content)
				.ExecuteCoroutine(this, null);
		}

		protected override void SendPayloadToForumAndReply() {
			WebhookBuilder.CreateForum(forumWebhookUrl)
				.SetUsername(m_Payload.username)
				.SetThreadName(m_Payload.title)
				.SetContent(m_Payload.content)
				.ExecuteCoroutine(this, OnComplete);
			
			void OnComplete(WebhookResponseResult result) {
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
							.ExecuteCoroutine(this, null);
					}
				}
			}
		}

		protected override void CreateBugReportToForum() {
			string markdownContent = "# Bug report from user\nHere is the description.\n" + SystemInfoHelper.GetSystemInfoInMarkdownList();
			WebhookBuilder.CreateForum(forumWebhookUrl)
				.SetUsername(m_Payload.username)
				.SetThreadName(m_Payload.title)
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
	}
}