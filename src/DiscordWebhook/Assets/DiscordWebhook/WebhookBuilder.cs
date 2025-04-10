﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using Snowflake = System.UInt64;

#if DISCORD_WEBHOOK_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif

#if UNITY_EDITOR && DISCORD_WEBHOOK_EDITOR_COROUTINE_SUPPORT
using Unity.EditorCoroutines.Editor;
#endif

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
		// Forum needs either a thread name or a thread id. 
		private string m_ThreadName;
		private Snowflake m_RepliedThreadId;

		// optional
		private string m_Username;
		private AdditionalFile m_AttachedImage;
		private List<AdditionalFile> m_AdditionalFiles;
		private List<Snowflake> m_AppliedTags;
		private bool m_CaptureScreenshot;
		private bool m_CompressAllFilesToZip;
		private string m_ZipFileName;

		private bool m_DisableWaitingForResponse;
		private bool m_DisableLogging;
		private bool m_PreventAppendThreadNameInContent;

		public const int MaximumThreadName = 100;
		public const int MaximumUsername = 80;
		public const int MaximumContent = 2000;

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
		/// [Required for Forum] Set the thread name/title of the message. (Only available for Forum)
		/// Maximum length is 100 characters, and it will be truncated and appended with "..." if it exceeds the limit.
		/// </summary>
		/// <param name="threadName"></param>
		/// <returns></returns>
		public WebhookBuilder SetThreadName(string threadName) {
			m_ThreadName = threadName;
			return this;
		}

		/// <summary>
		/// [Required for Forum] Set the thread id of the message you want to reply to. (Only available for Forum)
		/// </summary>
		/// <param name="threadId"></param>
		/// <returns></returns>
		public WebhookBuilder SetRepliedThreadId(Snowflake threadId) {
			m_RepliedThreadId = threadId;
			return this;
			
		}
		
		/// <summary>
		/// [Optional] Set whether to prevent appending the thread name to the content if it exceeds the limit.
		/// </summary>
		/// <param name="disable"></param>
		/// <returns></returns>
		public WebhookBuilder SetPreventAppendThreadNameInContent(bool disable) {
			m_PreventAppendThreadNameInContent = disable;
			return this;
		}

		/// <summary>
		/// [Optional] Set the username of the message. Maximum length is 80 characters.
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
		/// [Optional] Add additional files to the message from file paths.
		/// </summary>
		/// <param name="filePaths"></param>
		/// <returns></returns>
		public WebhookBuilder AddFiles(params string[] filePaths) {
			if (m_AdditionalFiles == null) {
				m_AdditionalFiles = new List<AdditionalFile>();
			}

			foreach (var filePath in filePaths) {
				m_AdditionalFiles.Add(AdditionalFile.FromPath(filePath));
			}
			return this;
		}
		
		/// <summary>
		/// [Optional] Add additional files to the message.
		/// </summary>
		/// <param name="files"></param>
		/// <returns></returns>
		public WebhookBuilder AddFiles(params AdditionalFile[] files) {
			if (m_AdditionalFiles == null) {
				m_AdditionalFiles = new List<AdditionalFile>();
			}

			m_AdditionalFiles.AddRange(files);
			return this;
		}

		/// <summary>
		/// [Optional] Add all files in the directory to the message.
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <returns></returns>
		public WebhookBuilder AddFilesInDirectory(string directoryPath) {
			if (m_AdditionalFiles == null) {
				m_AdditionalFiles = new List<AdditionalFile>();
			}

			// foreach file in directory
			foreach (var filePath in System.IO.Directory.GetFiles(directoryPath)) {
				m_AdditionalFiles.Add(AdditionalFile.FromPath(filePath));
			}
			return this;
		}
		

		/// <summary>
		/// [Optional] Add all files in the directory to the message.
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <param name="searchPattern"></param>
		/// <param name="searchOption"></param>
		/// <returns></returns>
		public WebhookBuilder AddFilesInDirectory(string directoryPath, string searchPattern, System.IO.SearchOption searchOption) {
			if (m_AdditionalFiles == null) {
				m_AdditionalFiles = new List<AdditionalFile>();
			}

			// foreach file in directory
			foreach (var filePath in System.IO.Directory.GetFiles(directoryPath, searchPattern, searchOption)) {
				m_AdditionalFiles.Add(AdditionalFile.FromPath(filePath));
			}
			return this;
		}

		/// <summary>
		/// [Optional] Set whether to compress all additional files to a zip file.
		/// </summary>
		/// <param name="compressAllFilesToZip"></param>
		/// <param name="zipFileName"></param>
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
		/// [Optional] Set whether to disable waiting for response. Default is false.
		/// </summary>
		/// <param name="disabled"></param>
		/// <returns></returns>
		public WebhookBuilder DisableWaitingForResponse(bool disabled) {
			m_DisableWaitingForResponse = disabled;
			return this;
		}
		
		/// <summary>
		/// [Optional] Set whether to enable error log when something goes wrong. Default is true.
		/// </summary>
		/// <param name="enabled"></param>
		/// <returns></returns>
		[Obsolete("SetErrorLogEnabled is deprecated, please use DisableLogging instead.")]
		public WebhookBuilder SetErrorLogEnabled(bool enabled) {
			m_DisableLogging = !enabled;
			return this;
		}
		
		/// <summary>
		/// [Optional] Set whether to disable logging.
		/// </summary>
		/// <param name="disabled"></param>
		/// <returns></returns>
		public WebhookBuilder DisableLogging(bool disabled) {
			m_DisableLogging = disabled;
			return this;
		}
		
		/// <summary>
		/// [Optional] Add a tag to the message. (Only available for Forum)
		/// </summary>
		/// <param name="tagId"></param>
		/// <returns></returns>
		public WebhookBuilder AddTag(Snowflake tagId) {
			if (m_AppliedTags == null) {
				m_AppliedTags = new List<Snowflake>();
			}

			m_AppliedTags.Add(tagId);
			return this;
		}
		
		/// <summary>
		/// [Optional] Add tags to the message. (Only available for Forum)
		/// </summary>
		/// <param name="tagIds"></param>
		/// <returns></returns>
		public WebhookBuilder AddTags(params Snowflake[] tagIds) {
			if (m_AppliedTags == null) {
				m_AppliedTags = new List<Snowflake>();
			}

			m_AppliedTags.AddRange(tagIds);
			return this;
		}
		
		/// <summary>
		/// [Optional] Clear and set tags to the message. (Only available for Forum)
		/// </summary>
		/// <param name="tagIds"></param>
		/// <returns></returns>
		public WebhookBuilder SetTags(params Snowflake[] tagIds) {
			if (m_AppliedTags == null) {
				m_AppliedTags = new List<Snowflake>();
			}

			m_AppliedTags.Clear();
			m_AppliedTags.AddRange(tagIds);
			return this;
		}
		
		/// <summary>
		/// Execute the webhook with coroutine and the result will be returned in the callback.
		/// The monoBehaviour is needed to run the coroutine.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public Coroutine ExecuteCoroutine(MonoBehaviour coroutineRunner, Action<WebhookResponseResult> onComplete) {
			return coroutineRunner.StartCoroutine(ExecuteIEnumerator(onComplete));
		}
		
