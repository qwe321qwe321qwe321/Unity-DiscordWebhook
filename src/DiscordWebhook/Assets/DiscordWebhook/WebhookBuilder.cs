using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace DiscordWebhook {
	public enum ChannelType {
		TextChannel,
		Forum
	}
	public struct WebhookBuilder {
		// required
		private ChannelType m_ChannelType;
		private string m_WebhookUrl;
		private string m_Content;
		private string m_ThreadName;

		// optional
		private string m_Username;
		private AdditionalFile m_AttachedImage;
		private List<AdditionalFile> m_AdditionalFiles;
		private bool m_CaptureScreenshot;
		private bool m_CompressAllFilesToZip;
		private string m_ZipFileName;

		/// <summary>
		/// Create a new WebhookBuilder for TextChannel.
		/// </summary>
		/// <param name="webhookUrl"></param>
		/// <returns></returns>
		public static WebhookBuilder CreateTextChannel(string webhookUrl) {
			return new WebhookBuilder()
				.SetChannelType(ChannelType.TextChannel)
				.SetUrl(webhookUrl);
		}
		
		/// <summary>
		/// Create a new WebhookBuilder for Forum.
		/// </summary>
		/// <param name="webhookUrl"></param>
		/// <returns></returns>
		public static WebhookBuilder CreateForum(string webhookUrl) {
			return new WebhookBuilder()
				.SetChannelType(ChannelType.Forum)
				.SetUrl(webhookUrl);
		}
		
		private WebhookBuilder SetChannelType(ChannelType channelType) {
			m_ChannelType = channelType;
			return this;
		}

		private WebhookBuilder SetUrl(string webhookUrl) {
			m_WebhookUrl = webhookUrl;
			return this;
		}

		/// <summary>
		/// [Required] Set the content of the message.
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public WebhookBuilder SetContent(string content) {
			m_Content = content;
			return this;
		}

		/// <summary>
		/// [Required for Forum] Set the thread name/title of the message.
		/// </summary>
		/// <param name="threadName"></param>
		/// <returns></returns>
		public WebhookBuilder SetThreadName(string threadName) {
			m_ThreadName = threadName;
			return this;
		}

		/// <summary>
		/// [Optional] Set the username of the message.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public WebhookBuilder SetUsername(string username) {
			m_Username = username;
			return this;
		}

		/// <summary>
		/// [Optional] Set the attached image of the message.
		/// </summary>
		/// <param name="texture2D"></param>
		/// <returns></returns>
		public WebhookBuilder SetAttachedImage(Texture2D texture2D) {
			m_AttachedImage = AdditionalFile.FromTexture(texture2D);
			return this;
		}
		
		/// <summary>
		/// [Optional] Add an additional file to the message from file path.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public WebhookBuilder AddFile(string filePath) {
			return AddFile(AdditionalFile.FromPath(filePath));
		}
		
		/// <summary>
		/// [Optional] Add an additional file to the message from Texture2D.
		/// </summary>
		/// <param name="texture2D"></param>
		/// <returns></returns>
		public WebhookBuilder AddFile(Texture2D texture2D) {
			return AddFile(AdditionalFile.FromTexture(texture2D));
		}
		
		/// <summary>
		/// [Optional] Add an additional file to the message from byte array.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public WebhookBuilder AddFile(string fileName, byte[] data) {
			return AddFile(AdditionalFile.FromBytes(fileName, data));
		}

		/// <summary>
		/// [Optional] Add an additional file to the message.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public WebhookBuilder AddFile(AdditionalFile file) {
			if (m_AdditionalFiles == null) {
				m_AdditionalFiles = new List<AdditionalFile>();
			}

			m_AdditionalFiles.Add(file);
			return this;
		}
		
		/// <summary>
		/// [Optional] Set whether to compress all additional files to a zip file.
		/// </summary>
		/// <param name="compressAllFilesToZip"></param>
		/// <returns></returns>
		public WebhookBuilder SetCompressAllFilesToZip(bool compressAllFilesToZip, string zipFileName = null) {
			m_CompressAllFilesToZip = compressAllFilesToZip;
			m_ZipFileName = zipFileName;
			return this;
		}

		/// <summary>
		/// [Optional] Set whether to capture a screenshot. If true, it will override the attached image.
		/// </summary>
		/// <param name="captureScreenshot"></param>
		/// <returns></returns>
		public WebhookBuilder SetCaptureScreenshot(bool captureScreenshot) {
			m_CaptureScreenshot = captureScreenshot;
			return this;
		}

		/// <summary>
		/// Execute the webhook.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public async UniTask<bool> ExecuteAsync() {
			// Required fields
			switch (m_ChannelType) {
				case ChannelType.TextChannel:
					if (string.IsNullOrEmpty(m_Content)) {
						Debug.LogError("Content is required for TextChannel webhook.");
						return false;
					}
					break;
				case ChannelType.Forum:
					if (string.IsNullOrEmpty(m_Content)) {
						Debug.LogError("Content is required for Forum webhook.");
						return false;
					}
					if (string.IsNullOrEmpty(m_ThreadName)) {
						Debug.LogError("ThreadName is required for Forum webhook.");
						return false;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			if (m_CaptureScreenshot) {
				// Capture screenshot will override attached image.
				var screenshot = await Util.CaptureScreenshot();
				if (screenshot) {
					m_AttachedImage = AdditionalFile.FromTexture(screenshot);
				}

				Object.DestroyImmediate(screenshot);
			}

			using UnityWebRequest www = UnityWebRequest.Post(m_WebhookUrl, BuildFormData());
			await www.SendWebRequest();

			if (www.result != UnityWebRequest.Result.Success) {
				Debug.LogError("Error sending webhook: " + www.error);
				return false;
			} else {
				Debug.Log($"Webhook sent successfully!\n{www.downloadHandler.text}");
				return true;
			}
		}

		private WWWForm BuildFormData() {
			WWWForm form = new();
			if (m_Username != null) {
				form.AddField("username", m_Username);
			}

			form.AddField("content", m_Content);

			if (m_ChannelType == ChannelType.Forum) {
				form.AddField("thread_name", m_ThreadName);
			}

			int fileIndex = 1;
			if (m_AttachedImage.IsValid) {
				AddFileToForm(m_AttachedImage);
			}

			if (m_AdditionalFiles != null && m_AdditionalFiles.Count > 0) {
				if (m_CompressAllFilesToZip) {
					string fileName = string.IsNullOrWhiteSpace(m_ZipFileName) ? "files.zip" : m_ZipFileName;
					if (!fileName.EndsWith(".zip")) {
						fileName += ".zip";
					}
					var zipFile = Util.CompressToZip(fileName, m_AdditionalFiles.ToArray());
					AddFileToForm(zipFile);
				} else {
					foreach (var file in m_AdditionalFiles) {
						if (!file.IsValid) {
							continue;
						}
						AddFileToForm(file);
					}
				}
			}

			return form;

			void AddFileToForm(AdditionalFile file) {
				string fieldName = "file" + fileIndex++;
				form.AddBinaryData(fieldName, file.data,
					string.IsNullOrWhiteSpace(file.fileName) ? fieldName : file.fileName);
			}
		}
	}
}