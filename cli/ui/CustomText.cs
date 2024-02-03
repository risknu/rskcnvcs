using SFML.Graphics;
using SFML.System;

namespace RskBox {
    static public class CustomText {
        public static Text CreateText(Font font, Vector2f position, Color fillColor, uint size) {
            Text customText = new Text("custom_text", font, size);
            customText.Position = position;
            customText.FillColor = fillColor;
            customText.OutlineColor = Color.Black;
            customText.OutlineThickness = 1.0f;
            return customText;
        }

        public static void UpdateTextFPS(Text customText, Clock clock) {
            float deltaTime = clock.Restart().AsSeconds();
            float fpsCount = 1.0f / deltaTime;
            customText.DisplayedString = $"fps: {fpsCount:N0}";
        }

        public static void UpdateText(Text customText, string updateTo) {
            customText.DisplayedString = updateTo;
        }
    }
}