using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RskCnv {
    static class BackgroundGenerator {
        private static RenderTexture backgroundTexture;

        public static void Initialize(Vector2u size) {
            backgroundTexture = new RenderTexture(size.X, size.Y);
            GenerateBackgroundTexture(size.X, size.Y);
        }

        private static void GenerateBackgroundTexture(uint width, uint height) {
            backgroundTexture.Clear(new Color(0, 0, 0, 0));

            RectangleShape smallCell = new RectangleShape(new Vector2f(10, 10));
            smallCell.FillColor = new Color(23, 20, 27);
            smallCell.OutlineColor = new Color(27, 24, 30);
            smallCell.OutlineThickness = 1;

            RectangleShape cell = new RectangleShape(new Vector2f(100, 100));
            cell.FillColor = Color.Transparent;
            cell.OutlineColor = new Color(40, 39, 42);
            cell.OutlineThickness = 1;

            for (int x = 0; x < width; x += 100) {
                for (int y = 0; y < height; y += 100) {
                    for (int i = 0; i < 100; i += 10) {
                        for (int j = 0; j < 100; j += 10) {
                            smallCell.Position = new Vector2f(x + i, y + j);
                            backgroundTexture.Draw(smallCell);
                        }
                    }
                    cell.Position = new Vector2f(x, y);
                    backgroundTexture.Draw(cell);
                }
            }
            backgroundTexture.Display();
        }

        public static void UpdateBackground(RenderWindow window) {
            Sprite backgroundSprite = new Sprite(backgroundTexture.Texture);
            window.Draw(backgroundSprite);
        }
    }
}
