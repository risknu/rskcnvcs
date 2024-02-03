using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RskBox {
    static public class App {
        private static PropertiesParser propertiesParser = new PropertiesParser();
        private static Networking? networking { get; set; }

        private static Font customFont = new Font("assets/PPMori-Regular.otf");
        private static Text FPSCustomText = CustomText.CreateText(customFont, new Vector2f(2.0f, 2.0f), Color.White, 10);

        private static Clock clock = new Clock();
        private static Color selectedColor = new Color(255, 255, 255);
        private static int[] mousePositionData = { 0, 0 };

        private static PixelRenderer pixelRenderer = new PixelRenderer();
        
        private static EventHandle drawEvent = new EventHandle();
        private static EventHandle colorPickerEvent = new EventHandle();
        private static EventHandle chatOpenEvent = new EventHandle();
        private static EventHandle shouldExitFromAppEvent = new EventHandle();

        public static void RunApp(RenderWindow windowNew) {
            propertiesParser.LoadPropertiesFile("rskbox.properties");
            RenderWindow window = windowNew;
            window.SetFramerateLimit(uint.Parse(propertiesParser.GetValue("WindowFramerateLimit")));

            networking = new Networking("127.0.0.1", 2425);
            Thread receiveThread = new Thread(ReceiveDataFromServer);
            receiveThread.Start();

            EventHandlerWindow(window, receiveThread);
            
            Start(window);
            while (window.IsOpen) {
                window.DispatchEvents();
                window.Clear(Color.Black);

                Update(window);
                UpdateUI(window);

                window.Display();
            }
        }

        private static void EventHandlerWindow(RenderWindow window, Thread receiveThread) {
            window.Closed += (sender, e) => {
                if (networking == null) return;
                window.Close();
                shouldExitFromAppEvent.DeactivateEventMode();
                networking.CloseConnection();
                receiveThread.Join();
            };

            window.TextEntered += (sender, e) => {
                if (networking == null) return;
                if (chatOpenEvent.IsEventModeActive() == true) {
                    Chat.HandleTextInput(e, networking);
                }
            };

            window.MouseButtonPressed += (sender, e) => {
                drawEvent.ActivateEventMode();
            }; window.MouseButtonReleased += (sender, e) => {
                if (networking == null) return;
                drawEvent.DeactivateEventMode();
                networking.SendArrayToServer(pixelRenderer.temporaryPixels);
                pixelRenderer.ClearTmp();
            };

            window.MouseMoved += (sender, e) => {
                if (drawEvent.IsEventModeActive() == true) {
                    float x = e.X / pixelRenderer.pixelCellSize;
                    float y = e.Y / pixelRenderer.pixelCellSize;
                    PixelStruct pixelStruct = new PixelStruct(x, y, selectedColor.R, selectedColor.G, selectedColor.B);
                    pixelRenderer.AddTmp(pixelStruct, x, y);
                    pixelRenderer.Add(pixelStruct, x, y);
                }
                mousePositionData[0] = e.X;
                mousePositionData[1] = e.Y;
            };

            window.KeyPressed += (sender, e) => {
                if (e.Code == Keyboard.Key.I && chatOpenEvent.IsEventModeActive() == false) {
                    colorPickerEvent.ActivateEventMode();
                } else if (e.Code == Keyboard.Key.T && chatOpenEvent.IsEventModeActive() == false) {
                    chatOpenEvent.ActivateEventMode();
                } else if (e.Code == Keyboard.Key.Escape && chatOpenEvent.IsEventModeActive() == true) {
                    chatOpenEvent.DeactivateEventMode();
                }
            }; window.KeyReleased += (sender, e) => {
                if (e.Code == Keyboard.Key.I) {
                    colorPickerEvent.DeactivateEventMode();
                }
            };
        }

        private static void ReceiveDataFromServer() {
            if (networking == null) return;
            while (shouldExitFromAppEvent.IsEventModeActive() == false) {
                object receivedData = networking.ReceiveArrayFromServer();
                if (receivedData is List<PixelStruct> receivedPixels) {
                    pixelRenderer.AddRange(receivedPixels);
                } else if (receivedData is string receivedString) {
                    if (receivedString == "null") continue;
                    Chat.ReceiveMessage(receivedString);
                }
                Thread.Sleep(10);
            }
        }

        private static void Start(RenderWindow window) {
            BackgroundGenerator.Initialize(new Vector2u(uint.Parse(propertiesParser.GetValue("WindowWidth")), 
                                                uint.Parse(propertiesParser.GetValue("WindowHeight"))));
        }

        private static void Update(RenderWindow window) {
            BackgroundGenerator.UpdateBackground(window);
            pixelRenderer.PixelsRendererMethod(window);
            selectedColor = ColorPicker.HandleColorPicker(window, mousePositionData, colorPickerEvent, selectedColor, 10);
        }

        private static void UpdateUI(RenderWindow window) {
            CustomText.UpdateTextFPS(FPSCustomText, clock);
            window.Draw(FPSCustomText);

            if (chatOpenEvent.IsEventModeActive() == false) return;
            Chat.UpdateChat(window, customFont, chatOpenEvent);
        }
    }
}
