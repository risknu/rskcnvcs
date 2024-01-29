using SFML.Graphics;
using SFML.System;

namespace RskCnv {
    static class FPS {
        public static Text CreateFPSText(Font font, Clock clock) {
            Text fpsText = new Text("fps: -1", font, 10);
            fpsText.Position = new Vector2f(2, 2);
            fpsText.FillColor = Color.White;
            fpsText.OutlineColor = Color.Black;
            fpsText.OutlineThickness = 1;

            UpdateFPSText(fpsText, clock);

            return fpsText;
        }

        public static void UpdateFPSText(Text fpsText, Clock clock) {
            float deltaTime = clock.Restart().AsSeconds();
            float fps = 1.0f / deltaTime;
            fpsText.DisplayedString = $"fps: {fps:N2}";
        }
    }
}