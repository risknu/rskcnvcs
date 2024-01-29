using System;
using System.Collections.Generic;
using System.Threading;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RskCnv {
    class Program {
        private static bool shouldExitFromApp = false;
        private static bool isPickerOn = false;

        private static List<PixelStruct> pixelStructs = new List<PixelStruct>();
        private static List<PixelStruct> temporaryPixels = new List<PixelStruct>();
        private static Networking networking;

        private static readonly object pixelStructsLock = new object();

        private const int outlineThickness = 1;
        private static readonly Color outlineColor = new Color(25, 25, 25);

        static void Main() {
            networking = new Networking("127.0.0.1", 5555);

            var window = new RenderWindow(new VideoMode(720, 440), "rskcnv - csharp<cli>");
            window.Closed += OnWindowClose;
            
            Color selectedColor = new Color(255, 255, 255);
            bool isDrawing = false;
            int cellSize = 10;
            int[] mousePosition = {0, 0};

            window.MouseButtonPressed += (sender, e) => isDrawing = true;
            window.MouseButtonReleased += (sender, e) => {
                isDrawing = false;
                networking.SendArrayToServer(temporaryPixels);
                temporaryPixels.Clear();
            };
            window.MouseMoved += (sender, e) => {
                if (isDrawing) {
                    float x = e.X / cellSize;
                    float y = e.Y / cellSize;
                    PixelStruct pixelStruct = new PixelStruct(x, y, selectedColor.R, selectedColor.G, selectedColor.B);
                    temporaryPixels.Add(pixelStruct);
                    pixelStructs.Add(pixelStruct);
                }
                mousePosition[0] = e.X;
                mousePosition[1] = e.Y;
            };

            window.KeyPressed += (sender, e) => {
                if (e.Code == Keyboard.Key.A) {
                    isPickerOn = true;
                }
            };
            window.KeyReleased += (sender, e) => {
                if (e.Code == Keyboard.Key.A) {
                    isPickerOn = false;
                }
            };

            Thread receiveThread = new Thread(ReceiveDataFromServer);
            receiveThread.Start();

            Font font = new Font("fonts/ARIAL.TTF");
            Text fpsText = new Text("fps: -1", font, 10);
            fpsText.Position = new Vector2f(2, 2);
            fpsText.FillColor = Color.White;
            fpsText.OutlineColor = Color.Black;
            fpsText.OutlineThickness = 1;

            Clock clock = new Clock();

            window.SetFramerateLimit(1000);
            while (window.IsOpen) {
                window.DispatchEvents();

                window.Clear(Color.Black);

                UpdateBackground(window);

                lock (pixelStructsLock) {
                    foreach (var pixel in pixelStructs) {
                        RectangleShape rectangle = new RectangleShape(new Vector2f(cellSize, cellSize));
                        rectangle.Position = new Vector2f(pixel.x * cellSize, pixel.y * cellSize);
                        rectangle.FillColor = new Color(
                            (byte)pixel.r,
                            (byte)pixel.g,
                            (byte)pixel.b
                        );
                        window.Draw(rectangle);
                    }
                }

                if (isPickerOn) {
                    Color gradientColor = CalculateGradient(mousePosition[0] / (float)window.Size.X, mousePosition[1] / (float)window.Size.Y);
                    selectedColor = gradientColor;
                    RectangleShape d = new RectangleShape(new Vector2f(cellSize, cellSize));
                    d.Position = new Vector2f(mousePosition[0]+10, mousePosition[1]+10);
                    d.FillColor = selectedColor;
                    window.Draw(d);
                }

                UpdateUI(window, fpsText, clock);
                
                window.Display();
            }
        }

        static void UpdateUI(RenderWindow window, Text fpsText, Clock clock) {
            float deltaTime = clock.Restart().AsSeconds();
            float fps = 1.0f / deltaTime;
            fpsText.DisplayedString = $"fps: {fps:N2}";
            window.Draw(fpsText);
        }

        static void UpdateBackground(RenderWindow window) {
            RectangleShape smallCell = new RectangleShape(new Vector2f(10, 10));
            smallCell.FillColor = new Color(0xFF, 0xFF, 0xFF);
            smallCell.OutlineColor = new Color(0xE8, 0xE8, 0xE8);
            smallCell.OutlineThickness = outlineThickness;

            RectangleShape cell = new RectangleShape(new Vector2f(100, 100));
            cell.FillColor = Color.Transparent;
            cell.OutlineColor = outlineColor;
            cell.OutlineThickness = outlineThickness;

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

        static Color CalculateGradient(float x, float y) {
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

        static float Lerp(float a, float b, float t) {
            return a + t * (b - a);
        }

        static void ReceiveDataFromServer() {
            while (!shouldExitFromApp) {
                List<PixelStruct> receivedPixels = networking.ReceiveArrayFromServer();
                lock (pixelStructsLock) {
                    pixelStructs.AddRange(receivedPixels);
                }
                Thread.Sleep(10);
            }
        }

        static void OnWindowClose(object? sender, EventArgs e) {
            shouldExitFromApp = true;
            RenderWindow window = (RenderWindow)sender!;
            window.Close();
            networking.CloseConnection();
        }
    }
}
