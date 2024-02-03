using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

namespace RskBox {
    public class Menu {
        private static PropertiesParser propertiesParser = new PropertiesParser();
        
        private static RenderWindow? window { get; set; }
        private static string[] buttonsArray = { "Multiplayer", "Quit" };
        private static int currentButtonIndex = 1;

        private static SoundBuffer? soundBuffer { get; set; }
        private static Sound? sound { get; set; }

        private static EventHandle enterKeyPressed = new EventHandle();
        private static EventHandle gameNotInMenu = new EventHandle();

        private static Font font = new Font("assets/PPMori-Regular.otf");

        public Menu(RenderWindow windowNew) {
            propertiesParser.LoadPropertiesFile("rskbox.properties");
            window = windowNew;
        }

        public void EventHandleWindow(RenderWindow window, Sound sound) {
            window.KeyReleased += (sender, e) => {
                if (e.Code == Keyboard.Key.Enter) {
                    enterKeyPressed.ActivateEventMode();
                }
            };

            window.KeyPressed += (sender, e) => {
                if (e.Code == Keyboard.Key.Up && enterKeyPressed.IsEventModeActive() == false) {
                    currentButtonIndex = Math.Max(0, currentButtonIndex - 1);
                    sound.Play();
                } else if (e.Code == Keyboard.Key.Down && enterKeyPressed.IsEventModeActive() == false) {
                    currentButtonIndex = Math.Min(buttonsArray.Length - 1, currentButtonIndex + 1);
                    sound.Play();
                } else if (e.Code == Keyboard.Key.Enter && enterKeyPressed.IsEventModeActive() == false) {
                    sound.Play();
                    if (buttonsArray[currentButtonIndex] == "Quit") {
                        window.Close();
                    } else if (buttonsArray[currentButtonIndex] == "Multiplayer") {
                        gameNotInMenu.ActivateEventMode();
                        StartMultiplayerClient();
                    }
                }
            };
        }

        public void Run() {
            if (window == null) return;
            Start();
            while (window.IsOpen && gameNotInMenu.IsEventModeActive() == false) {
                window.DispatchEvents();

                window.Clear(Color.Black);

                Update();
                UpdateUI();

                window.Display();
            }
        }

        private void Start() {
            if (window == null) return;
            soundBuffer = new SoundBuffer("assets/audio/onMove.wav");
            BackgroundGenerator.Initialize(new Vector2u(uint.Parse(propertiesParser.GetValue("WindowWidth")), 
                                                uint.Parse(propertiesParser.GetValue("WindowHeight"))));
            sound = new Sound(soundBuffer);
            EventHandleWindow(window, sound);
            MenuBackgroundCells.RunBackground(window);
        }

        private void Update() {
            if (window == null) return;
            BackgroundGenerator.UpdateBackground(window);
            MenuBackgroundCells.UpdateBackgroundMenu();
        }

        private void UpdateUI() {
            if (window == null) return;
            for (int i = 0; i < buttonsArray.Length; i++) {
                Text menuButton = CustomText.CreateText(font, new Vector2f(2.0f, 2.0f), Color.White, 30);
                CustomText.UpdateText(menuButton, buttonsArray[i]);
                if (currentButtonIndex == i) {
                    menuButton.FillColor = Color.Yellow;
                } else {
                    menuButton.FillColor = Color.White;
                }
                menuButton.Position = new Vector2f(100, 200 + (i * 35));
                window.Draw(menuButton);
            }
        }

        private static void StartMultiplayerClient() {
            if (window == null) return;
            App.RunApp(window);
        }
    }
}
