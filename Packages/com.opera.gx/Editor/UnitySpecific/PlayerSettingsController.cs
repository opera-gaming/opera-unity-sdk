using System.IO;
using UnityEditor;

namespace Opera
{
    public static class PlayerSettingsController
    {
        // This is the only standard template with the adaptive size.
        private const string RECOMMENDED_WEBGL_TEMPLATE = "PROJECT:GxGames";

        // Other compression types throw errors.
        private const WebGLCompressionFormat RECOMMENDED_COMPRESSION_FORMAT = WebGLCompressionFormat.Disabled;

        public static bool AreSettingsRecommended() => IsTemplateRecommended() &&
                                                       IsCompressionRecommended();

        public static void SetRecommendedSettings()
        {
            PlayerSettings.WebGL.template = RECOMMENDED_WEBGL_TEMPLATE;
            PlayerSettings.WebGL.compressionFormat = RECOMMENDED_COMPRESSION_FORMAT;
        }

        public static bool IsTemplateRecommended() => PlayerSettings.WebGL.template == RECOMMENDED_WEBGL_TEMPLATE;
        public static bool IsCompressionRecommended() => PlayerSettings.WebGL.compressionFormat == RECOMMENDED_COMPRESSION_FORMAT;
    }
}
