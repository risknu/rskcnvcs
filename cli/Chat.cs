using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RskCnv {
    static class Chat {
        private static readonly object lockObject = new object();
        private static string messageContent = "";
        private static System.Text.StringBuilder messageContentBuilder = new System.Text.StringBuilder();

        private static string userNickname = "CLI<1488>";
        private static string inputText = "";

        private static int chatWidth = 220;
        private static int chatHeight = 440;
        private static int lineHeight = 13;

        public static void HandleTextInput(TextEventArgs e, Networking networking) {
            char inputChar = e.Unicode[0];
            if (inputChar == '\n' && inputText.Length != 0) {
                if (inputText.Length < 32) {
                    networking.SendMessageToServer("\n" + userNickname + " " + inputText);
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

        public static void UpdateChat(RenderWindow window, Font font, bool isChatOpen) {
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

        public static void ReceiveMessage(string receivedData) {
            lock (lockObject) {
                messageContentBuilder.AppendLine(receivedData);
            }
        }
    }
}
