using UnityEngine;

namespace DiscordWebhook {
    public static class SystemInfoHelper {
        public static string GetSystemInfoInMarkdownList() {
            return
                $"* CPU: {SystemInfo.processorType} @{SystemInfo.processorFrequency}MHz {SystemInfo.processorCount}-threads\n" +
                $"* System Memory Size: {SystemInfo.systemMemorySize} \n" +
                $"* GPU: {SystemInfo.graphicsDeviceName}\n" +
                $"* GPU Memory Size: {SystemInfo.graphicsMemorySize} \n" +
                $"* Graphics API: {SystemInfo.graphicsDeviceType}\n" +
                $"* OS: {SystemInfo.operatingSystem} \n" +
                
                $"* Screen: {Screen.width}x{Screen.height} @{GetMonitorRefreshRateHz()}, fullScreenMode={Screen.fullScreenMode}, vSync={QualitySettings.vSyncCount}, targetFrameRate={Application.targetFrameRate}\n" +
                $"* buildGUID: {Application.buildGUID}, isEditor={Application.isEditor}\n" +
                $"* Unity: {Application.unityVersion}\n"
                ;
        }

        public static string GetMonitorRefreshRateHz() {
#if UNITY_2022_2_OR_NEWER
            return Screen.currentResolution.refreshRateRatio.ToString() + "Hz";
#else
            return Screen.currentResolution.refreshRate.ToString() + "Hz";
#endif
        }
    }
}