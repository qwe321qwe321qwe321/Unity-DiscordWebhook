using System;
using System.IO;
using System.IO.Compression;
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
		
		/// <summary>
		/// Compress the files to a zip file in byte array.
		/// </summary>
		/// <param name="compressedFilename"></param>
		/// <param name="additionalFiles"></param>
		/// <returns></returns>
		public static AdditionalFile CompressToZip(string compressedFilename, params AdditionalFile[] additionalFiles) {
			// https://stackoverflow.com/questions/17217077/create-zip-file-from-byte
			using (var compressedFileStream = new MemoryStream())
			{
				//Create an archive and store the stream in memory.
				using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false)) {
					foreach (var file in additionalFiles) {
						//Create a zip entry for each attachment
						var zipEntry = zipArchive.CreateEntry(file.fileName);

						//Get the stream of the attachment
						using (var originalFileStream = new MemoryStream(file.data))
						using (var zipEntryStream = zipEntry.Open()) {
							//Copy the attachment stream to the zip entry stream
							originalFileStream.CopyTo(zipEntryStream);
						}
					}
				}
				
				return AdditionalFile.FromBytes(compressedFilename, compressedFileStream.ToArray());
			}
		}
	}
}