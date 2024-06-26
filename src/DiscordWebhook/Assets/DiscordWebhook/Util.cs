using System.IO;
using System.IO.Compression;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DiscordWebhook {
	public static class Util {
		/// <summary>
		/// Capture the screenshot and return the Texture2D.
		/// </summary>
		/// <returns></returns>
		public static async UniTask<Texture2D> CaptureScreenshot() {
#pragma warning disable CS0618 // Type or member is obsolete
			await UniTask.WaitForEndOfFrame(); // Must wait for end of frame for ScreenCapture.CaptureScreenshotAsTexture 
#pragma warning restore CS0618 // Type or member is obsolete
			return ScreenCapture.CaptureScreenshotAsTexture();
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