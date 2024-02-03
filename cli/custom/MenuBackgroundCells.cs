using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RskBox {
    public static class MenuBackgroundCells {
        private static int width = 720;
        private static int height = 440;

        private static int cellSize = 10;
        private static float updateInterval = 0.1f;

        private static RenderWindow? window { get; set; }
        private static Clock clock = new Clock();

        private static bool[,] cells = InitializeRandomCells(width / cellSize, height / cellSize);

        public static void RunBackground(RenderWindow newWindow) {
            window = newWindow;
        }

        public static void UpdateBackgroundMenu() {
            if (window == null || cells == null) return;
            if (clock.ElapsedTime.AsSeconds() >= updateInterval) {
                cells = UpdateCells(cells);
                clock.Restart();
            }
            DrawCells(window, cells, cellSize);
        }

        private static bool[,] InitializeRandomCells(int width, int height) {
            var random = new Random();
            bool[,] cells = new bool[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    cells[x, y] = random.Next(2) == 0;
                }
            }
            return cells;
        }
        
        private static bool[,] UpdateCells(bool[,] cells) {
            int width = cells.GetLength(0);
            int height = cells.GetLength(1);
            bool[,] newCells = new bool[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int neighbors = CountNeighbors(cells, x, y);
                    newCells[x, y] = (cells[x, y] && neighbors == 2) || neighbors == 3;
                }
            }
            return newCells;
        }

        private static int CountNeighbors(bool[,] cells, int x, int y) {
            int width = cells.GetLength(0);
            int height = cells.GetLength(1);
            int count = 0;
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    int neighborX = (x + i + width) % width;
                    int neighborY = (y + j + height) % height;
                    if (!(i == 0 && j == 0) && cells[neighborX, neighborY]) {
                        count++;
                    }
                }
            }
            return count;
        }

        private static void DrawCells(RenderWindow window, bool[,] cells, int cellSize) {
            int width = cells.GetLength(0);
            int height = cells.GetLength(1);
            RectangleShape cellShape = new RectangleShape(new Vector2f(cellSize, cellSize));
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    cellShape.Position = new Vector2f(x*cellSize, y*cellSize);
                    cellShape.FillColor = cells[x, y] ? new Color(192, 192, 192) : Color.Transparent;
                    window.Draw(cellShape);
                }
            }
        }
    }
}