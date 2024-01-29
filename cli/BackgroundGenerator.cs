using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RskCnv {
    static class BackgroundGenerator {
        public static void UpdateBackground(RenderWindow window, int cellSize) {
            RectangleShape smallCell = new RectangleShape(new Vector2f(10, 10));
            smallCell.FillColor = new Color(0xFF, 0xFF, 0xFF);
            smallCell.OutlineColor = new Color(0xE8, 0xE8, 0xE8);
            smallCell.OutlineThickness = 1;

            RectangleShape cell = new RectangleShape(new Vector2f(100, 100));
            cell.FillColor = Color.Transparent;
            cell.OutlineColor = new Color(25, 25, 25);
            cell.OutlineThickness = 1;

            for (int x = 0; x < window.Size.X; x += 100) {
                for (int y = 0; y < window.Size.Y; y += 100) {
                    for (int i = 0; i < 100; i += 10) {
                        for (int j = 0; j < 100; j += 10) {
                            smallCell.Position = new Vector2f(x + i, y + j);
                            window.Draw(smallCell);
                        }
                    }
                    cell.Position = new Vector2f(x, y);
                    window.Draw(cell);
                }
            }
        }
    }
}
