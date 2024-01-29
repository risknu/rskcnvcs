using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RskCnv {
    static class ColorPicker {
        public static Color HandleColorPicker(RenderWindow window, int[] mousePosition, bool isPickerOn, Color selectedColor, int cellSize) {
            if (isPickerOn) {
                Color gradientColor = CalculateGradient(mousePosition[0] / (float)window.Size.X, mousePosition[1] / (float)window.Size.Y);
                selectedColor = gradientColor;
                RectangleShape colorRectangle = new RectangleShape(new Vector2f(cellSize, cellSize));
                colorRectangle.Position = new Vector2f(mousePosition[0] + 10, mousePosition[1] + 10);
                colorRectangle.FillColor = selectedColor;
                window.Draw(colorRectangle);
            }

            return selectedColor;
        }

        private static Color CalculateGradient(float x, float y) {
            Color centerColor = new Color(255, 255, 255);
            Color topColor = new Color(255, 255, 0);
            Color rightColor = new Color(0, 255, 0);
            Color leftColor = new Color(255, 0, 0);
            Color bottomColor = new Color(0, 0, 255);

            byte red = (byte)Lerp(Lerp(leftColor.R, centerColor.R, x), Lerp(rightColor.R, centerColor.R, x), y);
            byte green = (byte)Lerp(Lerp(topColor.G, centerColor.G, y), Lerp(bottomColor.G, centerColor.G, y), x);
            byte blue = (byte)Lerp(Lerp(topColor.B, centerColor.B, y), Lerp(bottomColor.B, centerColor.B, y), x);

            return new Color(red, green, blue);
        }

        private static float Lerp(float a, float b, float t) {
            return a + t * (b - a);
        }
    }
}
