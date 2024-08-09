using UnityEngine;

namespace DiscordWebhook.Samples {
    public abstract class WebhookSample : MonoBehaviour {
        // There must be a better way to deal with these secrets, but for the sake of simplicity, I'll just put it here.
        public string textChannelWebhookUrl = "your_webhook_url_here";
        public string forumWebhookUrl = "your_webhook_url_here";
        public string serverId = "not_necessary_server_id";

        public ulong[] forumTagIds = new[] {
            123456789012345678UL, 
            123132132132132132UL
        };

        protected class WebhookSampleFormPayload {
            public string title = "title";
            public string content = "content";
            public string username = "username";
        }
        protected  WebhookSampleFormPayload m_Payload = new WebhookSampleFormPayload();

        protected void OnGUI() {
            // The user interface to demonstrate the full functionality of the DiscordWebhook is available with UniTask.
            // Background window with scroll view.
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginVertical("box");
            if (GUILayout.Button("Say hello world!")) {
                 SayHelloWorldToTextChannel();
            }
            if (GUILayout.Button("Screenshot!")) {
                SendScreenshotToTextChannel();
            }
			
            // Username, Title, content field with labels, and buttons to send the message.
            GUILayout.Label("Username");
            m_Payload.username = GUILayout.TextField(m_Payload.username);
            GUILayout.Label("Title");
            m_Payload.title = GUILayout.TextField(m_Payload.title);
            GUILayout.Label("Content");
            m_Payload.content = GUILayout.TextArea(m_Payload.content);

            if (GUILayout.Button("Send To Text Channel!")) {
                SendPayloadToTextChannel();
            }
            
            if (GUILayout.Button("Create a post and reply it!")) {
                SendPayloadToForumAndReply();
            }

            if (GUILayout.Button("Create a Bug report post!")) {
                CreateBugReportToForum();
            }
			
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        protected abstract void SayHelloWorldToTextChannel();
        protected abstract void SendScreenshotToTextChannel();
        protected abstract void SendPayloadToTextChannel();
        protected abstract void SendPayloadToForumAndReply();
        protected abstract void CreateBugReportToForum();
    }
}