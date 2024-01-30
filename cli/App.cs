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

        private static readonly object lockObject = new object();
        private static string messageContent = "";
        private static System.Text.StringBuilder messageContentBuilder = new System.Text.StringBuilder();

        private static string userNickname = "CLI<1488>";

        private static string inputText = "";
        private static readonly object pixelStructsLock = new object();

        private const int outlineThickness = 1;
        private static readonly Color outlineColor = new Color(25, 25, 25);

        private static int cellSize = 10;
        private static int[] mousePosition = {0, 0};
        private static Color selectedColor = new Color(255, 255, 255);

        private static int chatWidth = 220;
        private static int chatHeight = 440;
        private static int lineHeight = 13;

        public static void Initialize() {
            window = new RenderWindow(new VideoMode(720, 440), "rskcnv - csharp<cli>");
            window.Closed += OnWindowClose;

            networking = new Networking("178.63.27.183", 2425);

            Thread receiveThread = new Thread(ReceiveDataFromServer);
            receiveThread.Start();

            window.SetFramerateLimit(1000);

            window.TextEntered += (sender, e) => {
                if (isChatOpen == true) {
                    char inputChar = e.Unicode[0];
                    if (inputChar == '\n' && inputText.Length != 0) {
                        if (inputText.Length < 32) {
                            networking.SendMessageToServer("\n"+userNickname+" "+inputText);
                            lock (lockObject) {
                                messageContentBuilder.AppendLine("You: "+inputText);
                            }
                            inputText = "";
                        }
                    } else if (inputChar == '\b' && inputText.Length != 0) {
                        inputText = inputText.Remove(inputText.Length-1);
                    } else {
                        if (inputChar != '\b' && inputChar != '\n' && inputText.Length < 32) {
                            inputText += inputChar;
                        }
                    }
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
                    temporaryPixels.Add(pixelStruct);
                    pixelStructs.RemoveAll(p => p.x == x && p.y == y);
                    pixelStructs.Add(pixelStruct);
                }
                mousePosition[0] = e.X;
                mousePosition[1] = e.Y;
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

            lock (lockObject) {
                messageContent = messageContentBuilder.ToString();
            }

            RectangleShape backgroundTextMessages = new RectangleShape(new Vector2f(chatWidth, chatHeight));
            backgroundTextMessages.FillColor = new Color(0, 0, 0, 50);
            window.Draw(backgroundTextMessages);

            Text messageText = new Text(messageContent, font, 10);
            messageText.Position = new Vector2f(2, chatHeight - ((messageContent.Count(c => c == '\n') + 2) * 13));
            messageText.FillColor = Color.White;
            messageText.OutlineColor = Color.Black;
            messageText.OutlineThickness = 1;
            window.Draw(messageText);

            RectangleShape backgroundText = new RectangleShape(new Vector2f(chatWidth, lineHeight));
            backgroundText.FillColor = new Color(0, 0, 0, 75);
            backgroundText.Position = new Vector2f(0, chatHeight - lineHeight);
            window.Draw(backgroundText);

            Text inputTextDisplay = new Text($"{inputText}", font, 10);
            inputTextDisplay.Position = new Vector2f(2, chatHeight - lineHeight);
            inputTextDisplay.FillColor = Color.White;
            inputTextDisplay.OutlineColor = Color.Black;
            inputTextDisplay.OutlineThickness = 1;
            window.Draw(inputTextDisplay);
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
                    lock (lockObject) {
                        messageContentBuilder.AppendLine(receivedData.ToString());
                    }
                }

                Thread.Sleep(10);
            }
        }
    }
}
