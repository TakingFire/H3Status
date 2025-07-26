using BepInEx.Configuration;

namespace H3Status.Config
{

    internal static class ConfigManager
    {
        public static void BindAll(ConfigFile config)
        {
            Connection.Bind(config);
        }
    }

    internal static class Connection
    {
        public static ConfigEntry<bool> useExternalAddress;
        public static ConfigEntry<string> webSocketAddress;
        public static ConfigEntry<int> webSocketPort;

        public static void Bind(ConfigFile config)
        {
            useExternalAddress = config.Bind(
                "Connection",
                "useExternalAddress",
                false,
                "Whether to connect to an external address instead of creating a local server."
            );

            webSocketAddress = config.Bind(
                "Connection",
                "webSocketAddress",
                "localhost",
                "The address to connect to when 'Use External Address' is enabled.\nNote: 'ws://' is added automatically."
            );

            webSocketPort = config.Bind(
                "Connection",
                "webSocketPort",
                9504,
                new ConfigDescription(
                    "The port used for the local WebSocket server.\nExternal connections use standard ports.",
                    new AcceptableValueRange<int>(0, 65535)
                )
            );
        }
    }

}
