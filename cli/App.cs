using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RskCnv {
    static class App {
        private static RenderWindow window;
        private static bool shouldExitFromApp = false;
        private static bool isPickerOn = false;
        private static bool isChatOpen = false;
        private static bool isDrawing = false;

        private static List<PixelStruct> pixelStructs = new List<PixelStruct>();
        private static List<PixelStruct> temporaryPixels = new List<PixelStruct>();
        private static Networking networking;

        private static Clock clock = new Clock();
        private static Font textFont = new Font("fonts/ARIAL.TTF");
        private static Text fpsText = FPS.CreateFPSText(textFont, clock);

        private static readonly object pixelStructsLock = new object();

        private const int outlineThickness = 1;
        private static readonly Color outlineColor = new Color(25, 25, 25);

        private static int cellSize = 10;
        private static int[] mousePosition = { 0, 0 };
        private static Color selectedColor = new Color(255, 255, 255);

        public static void Initialize() {
            window = new RenderWindow(new VideoMode(720, 440), "rskcnv - csharp<cli>");
            window.Closed += OnWindowClose;

            networking = new Networking("127.0.0.1", 2425);

            Thread receiveThread = new Thread(ReceiveDataFromServer);
            receiveThread.Start();

            window.SetFramerateLimit(2000);

            window.TextEntered += (sender, e) => {
                if (isChatOpen == true) {
                    Chat.HandleTextInput(e, networking);
                }
            };

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
                    temporaryPixels.RemoveAll(p => p.x == x && p.y == y);
                    temporaryPixels.Add(pixelStruct);
                    pixelStructs.RemoveAll(p => p.x == x && p.y == y);
                    pixelStructs.Add(pixelStruct);
                }
                mousePosition = [e.X, e.Y];
            };

            window.KeyPressed += (sender, e) => {
                if (e.Code == Keyboard.Key.T && isChatOpen == false) {
                    isChatOpen = true;
                } else if (e.Code == Keyboard.Key.Escape && isChatOpen == true) {
                    isChatOpen = false;
                }
                
                if (e.Code == Keyboard.Key.A) {
                    isPickerOn = true;
                }
            };
            window.KeyReleased += (sender, e) => {
                if (e.Code == Keyboard.Key.A) {
                    isPickerOn = false;
                }
            };

            BackgroundGenerator.Initialize(new Vector2u(720, 440));

            Text debugText = new Text("fps: -1", textFont, 10);
            debugText.Position = new Vector2f(2, 15);
            debugText.FillColor = Color.White;
            debugText.OutlineColor = Color.Black;
            debugText.OutlineThickness = 1;

            while (window.IsOpen) {
                window.DispatchEvents();

                window.Clear(Color.Black);

                BackgroundGenerator.UpdateBackground(window);
                debugText.DisplayedString = pixelStructs.Count.ToString();
                window.Draw(debugText);

                lock (pixelStructsLock) {
                    RectangleShape rectangle = new RectangleShape(new Vector2f(cellSize, cellSize));
                    foreach (var pixel in pixelStructs) {
                        rectangle.Position = new Vector2f(pixel.x * cellSize, pixel.y * cellSize);
                        rectangle.FillColor = new Color((byte)pixel.r, (byte)pixel.g, (byte)pixel.b);
                        window.Draw(rectangle);
                    }
                }

                selectedColor = ColorPicker.HandleColorPicker(window, mousePosition, isPickerOn, selectedColor, cellSize);
                UpdateUI(window, textFont);

                window.Display();
            }
        }

        private static void UpdateUI(RenderWindow window, Font font) {
            FPS.UpdateFPSText(fpsText, clock);
            window.Draw(fpsText);
            if (!isChatOpen) return;

            Chat.UpdateChat(window, font, isChatOpen);
        }

        private static void OnWindowClose(object? sender, EventArgs e) {
            RenderWindow window = (RenderWindow)sender!;
            networking.CloseConnection();
            shouldExitFromApp = true;
            window.Close();
        }

        private static void ReceiveDataFromServer() {
            while (!shouldExitFromApp) {
                object receivedData = networking.ReceiveArrayFromServer();
                if (receivedData is List<PixelStruct> receivedPixels) {
                    lock (pixelStructsLock) {
                        pixelStructs.AddRange(receivedPixels);
                    }
                } else if (receivedData is string receivedString) {
                    Chat.ReceiveMessage(receivedData.ToString());
                }

                Thread.Sleep(10);
            }
        }
    }
}