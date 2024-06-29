using System;
using UnityEngine;
using UnityEngine.Networking;

#if DISCORD_WEBHOOK_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif


namespace DiscordWebhook {
    public static class DiscordBotApi {
        private const string BASE_API_URL = "https://discord.com/api/v10/";
        private const string GUILD_API = BASE_API_URL + "guilds/"; 
        private const string CHANNEL_API = BASE_API_URL + "channels/";

        /// <summary>
        /// Get all available tags from the forum channel ID.
        /// </summary>
        
#if DISCORD_WEBHOOK_UNITASK_SUPPORT
        public static async UniTask<TagObject[]> GetForumAvailableTags(string botToken, string forumChannelId) {
#else
        public static async Task<TagObject[]> GetForumAvailableTags(string botToken, string forumChannelId) {
#endif
            
            var response = await GetChannel(botToken, forumChannelId);
            if (response == null) {
                return null;
            }

            return response.Value.available_tags;
        }

        /// <summary>
        /// Get the channel object from the channel ID.
        /// </summary>
        
#if DISCORD_WEBHOOK_UNITASK_SUPPORT
        public static async UniTask<ChannelObject?> GetChannel(string botToken, string channelId) {
#else
        public static async Task<ChannelObject?> GetChannel(string botToken, string channelId) {
#endif
            
            string json = await GetChannelJson(botToken, channelId);
            return ParseJson<ChannelObject>(json);
        }

        /// <summary>
        /// Get the channel in json from the channel ID.
        /// </summary>
#if DISCORD_WEBHOOK_UNITASK_SUPPORT
            public static UniTask<string> GetChannelJson(string botToken, string channelId) {
#else
            public static Task<string> GetChannelJson(string botToken, string channelId) {
#endif
            string url = CHANNEL_API + channelId;
            return GetRequestAsync(url, botToken);
        }
        
        /// <summary>
        /// Get all channels in json from the guild ID.
        /// </summary>
       
#if DISCORD_WEBHOOK_UNITASK_SUPPORT
        public static UniTask<string> GetAllChannelsJson(string botToken, string guildId) {
#else
        public static Task<string> GetAllChannelsJson(string botToken, string guildId) {
#endif
            
            string url = GUILD_API + guildId + "/channels";
            return GetRequestAsync(url, botToken);
        }

#if DISCORD_WEBHOOK_UNITASK_SUPPORT
        private static async UniTask<string> GetRequestAsync(string url, string botToken) {
#else
        private static async Task<string> GetRequestAsync(string url, string botToken) {
#endif
            
            // Use UnityWebRequest to get the data.
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("Authorization", $"Bot {botToken}");
#if DISCORD_WEBHOOK_UNITASK_SUPPORT
            await webRequest.SendWebRequest();
#else
            var asyncOp = webRequest.SendWebRequest();
            while (!asyncOp.isDone) {
                await Task.Yield();
            }
#endif
            if (webRequest.result != UnityWebRequest.Result.Success) {
                Debug.LogError($"Failed to get data from {url}: {webRequest.error}");
                return null;
            }
            
            return webRequest.downloadHandler.text; 
        }
        
        private static T? ParseJson<T>(string json) where T : struct {
            if (json == null) {
                return null;
            }
            try {
                return JsonUtility.FromJson<T>(json);
            } catch (Exception e) {
                Debug.LogError($"Failed to parse {typeof(T)} from {json}: {e}");
                return null;
            }
        }
    }
}