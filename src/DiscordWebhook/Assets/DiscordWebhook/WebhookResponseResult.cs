using UnityEngine;
using Snowflake = System.UInt64;

namespace DiscordWebhook {
    public struct WebhookResponseResult {
        public bool isSuccess;
        public string errorMessage;
        public ResponseObject? response;
        
        public static WebhookResponseResult Failure(string error) {
            return new WebhookResponseResult() {
                isSuccess = false,
                errorMessage = error,
            };
        }
        
        public static WebhookResponseResult Success(string responseBody) {
            ResponseObject? response = null;
            if (!string.IsNullOrWhiteSpace(responseBody)) {
                try {
                    response = JsonUtility.FromJson<ResponseObject>(responseBody);
                } catch {
                    response = null;
                }
            }

            return new WebhookResponseResult() {
                isSuccess = true,
                errorMessage = null,
                response = response,
            };
        }

        public override string ToString() {
            return isSuccess ? $"Success: {response}" : $"Failure: {errorMessage}";
        }
    }
    
    public static class WebhookResponseExtensions {
        /// <summary>
        /// Get the message URL from the response. Return null if response doesn't have a valid response object.
        /// Need serverId to construct the URL.
        /// </summary>
        public static string GetMessageURL(this WebhookResponseResult responseResult, string serverId) {
            if (responseResult.HasResponse()) {
                return $"https://discord.com/channels/{serverId}/{responseResult.response.Value.channel_id}/{responseResult.response.Value.id}";
            }
            
            // Not able to get message URL if response is null or failed.
            return null;
        }
        
        /// <summary>
        /// Check if the response has a valid response object.
        /// </summary>
        public static bool HasResponse(this WebhookResponseResult responseResult) {
            return responseResult.isSuccess && responseResult.response.HasValue;
        }
    }
    
    [System.Serializable]
    public struct ResponseObject {
        public int type;
        public Snowflake channel_id;
        public string content;
        //public object[] mentions;
        //public object[] mention_roles;
        public Attachment[] attachments;
        //public object[] embeds;
        public string timestamp;
        //public string edited_timestamp;
        public int flags;
        //public object[] components;
        public Snowflake id;
        public Author author;
        public bool pinned;
        public bool mention_everyone;
        public bool tts;
        public Snowflake webhook_id;

        public override string ToString() {
            return $"(ResponseObject) {JsonUtility.ToJson(this)}";
        }
    }

    /// <summary>
    /// https://discord.com/developers/docs/resources/channel#attachment-object
    /// </summary>
    [System.Serializable]
    public struct Attachment {
        public Snowflake id;
        public string filename;
        public int size;
        public string url;
        public string proxy_url;
        public int width;
        public int height;
        public string content_type;
        public string placeholder;
        public int placeholder_version;
        
        public override string ToString() {
            return $"(Attachment) {JsonUtility.ToJson(this)}";
        }
    }

    [System.Serializable]
    public struct Author {
        public Snowflake id;
        public string username;
        //public object avatar;
        //public object discriminator;
        public int public_flags;
        public int flags;
        public bool bot;
        //public object global_name;
        //public object clan;
        
        public override string ToString() {
            return $"(Author) {JsonUtility.ToJson(this)}";
        }
    }
    
    [System.Serializable]
    public struct ChannelObject {
        public Snowflake id;
        public int type;
        public Snowflake last_message_id;
        public int flags;
        public Snowflake guild_id;
        public string name;
        public Snowflake parent_id;
        public int rate_limit_per_user;
        //public object topic;
        public int position;
        //public object[] permission_overwrites;
        public bool nsfw;
        public TagObject[] available_tags;
        //public object default_reaction_emoji;
        //public object default_sort_order;
        public int default_forum_layout;
        //public object icon_emoji;
        //public object theme_color;
        public string template;
        
        public override string ToString() {
            return $"(ChannelObject) {JsonUtility.ToJson(this)}";
        }
    }

    [System.Serializable]
    public struct TagObject {
        public Snowflake id;
        public string name;
        public bool moderated;
        //public object emoji_id;
        //public object emoji_name;
        
        public override string ToString() {
            return $"(TagObject) {JsonUtility.ToJson(this)}";
        }
    }
}