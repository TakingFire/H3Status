using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using WebSocketSharp.Server;

namespace H3Status;

[BepInProcess("h3vr.exe")]
[BepInPlugin(Guid, Name, Version)]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "xyz.bacur.plugins.h3status";
    public const string Name = "H3Status";
    public const string Version = "0.3.0";


    internal static new ManualLogSource Logger;
    private static Harmony _harmony;

    private void Awake()
    {
        H3Status.Config.ConfigManager.BindAll(Config);
        int port = H3Status.Config.Connection.webSocketPort.Value;

        Logger = base.Logger;

        _harmony = new Harmony(Guid);
        _harmony.PatchAll(typeof(Patches.SceneHandler));
        _harmony.PatchAll(typeof(Patches.TNHScoreHandler));
        _harmony.PatchAll(typeof(Patches.TNHPhaseHandler));
        _harmony.PatchAll(typeof(Patches.PlayerHealthHandler));
        _harmony.PatchAll(typeof(Patches.WeaponAmmoHandler));

        Server.ServerManager.Start();
    }

    private void OnDestroy()
    {
        Server.ServerManager.Stop();
        _harmony.UnpatchSelf();
        Config.Clear();
    }
}
