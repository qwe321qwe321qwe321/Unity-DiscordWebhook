using DiscordWebhook;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorCoroutineSample : EditorWindow
{
#if DISCORD_WEBHOOK_EDITOR_COROUTINE_SUPPORT
	[MenuItem("Samples/Discord Webhook/EditorCoroutineSample")]
    private static void OpenWindow() {
        GetWindow<EditorCoroutineSample>();
    }
    
	public string textChannelWebhookUrl = "your_webhook_url_here";
	public string username = "username";
	public string title = "title";
	public string content = "content";

	private void OnGUI() {
		// text field for textChannelWebhookUrl, username, content
		textChannelWebhookUrl = EditorGUILayout.TextField("Text Channel Webhook URL", textChannelWebhookUrl);
		username = EditorGUILayout.TextField("Username", username);
		title = EditorGUILayout.TextField("Title", title);
		content = EditorGUILayout.TextField("Content", content);
		
		if (GUILayout.Button("Send Content")) {
			WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
				.SetUsername(username)
				.SetContent(content)
				.ExecuteEditorCoroutine(
					result => {
						Debug.Log($"Success={result.isSuccess}, response={result.response}");
					});
		}

		if (GUILayout.Button("Send Embed")) {
			WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
				.SetUsername(username)
				.AddEmbed(new DiscordEmbed {
					title = title,
					description = content,
					color = DiscordColorPreset.Yellow,
					fields = new[] {
						new DiscordEmbedField {
							name = "Username",
							value = username,
							inline = true
						}
					}
				})
				.ExecuteEditorCoroutine(
					result => {
						Debug.Log($"Success={result.isSuccess}, response={result.response}");
					});
		}
		
	}
#endif
}
