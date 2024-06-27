using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DiscordWebhook {
	public static class ScreenshotHelper {
		/// <summary>
		/// Capture the screenshot and return the Texture2D.
		/// </summary>
		/// <returns></returns>
		public static async UniTask<Texture2D> CaptureScreenshot(MonoBehaviour coroutineRunner = null) {
			// Must wait for end of frame for ScreenCapture.CaptureScreenshotAsTexture 
			if (coroutineRunner) {
				await UniTask.WaitForEndOfFrame(coroutineRunner); 
			} else {
				// This equals to WaitForEndOfFrame() without argument but it's not working with scene view in Unity.
				await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
			}
			return ScreenCapture.CaptureScreenshotAsTexture();
		}
	}
}