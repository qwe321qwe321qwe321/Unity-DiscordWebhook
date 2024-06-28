using System;
using System.Collections;
using UnityEngine;

#if UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif

namespace DiscordWebhook {
	public static class ScreenshotHelper {
		/// <summary>
		/// Capture a screenshot and return the Texture2D from the callback.
		/// </summary>
		/// <returns></returns>
		public static IEnumerator CaptureScreenshot(Action<Texture2D> resultCallback) {
			// Must wait for end of frame for ScreenCapture.CaptureScreenshotAsTexture 
			yield return new WaitForEndOfFrame();
			resultCallback?.Invoke(ScreenCapture.CaptureScreenshotAsTexture());
		}
		
		#if UNITASK_SUPPORT
		/// <summary>
		/// Capture a screenshot and return the Texture2D.
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
		#endif
	}
}