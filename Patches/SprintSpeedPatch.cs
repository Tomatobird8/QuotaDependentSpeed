using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace QuotaDependentSpeed.Patches
{
    [HarmonyPatch]
    internal class SprintSpeedPatch
    {
        static float value = 1f;

        static float previousSpeed;

        [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.UpdateProfitQuotaCurrentTime))]
        [HarmonyPostfix]
        private static void UpdateValue()
        {
            value = CalculateSpeed();
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
        [HarmonyPrefix]
        private static void UpdatePrefix(ref bool ___isPlayerControlled, ref float ___movementSpeed)
        {
            if (!___isPlayerControlled || StartOfRound.Instance.isChallengeFile)
            {
                return;
            }
            previousSpeed = ___movementSpeed;
            ___movementSpeed *= value;
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
        [HarmonyPostfix]
        private static void UpdatePostfix(ref bool ___isPlayerControlled, ref float ___movementSpeed)
        {
            if (!___isPlayerControlled || StartOfRound.Instance.isChallengeFile)
            {
                return;
            }
            ___movementSpeed = previousSpeed;
        }

        static float CalculateSpeed()
        {
            float ratio = (float)TimeOfDay.Instance.profitQuota / QuotaDependentSpeed.quotaBaseValue.Value;
            QuotaDependentSpeed.Logger.LogDebug(ratio);
            QuotaDependentSpeed.Logger.LogDebug("pow:");
            QuotaDependentSpeed.Logger.LogDebug(Mathf.Pow(ratio, QuotaDependentSpeed.quotaEffectScaler.Value));
            QuotaDependentSpeed.Logger.LogDebug("result: " + Mathf.Clamp(Mathf.Pow(ratio, QuotaDependentSpeed.quotaEffectScaler.Value), QuotaDependentSpeed.minSpeedMultiplier.Value, QuotaDependentSpeed.maxSpeedMultiplier.Value));
            return Mathf.Clamp(Mathf.Pow(ratio, QuotaDependentSpeed.quotaEffectScaler.Value), QuotaDependentSpeed.minSpeedMultiplier.Value, QuotaDependentSpeed.maxSpeedMultiplier.Value);
        }
    }
}
