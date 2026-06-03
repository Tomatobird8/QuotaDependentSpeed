using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace QuotaDependentSpeed
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class QuotaDependentSpeed : BaseUnityPlugin
    {
        public static QuotaDependentSpeed Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        public static ConfigEntry<int> quotaBaseValue = null!;

        public static ConfigEntry<float> quotaEffectScaler = null!;

        public static ConfigEntry<float> minSpeedMultiplier = null!;

        public static ConfigEntry<float> maxSpeedMultiplier = null!;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            quotaBaseValue = Config.Bind<int>("General", "QuotaBaseValue", 1000, "The value of quota");

            quotaEffectScaler = Config.Bind<float>("General", "QuotaEffectScaler", 0.5f, "How strongly the difference from QuotaBaseValue affects player speed.");

            minSpeedMultiplier = Config.Bind<float>("General", "MinSpeedMultiplier", 0.01f, "How slow should the player be at slowest?");

            maxSpeedMultiplier = Config.Bind<float>("General", "MaxSpeedMultiplier", 999f, "How fast should the player be at fastest?");

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }
}
