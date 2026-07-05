using System;
using BepInEx.Configuration;

namespace H3Status
{
    internal static class Config
    {
        public static ConfigEntry<bool> ServerEnabled;
        public static ConfigEntry<int> ServerPort;
        public static ConfigEntry<bool> LogScoreEvents;

        public static ConfigEntry<bool> SceneEvent;
        public static ConfigEntry<bool> AmmoEvent;
        public static ConfigEntry<bool> HealthEvent;
        public static ConfigEntry<bool> BuffEvent;
        public static ConfigEntry<bool> TNHLevelEvent;
        public static ConfigEntry<bool> TNHPhaseEvent;
        public static ConfigEntry<bool> TNHHoldPhaseEvent;
        public static ConfigEntry<bool> TNHScoreEvent;
        public static ConfigEntry<bool> TNHEncryptionDestroyed;
        public static ConfigEntry<bool> TNHTokenEvent;

        private static ConfigFile _configFile;

        private static ConfigEntry<T> Bind<T>(this ConfigFile config, string section, string key, T defaultValue, Action<T> callback)
        {
            ConfigEntry<T> entry = config.Bind(section, key, defaultValue);
            entry.SettingChanged += (sender, e) => callback(entry.Value);
            return entry;
        }

        public static void Init(ConfigFile config)
        {
            _configFile = config;

            string category1 = "1. General";
            string category2 = "2. Event Types";

            ServerEnabled = config.Bind(category1, "Server Enabled", true, OnServerEnabledChanged);
            ServerPort = config.Bind(category1, "Server Port", 9504, OnServerPortChanged);
            LogScoreEvents = config.Bind(category1, "Log Score Events", false);

            SceneEvent = config.Bind(category2, "Scene Changed", true);
            AmmoEvent = config.Bind(category2, "Score Changed", true);
            HealthEvent = config.Bind(category2, "Health Changed", true);
            BuffEvent = config.Bind(category2, "Powerup Activated", true);
            TNHLevelEvent = config.Bind(category2, "T&H Level Started", true);
            TNHPhaseEvent = config.Bind(category2, "T&H Phase Changed", true);
            TNHHoldPhaseEvent = config.Bind(category2, "T&H Hold Phase Changed", true);
            TNHScoreEvent = config.Bind(category2, "T&H Score Changed", true);
            TNHEncryptionDestroyed = config.Bind(category2, "T&H Encryption Destroyed", true);
            TNHTokenEvent = config.Bind(category2, "T&H Tokens Changed", true);
        }

        private static void OnServerEnabledChanged(bool enabled)
        {
            if (enabled) { Plugin.Start(); }
            else { Plugin.Stop(); }
        }

        private static void OnServerPortChanged(int port)
        {
            Server.Stop();
            Server.Start(port);
        }
    }
}