#if UNITY_EDITOR && DISCORD_WEBHOOK_EDITOR_COROUTINE_SUPPORT
		/// <summary>
		/// Execute the webhook with coroutine and the result will be returned in the callback.
		/// The monoBehaviour is needed to run the coroutine.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public EditorCoroutine ExecuteEditorCoroutine(Action<WebhookResponseResult> onComplete) {
			return EditorCoroutineUtility.StartCoroutineOwnerless(ExecuteIEnumerator(onComplete));
		}

#endif
		
		/// <summary>
		/// Return the IEnumerator to execute the webhook.
		/// </summary>
		/// <param name="onComplete"></param>
		/// <returns></returns>
		public IEnumerator ExecuteIEnumerator(Action<WebhookResponseResult> onComplete) {
			if (CheckFieldErrors(out var error)) {
				if (!m_DisableLogging) {
					Debug.LogError(error);
				}
				var result = WebhookResponseResult.Failure(error);
				onComplete?.Invoke(result);
				yield break;
			}
			
			if (m_CaptureScreenshot) {
				// Capture screenshot will override attached image.
				Texture2D screenshot = null;
				yield return ScreenshotHelper.CaptureScreenshot(capturedTexture => { screenshot = capturedTexture; });
				if (screenshot) {
					m_AttachedImage = AdditionalFile.FromTexture(screenshot);
				}
				Object.DestroyImmediate(screenshot);
			}

			using (UnityWebRequest www = UnityWebRequest.Post(BuildUri(), BuildFormData())) {
				yield return www.SendWebRequest();
				try {
					var result = ResolveRequestResult(www);
					onComplete?.Invoke(result);
				} catch (Exception e) {
					var result = ResolveRequestException(e);
					onComplete?.Invoke(result);
				}
			}
		}

