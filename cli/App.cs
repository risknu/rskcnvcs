using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RskBox {
    static public class App {
        private static Clock clock = new Clock();
        private static Color selectedColor = new Color(255, 255, 255);
        private static int[] mousePositionData = { 0, 0 };

        private static PixelRenderer pixelRenderer = new PixelRenderer();
        
        private static EventHandle drawEvent = new EventHandle();
        private static EventHandle colorPickerEvent = new EventHandle();
        private static EventHandle shouldExitFromAppEvent = new EventHandle();

        public static void RunApp(RenderWindow windowNew) {
            RenderWindow window = windowNew;
            window.SetFramerateLimit(75);

            EventHandlerWindow(window);
            
            Start(window);
            while (window.IsOpen) {
                window.DispatchEvents();
                window.Clear(Color.Black);

                Update(window);
                UpdateUI(window);

                window.Display();
            }
        }

        private static void EventHandlerWindow(RenderWindow window) {
            window.Closed += (sender, e) => {
                window.Close();
                shouldExitFromAppEvent.DeactivateEventMode();
            };

            window.MouseButtonPressed += (sender, e) => {
                drawEvent.ActivateEventMode();
            }; window.MouseButtonReleased += (sender, e) => {
                drawEvent.DeactivateEventMode();
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
                if (e.Code == Keyboard.Key.I) {
                    colorPickerEvent.ActivateEventMode();
                } 
            }; window.KeyReleased += (sender, e) => {
                if (e.Code == Keyboard.Key.I) {
                    colorPickerEvent.DeactivateEventMode();
                }
            };
        }

        private static void Start(RenderWindow window) { BackgroundGenerator.Initialize(new Vector2u(720, 440)); }

        private static void Update(RenderWindow window) {
            BackgroundGenerator.UpdateBackground(window);
            pixelRenderer.PixelsRendererMethod(window);
            selectedColor = ColorPicker.HandleColorPicker(window, mousePositionData, colorPickerEvent, selectedColor, 10);
        }

        private static void UpdateUI(RenderWindow window) {}
    }
}
