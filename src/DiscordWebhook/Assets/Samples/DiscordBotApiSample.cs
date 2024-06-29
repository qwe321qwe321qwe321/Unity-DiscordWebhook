using UnityEngine;

namespace DiscordWebhook.Samples {
    public class DiscordBotApiSample : MonoBehaviour {
        public string botToken;
        public string guildId;
        public string channelId;
        
        private async void Start() {
            if (string.IsNullOrWhiteSpace(botToken)) {
                Debug.LogWarning("Please set botToken.");
                return;
            }
            
            var channel  = await DiscordBotApi.GetChannel(botToken, channelId);
            if (channel.HasValue) {
                Debug.Log($"Channel: {channel.Value}");
            } else {
                Debug.LogError("Failed to get channel.");
            }
            
            var json = await DiscordBotApi.GetAllChannelsJson(botToken, guildId);
            Debug.Log($"All channels: {json}");
            
            var tags = await DiscordBotApi.GetForumAvailableTags(botToken, channelId);
            if (tags != null) {
                foreach (var tagObject in tags) {
                    Debug.Log($"Tag: {tagObject}");
                }
            } else {
                Debug.LogError("Failed to get tags.");
            }
        }
    }
}