using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

class Program {
    private static  RenderWindow? window;
    private static string[] buttonsArray = {"Multiplayer", "Quit"};
    private static int currentButtonIndex = 1;

    private static SoundBuffer? soundBuffer;
    private static Sound? sound;

    private static bool enterKeyPressed = true;

    static void Main() {
        window = new RenderWindow(new VideoMode(720, 440), "rskcnvcs <cli>");
        window.Closed += (sender, e) => window.Close();

        soundBuffer = new SoundBuffer("assets/audio/onMove.wav");
        sound = new Sound(soundBuffer);

        bool gameInMenu = true;

        Font font = new Font("assets/fonts/ARIAL.TTF");

        window.SetFramerateLimit(120);
        window.KeyPressed += (sender, e) => {
            if (e.Code == Keyboard.Key.Up && enterKeyPressed == true) {
                currentButtonIndex = Math.Max(0, currentButtonIndex - 1);
                sound.Play();
            } else if (e.Code == Keyboard.Key.Down && enterKeyPressed == true) {
                currentButtonIndex = Math.Min(buttonsArray.Length - 1, currentButtonIndex + 1);
                sound.Play();
            } else if (e.Code == Keyboard.Key.Enter && enterKeyPressed == true) {
                sound.Play();
                if (buttonsArray[currentButtonIndex] == "Quit") {
                    window.Close();
                } else if (buttonsArray[currentButtonIndex] == "Multiplayer") {
                    gameInMenu = false;
                    RskCnv.App.Initialize(window);
                }
            }
        };

        window.KeyReleased += (sender, e) => {
            if (e.Code == Keyboard.Key.Enter) {
                enterKeyPressed = false;
            }
        };

        int startTextPosition = 200;
        while (gameInMenu && window.IsOpen) {
            window.DispatchEvents();

            window.Clear(Color.Black);

            for (int i = 0; i < buttonsArray.Length; i++) {
                Text menuButton = new Text(buttonsArray[i], font, 30);
                if (currentButtonIndex == i) {
                    menuButton.FillColor = Color.Yellow;
                } else {
                    menuButton.FillColor = Color.White;
                }
                menuButton.Position = new Vector2f(100, startTextPosition + (i * 35));
                window.Draw(menuButton);
            }

            window.Display();
        }
    }
}