#if DISCORD_WEBHOOK_UNITASK_SUPPORT
		/// <summary>
		/// Execute the webhook with UniTask.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public async UniTask<WebhookResponseResult> ExecuteAsync() {
			if (CheckFieldErrors(out var error)) {
				if (!m_DisableLogging) {
					Debug.LogError(error);
				}
				return WebhookResponseResult.Failure(error);
			}
			
			if (m_CaptureScreenshot) {
				// Capture screenshot will override attached image.
				var screenshot = await ScreenshotHelper.CaptureScreenshot();
				if (screenshot) {
					m_AttachedImage = AdditionalFile.FromTexture(screenshot);
				}
				Object.DestroyImmediate(screenshot);
			}

			try {
				using UnityWebRequest www = UnityWebRequest.Post(BuildUri(), BuildFormData());
				await www.SendWebRequest();
				return ResolveRequestResult(www);
			} catch (Exception e) {
				return ResolveRequestException(e);
			}
		}
#endif

		private bool CheckFieldErrors(out string errorMessage) {
			// Required fields
			switch (m_ChannelType) {
				case ChannelType.TextChannel:
					if (string.IsNullOrEmpty(m_Content)) {
						errorMessage = "Content is required for TextChannel webhook.";
						return true;
					}
					break;
				case ChannelType.Forum:
					if (string.IsNullOrEmpty(m_Content)) {
						errorMessage = "Content is required for Forum webhook.";
						return true;
					}
					if (string.IsNullOrEmpty(m_ThreadName) && m_RepliedThreadId == 0) {
						errorMessage = "ThreadName or ThreadId is required for Forum webhook.";
						return true;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			errorMessage = null;
			return false;
		}
		
		private WebhookResponseResult ResolveRequestResult(UnityWebRequest request) {
			if (!request.isDone) {
				string error = "Error sending webhook: Request is not done.";
				if (!m_DisableLogging) {
					Debug.LogError(error);
				}
				return WebhookResponseResult.Failure(error);
			}
			
			if (request.result != UnityWebRequest.Result.Success) {
				string error = "Error sending webhook: " + request.error;
				if (!m_DisableLogging) {
					Debug.LogError(error);
				}
				return WebhookResponseResult.Failure(error);
			} else {
				//Debug.Log($"Webhook sent successfully. {www.downloadHandler.isDone} Response: " + www.downloadHandler.text);
				return WebhookResponseResult.Success(request.downloadHandler.text);
			}
		}
		
		private WebhookResponseResult ResolveRequestException(Exception e) {
			string error = "Error sending webhook with exception: " + e;
			if (!m_DisableLogging) {
				Debug.LogError(error);
			}
			return WebhookResponseResult.Failure(error);
		}
		
		private Uri BuildUri() {
			var uriBuilder = new UriBuilder(m_WebhookUrl);
			var query = HttpQueryUtility.ParseQueryString(uriBuilder.Query);
			query["wait"] = m_DisableWaitingForResponse ? "false" : "true";
			if (m_RepliedThreadId != 0) {
				query["thread_id"] = m_RepliedThreadId.ToString();
			}
			uriBuilder.Query = HttpQueryUtility.ToQueryString(query);
			//Debug.Log(uriBuilder.Uri);
			return uriBuilder.Uri;
		}

		private WWWForm BuildFormData() {
			var jsonPayload = new Dictionary<string, object>();
			if (m_Username != null) {
				string username = m_Username;
				if (username.Length > MaximumUsername) {
					username = username.Substring(0, MaximumUsername);
				}
				jsonPayload.Add("username", username);
			}

			string content = m_Content;
			
			if (m_ChannelType == ChannelType.Forum) {
				if (string.IsNullOrWhiteSpace(m_ThreadName) == false) {
					string threadName = m_ThreadName;

					if (m_ThreadName.Length > MaximumThreadName) {
						// Append the thread name to the content if it exceeds the limit.
						if (!m_PreventAppendThreadNameInContent) {
							string extraContent = threadName.Substring(MaximumThreadName - 3, threadName.Length - (MaximumThreadName - 3)) + "\n";
							content = extraContent + content;
						}
						
						threadName = threadName.Substring(0, (MaximumThreadName - 3)) + "...";
					}

					jsonPayload.Add("thread_name", threadName);
					
					// tags are only available for Forum.
					if (m_AppliedTags != null && m_AppliedTags.Count > 0) {
						jsonPayload.Add("applied_tags",  m_AppliedTags);
					}
				}
			}
			
			if (content.Length > MaximumContent) {
				// Truncate the content if it exceeds the limit.
				content = content.Substring(0, MaximumContent - 3) + "...";
				// TODO: Append the truncated content to the attached file or comment below.
			}
			jsonPayload.Add("content", content);
			
			WWWForm form = new();
			form.AddField("payload_json", MiniJSON.Serialize(jsonPayload));

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
					var zipFile = AdditionalFile.CompressToZip(fileName, m_AdditionalFiles.ToArray());
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