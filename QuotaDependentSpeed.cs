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

        public static ConfigEntry<float> quotaEffectScalar = null!;

        public static ConfigEntry<float> minSpeedMultiplier = null!;

        public static ConfigEntry<float> maxSpeedMultiplier = null!;

        public static ConfigEntry<bool> inversed = null!;

        public static ConfigEntry<bool> randomSpeed = null!;

        public static ConfigEntry<bool> displayMultiplier = null!;

        public static ConfigEntry<bool> setSpeedOnCompany = null!;

        public static ConfigEntry<float> companySpeed = null!;

        public static float currentRatio = 0;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            quotaBaseValue = Config.Bind<int>("General", "QuotaBaseValue", 1000, "The value of quota");

            quotaEffectScalar = Config.Bind<float>("General", "QuotaEffectScalar", 0.5f, "How strongly the difference from QuotaBaseValue affects player speed.");

            minSpeedMultiplier = Config.Bind<float>("General", "MinSpeedMultiplier", 0.01f, "Minimum player speed multiplier.");

            maxSpeedMultiplier = Config.Bind<float>("General", "MaxSpeedMultiplier", 999f, "Maximum player speed multiplier.");

            inversed = Config.Bind<bool>("General", "Decrease speed", false, "Decrease player speed as quota increases.");

            randomSpeed = Config.Bind<bool>("General", "Random speed", false, "Choose a random speed instead of using the quota to calculate speed. Use min & max speed multipliers to set the limits. Random speed is independent from quota value. Scalar still affects the intensity.");

            displayMultiplier = Config.Bind<bool>("General", "Display multiplier", true, "Should the multiplier be displayed in the HUD during gameplay?");

            setSpeedOnCompany = Config.Bind<bool>("General", "Set speed on company", true, "Set a static speed value on company. Speed multipliers will not apply.");

            companySpeed = Config.Bind<float>("General", "Company speed multiplier", 1.0f, "Static speed multiplier on company.");

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
