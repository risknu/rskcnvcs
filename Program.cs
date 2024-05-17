using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using RskBox;

class Program {
    static void Main() {
        RenderWindow window = new RenderWindow(new VideoMode(720, 440), "rskcnvcs");
        window.Closed += (sender, e) => { window.Close(); };
        App.RunApp(window);
    }
}
