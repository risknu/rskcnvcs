using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using RskBox;

class Program {
    private static PropertiesParser propertiesParser = new PropertiesParser();

    static void Main() {
        propertiesParser.LoadPropertiesFile("rskbox.properties");
        RenderWindow window = new RenderWindow(new VideoMode(uint.Parse(propertiesParser.GetValue("WindowWidth")), 
                                                uint.Parse(propertiesParser.GetValue("WindowHeight"))), 
                                                propertiesParser.GetValue("WindowTitle"));
        window.Closed += (sender, e) => { window.Close(); };
        Menu menu = new Menu(window);
        menu.Run();
    }
}
