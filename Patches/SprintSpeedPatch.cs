using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace QuotaDependentSpeed.Patches
{
    [HarmonyPatch]
    internal class SprintSpeedPatch
    {
        static float value = 1f;

        private static Dictionary<PlayerControllerB, float> previousPlayerMovementSpeeds = new Dictionary<PlayerControllerB, float>();

        [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.UpdateProfitQuotaCurrentTime))]
        [HarmonyPostfix]
        private static void UpdateValue()
        {
            value = CalculateSpeed();
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
        [HarmonyPrefix]
        private static void UpdatePrefix(PlayerControllerB __instance, ref bool ___isPlayerControlled, ref float ___movementSpeed)
        {
            if (StartOfRound.Instance.isChallengeFile)
            {
                return;
            }
            previousPlayerMovementSpeeds[__instance] = ___movementSpeed;
            if (!___isPlayerControlled)
            {
                return;
            }
            ___movementSpeed *= value;
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
        [HarmonyPostfix]
        private static void UpdatePostfix(PlayerControllerB __instance, ref bool ___isPlayerControlled, ref float ___movementSpeed)
        {
            if (StartOfRound.Instance.isChallengeFile)
            {
                return;
            }

            if (previousPlayerMovementSpeeds.ContainsKey(__instance))
            ___movementSpeed = previousPlayerMovementSpeeds[__instance];
        }

        static float CalculateSpeed()
        {
            float ratio = (float)TimeOfDay.Instance.profitQuota / QuotaDependentSpeed.quotaBaseValue.Value;
            return Mathf.Clamp(Mathf.Pow(ratio, QuotaDependentSpeed.quotaEffectScaler.Value), QuotaDependentSpeed.minSpeedMultiplier.Value, QuotaDependentSpeed.maxSpeedMultiplier.Value);
        }
    }
}
