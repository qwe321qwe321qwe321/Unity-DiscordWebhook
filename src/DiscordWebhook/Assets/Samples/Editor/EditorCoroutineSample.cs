using DiscordWebhook;
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
	public string content = "content";

	private void OnGUI() {
		// text field for textChannelWebhookUrl, username, content
		textChannelWebhookUrl = EditorGUILayout.TextField("Text Channel Webhook URL", textChannelWebhookUrl);
		username = EditorGUILayout.TextField("Username", username);
		content = EditorGUILayout.TextField("Content", content);
		
		// A button to submit.
		if (GUILayout.Button("Submit")) {
			WebhookBuilder.CreateTextChannel(textChannelWebhookUrl)
				.SetUsername(username)
				.SetContent(content)
				.ExecuteEditorCoroutine(
					result => {
						Debug.Log($"Success={result.isSuccess}, response={result.response}");
					});
		}
		
	}
#endif
}
