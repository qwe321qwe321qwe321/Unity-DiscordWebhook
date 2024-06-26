using System;
using System.IO;
using UnityEngine;

namespace DiscordWebhook {
	public struct AdditionalFile {
		public string fileName;
		public byte[] data;
		
		public bool IsValid => data != null && data.Length > 0;
		
		/// <summary>
		/// Create an AdditionalFile from Texture2D.
		/// </summary>
		/// <param name="texture2D"></param>
		/// <returns></returns>
		public static AdditionalFile FromTexture(Texture2D texture2D) {
			if (!texture2D) {
				throw new ArgumentNullException(nameof(texture2D));
			}
			return new AdditionalFile() {
				fileName = "image.jpg",
				data = texture2D.EncodeToJPG(), 
			};
		}
		
		/// <summary>
		/// Create an AdditionalFile from file path.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static AdditionalFile FromPath(string filePath) {
			byte[] data = null;
			// Read file without violating file share.
			using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				using (BinaryReader binaryReader = new BinaryReader(fs))
				{
					data = binaryReader.ReadBytes((int)fs.Length); 
				}
			}
				
			return new AdditionalFile() {
				fileName = Path.GetFileName(filePath),
				data = data, 
			};
		}
		
		/// <summary>
		/// Create an AdditionalFile from byte array.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static AdditionalFile FromBytes(string fileName, byte[] data) {
			if (data == null || data.Length == 0) {
				throw new ArgumentNullException(nameof(data));
			}
			return new AdditionalFile() {
				fileName = fileName,
				data = data, 
			};
		}
		
		
	}
}