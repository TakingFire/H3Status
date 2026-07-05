using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace H3Status
{
    [BepInProcess("h3vr.exe")]
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "xyz.bacur.plugins.h3status";
        public const string Name = "H3Status";
        public const string Version = "0.5.0";

        internal static new ManualLogSource Logger;
        internal static Harmony Patcher;

        protected void Awake()
        {
            Logger = base.Logger;
            Patcher = new Harmony(Guid);
            H3Status.Config.Init(Config);

            if (!H3Status.Config.ServerEnabled.Value)
            {
                Logger.LogWarning("Server disabled in config");
                return;
            }

            Start();
        }

        protected void OnDestroy()
        {
            Stop();
        }

        internal static void Start()
        {
            Server.Start(H3Status.Config.ServerPort.Value);
            Patcher?.PatchAll(typeof(Patches.SceneHandler));
            Patcher?.PatchAll(typeof(Patches.TNHScoreHandler));
            Patcher?.PatchAll(typeof(Patches.TNHPhaseHandler));
            Patcher?.PatchAll(typeof(Patches.PlayerHealthHandler));
            Patcher?.PatchAll(typeof(Patches.WeaponAmmoHandler));
        }

        internal static void Stop()
        {
            Server.Stop();
            Patcher?.UnpatchSelf();
        }
    }
}
