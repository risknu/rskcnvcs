using SFML.Graphics;
using SFML.System;

namespace RskBox {
    public static class ColorPicker {
        private static Color vertex1Color = new Color(255, 0, 0);
        private static Color vertex2Color = new Color(255, 255, 0);
        private static Color vertex3Color = new Color(0, 0, 255);

        public static Color HandleColorPicker(RenderWindow window, int[] mousePosition, EventHandle pickerEventHandle, Color selectedColor, int cellSize) {
            if (pickerEventHandle.IsEventModeActive() == true) {
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
            float alpha, beta, gamma;
            (alpha, beta, gamma) = ((1 - x - y), x, y);

            byte red = (byte)(alpha * vertex1Color.R + beta * vertex2Color.R + gamma * vertex3Color.R);
            byte green = (byte)(alpha * vertex1Color.G + beta * vertex2Color.G + gamma * vertex3Color.G);
            byte blue = (byte)(alpha * vertex1Color.B + beta * vertex2Color.B + gamma * vertex3Color.B);

            return new Color(red, green, blue);
        }
    }
}
