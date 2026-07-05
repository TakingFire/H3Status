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

        public static int port = 9504;

        protected void Awake()
        {
            Logger = base.Logger;
            Patcher = new Harmony(Guid);

            Server.Start(port);

            Patcher = new Harmony(Guid);
            Patcher.PatchAll(typeof(Patches.SceneHandler));
            Patcher.PatchAll(typeof(Patches.TNHScoreHandler));
            Patcher.PatchAll(typeof(Patches.TNHPhaseHandler));
            Patcher.PatchAll(typeof(Patches.PlayerHealthHandler));
            Patcher.PatchAll(typeof(Patches.WeaponAmmoHandler));
        }

        protected void OnDestroy()
        {
            Server.Stop();
            Patcher.UnpatchSelf();
        }
    }
}
