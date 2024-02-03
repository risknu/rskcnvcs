using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RskBox {
    static class Chat {
        private static readonly object lockObject = new object();
        private static string messageContent = "";
        private static System.Text.StringBuilder messageContentBuilder = new System.Text.StringBuilder();

        private static string userNickname = "CLI<1488>";
        private static string inputText = "";

        public static void HandleTextInput(TextEventArgs e, Networking networking) {
            char inputChar = e.Unicode[0];
            if (inputChar == '\n' && inputText.Length != 0) {
                if (inputText.Length < 32) {
                    networking.SendMessageToServer(userNickname + " " + inputText);
                    lock (lockObject) {
                        messageContentBuilder.AppendLine("You: " + inputText);
                    }
                    inputText = "";
                }
            } else if (inputChar == '\b' && inputText.Length != 0) {
                inputText = inputText.Remove(inputText.Length - 1);
            } else {
                if (inputChar != '\b' && inputChar != '\n' && inputText.Length < 32) {
                    inputText += inputChar;
                }
            }
        }

        public static void UpdateChat(RenderWindow window, Font font, EventHandle isChatOpenEvent) {
            if (isChatOpenEvent.IsEventModeActive() == false) return;

            lock (lockObject) {
                messageContent = messageContentBuilder.ToString();
            }

            RectangleShape backgroundTextMessages = new RectangleShape(new Vector2f(220, 440));
            backgroundTextMessages.FillColor = new Color(0, 0, 0, 50);
            window.Draw(backgroundTextMessages);

            Text messageText = new Text(messageContent, font, 10);
            messageText.Position = new Vector2f(2, 440 - ((messageContent.Count(c => c == '\n') + 2) * 13));
            messageText.FillColor = Color.White;
            messageText.OutlineColor = Color.Black;
            messageText.OutlineThickness = 1;
            window.Draw(messageText);

            RectangleShape backgroundText = new RectangleShape(new Vector2f(220, 11));
            backgroundText.FillColor = new Color(0, 0, 0, 75);
            backgroundText.Position = new Vector2f(0, 440 - 11);
            window.Draw(backgroundText);

            Text inputTextDisplay = new Text($"{inputText}", font, 10);
            inputTextDisplay.Position = new Vector2f(2, 440 - 11);
            inputTextDisplay.FillColor = Color.White;
            inputTextDisplay.OutlineColor = Color.Black;
            inputTextDisplay.OutlineThickness = 1;
            window.Draw(inputTextDisplay);
        }

        public static void ReceiveMessage(string receivedData) {
            lock (lockObject) {
                messageContentBuilder.AppendLine(receivedData);
            }
        }
    }
}
