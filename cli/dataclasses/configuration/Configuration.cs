using System;
using System.Collections.Generic;
using System.IO;

namespace RskBox {
    class PropertiesParser {
        private Dictionary<string, string> properties;

        public PropertiesParser() {
            properties = new Dictionary<string, string>();
        }

        public void LoadPropertiesFile(string filePath) {
            properties.Clear();
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException("[RSKBOX_ERROR] => Exception.LoadPropertiesFile: program cant found file path => " + filePath);
            }
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines) {
                if (!string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#")) {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2) {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        properties[key] = value;
                    }
                }
            }
        }

        public string GetValue(string key) {
            if (properties.ContainsKey(key)) {
                return properties[key];
            } else {
                throw new KeyNotFoundException("[RSKBOX_ERROR] => Exception.GetValue: program cant found key in properties file.");
            }
        }
    }
}
