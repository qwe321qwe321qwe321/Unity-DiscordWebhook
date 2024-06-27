using UnityEngine;

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
        public long channel_id;
        public string content;
        //public object[] mentions;
        //public object[] mention_roles;
        public Attachment[] attachments;
        //public object[] embeds;
        public string timestamp;
        //public string edited_timestamp;
        public int flags;
        //public object[] components;
        public long id;
        public Author author;
        public bool pinned;
        public bool mention_everyone;
        public bool tts;
        public long webhook_id;

        public override string ToString() {
            return $"(ResponseObject) {JsonUtility.ToJson(this)}";
        }
    }

    [System.Serializable]
    public struct Attachment {
        public long id;
        public string filename;
        public long size;
        public string url;
        public string proxy_url;
        public int width;
        public int height;
        public string content_type;
        public string placeholder;
        public int placeholder_version;
    }

    [System.Serializable]
    public struct Author {
        public long id;
        public string username;
        //public object avatar;
        //public object discriminator;
        public int public_flags;
        public int flags;
        public bool bot;
        //public object global_name;
        //public object clan;
    }
}