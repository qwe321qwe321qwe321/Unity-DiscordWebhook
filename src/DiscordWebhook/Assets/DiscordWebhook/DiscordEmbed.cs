using System.Collections.Generic;
using UnityEngine;

namespace DiscordWebhook {
	public struct DiscordEmbed {
		public string title;
		public string description;
		public string url;
		public Color? color;
		public DiscordEmbedFooter? footer;
		public DiscordEmbedImage? image;
		public DiscordEmbedThumbnail? thumbnail;
		public DiscordEmbedAuthor? author;
		public DiscordEmbedField[] fields;

		internal Dictionary<string, object> ToJsonObject() {
			var json = new Dictionary<string, object>();

			if (string.IsNullOrWhiteSpace(title) == false) {
				json.Add("title", title);
			}

			if (string.IsNullOrWhiteSpace(description) == false) {
				json.Add("description", description);
			}

			if (string.IsNullOrWhiteSpace(url) == false) {
				json.Add("url", url);
			}

			if (color.HasValue) {
				json.Add("color", ColorUtility.ToDiscordColor(color.Value));
			}

			if (footer.HasValue) {
				var footerJson = footer.Value.ToJsonObject();
				if (footerJson != null) {
					json.Add("footer", footerJson);
				}
			}

			if (image.HasValue) {
				var imageJson = image.Value.ToJsonObject();
				if (imageJson != null) {
					json.Add("image", imageJson);
				}
			}

			if (thumbnail.HasValue) {
				var thumbnailJson = thumbnail.Value.ToJsonObject();
				if (thumbnailJson != null) {
					json.Add("thumbnail", thumbnailJson);
				}
			}

			if (author.HasValue) {
				var authorJson = author.Value.ToJsonObject();
				if (authorJson != null) {
					json.Add("author", authorJson);
				}
			}

			if (fields != null && fields.Length > 0) {
				var fieldJson = new List<object>();
				foreach (var field in fields) {
					var item = field.ToJsonObject();
					if (item != null) {
						fieldJson.Add(item);
					}
				}

				if (fieldJson.Count > 0) {
					json.Add("fields", fieldJson);
				}
			}

			return json.Count > 0 ? json : null;
		}
		
		static class ColorUtility {
			public static int ToDiscordColor(Color color) {
				Color32 color32 = color;
				return (color32.r << 16) | (color32.g << 8) | color32.b;
			}
		}
	}

	public struct DiscordEmbedField {
		public string name;
		public string value;
		public bool inline;

		internal Dictionary<string, object> ToJsonObject() {
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value)) {
				return null;
			}

			return new Dictionary<string, object> {
				{ "name", name },
				{ "value", value },
				{ "inline", inline }
			};
		}
	}

	public struct DiscordEmbedFooter {
		public string text;
		public string icon_url;

		internal Dictionary<string, object> ToJsonObject() {
			if (string.IsNullOrWhiteSpace(text)) {
				return null;
			}

			var json = new Dictionary<string, object> {
				{ "text", text }
			};

			if (string.IsNullOrWhiteSpace(icon_url) == false) {
				json.Add("icon_url", icon_url);
			}

			return json;
		}
	}

	public struct DiscordEmbedImage {
		public string url;

		internal Dictionary<string, object> ToJsonObject() {
			if (string.IsNullOrWhiteSpace(url)) {
				return null;
			}

			return new Dictionary<string, object> {
				{ "url", url }
			};
		}
	}

	public struct DiscordEmbedThumbnail {
		public string url;

		internal Dictionary<string, object> ToJsonObject() {
			if (string.IsNullOrWhiteSpace(url)) {
				return null;
			}

			return new Dictionary<string, object> {
				{ "url", url }
			};
		}
	}

	public struct DiscordEmbedAuthor {
		public string name;
		public string url;
		public string icon_url;

		internal Dictionary<string, object> ToJsonObject() {
			if (string.IsNullOrWhiteSpace(name)) {
				return null;
			}

			var json = new Dictionary<string, object> {
				{ "name", name }
			};

			if (string.IsNullOrWhiteSpace(url) == false) {
				json.Add("url", url);
			}

			if (string.IsNullOrWhiteSpace(icon_url) == false) {
				json.Add("icon_url", icon_url);
			}

			return json;
		}
	}
}
