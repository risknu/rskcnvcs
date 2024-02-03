using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace RskBox {
    public class PixelRenderer {
        private static List<PixelStruct> pixelStructs = new List<PixelStruct>(); 
        public List<PixelStruct> temporaryPixels = new List<PixelStruct>();

        private static readonly object pixelStructsLock = new object();

        public int pixelCellSize = 10;

        public void PixelsRendererMethod(RenderWindow window) {
            lock (pixelStructsLock) {
                RectangleShape rectangle = new RectangleShape(new Vector2f(pixelCellSize, pixelCellSize));
                foreach (var pixel in pixelStructs) {
                    rectangle.Position = new Vector2f(pixel.x * pixelCellSize, pixel.y * pixelCellSize);
                    rectangle.FillColor = new Color((byte)pixel.r, (byte)pixel.g, (byte)pixel.b);
                    window.Draw(rectangle);
                }
            }
        }

        public void ClearTmp() {
            temporaryPixels.Clear();
        }

        public void AddTmp(PixelStruct pixelStruct, float x, float y) {
            temporaryPixels.RemoveAll(p => p.x == x && p.y == y);
            temporaryPixels.Add(pixelStruct);
        }

        public void Add(PixelStruct pixelStruct, float x, float y) {
            pixelStructs.RemoveAll(p => p.x == x && p.y == y);
            pixelStructs.Add(pixelStruct);
        }

        public void AddRange(List<PixelStruct> receivedPixels) {
            lock (pixelStructsLock) {
                pixelStructs.AddRange(receivedPixels);
            }
        }
    }
}
