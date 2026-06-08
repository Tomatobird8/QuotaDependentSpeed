using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using QuotaDependentSpeed.Extensions;
using TMPro;

namespace QuotaDependentSpeed.Patches
{
    [HarmonyPatch]
    internal class SprintSpeedPatch
    {
        static float value = 1f;

        private static Dictionary<PlayerControllerB, float> previousPlayerMovementSpeeds = new Dictionary<PlayerControllerB, float>();

        private static TextMeshProUGUI? speedDisplay;

        [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.UpdateProfitQuotaCurrentTime))]
        [HarmonyPostfix]
        private static void UpdateValue()
        {
            value = CalculateSpeed();
            if (!QuotaDependentSpeed.displayMultiplier.Value)
            {
                return;
            }
            if (speedDisplay == null){
                GameObject speedDisplayObject = new GameObject("SpeedDisplay");
                speedDisplayObject.transform.parent = HUDManager.Instance.weightCounter.transform.parent;
                TextMeshProUGUI weightCounter = HUDManager.Instance.weightCounter;
                speedDisplay = speedDisplayObject.AddComponent<TextMeshProUGUI>();
                speedDisplay.textStyle = weightCounter.textStyle;
                speedDisplay.tag = weightCounter.tag;
                speedDisplay.alignment = weightCounter.alignment;
                speedDisplay.color = weightCounter.color;
                speedDisplay.font = weightCounter.font;
                speedDisplay.fontSize = weightCounter.fontSize;
                speedDisplay.fontStyle = weightCounter.fontStyle;
                speedDisplay.fontWeight = weightCounter.fontWeight;
                speedDisplay.enableAutoSizing = weightCounter.enableAutoSizing;
                speedDisplay.fontSizeMin = weightCounter.fontSizeMin;
                speedDisplay.fontSizeMax = weightCounter.fontSizeMax;
                speedDisplay.isOverlay = weightCounter.isOverlay;
                speedDisplay.transform.position = weightCounter.transform.position;
                speedDisplay.text = "text";
                RectTransform speedDisplayTransform = speedDisplay.GetComponent<RectTransform>();
                if (speedDisplayTransform == null)
                {
                    QuotaDependentSpeed.Logger.LogError("Transform not found");
                    return;
                }
                speedDisplayTransform.offsetMin = weightCounter.GetComponent<RectTransform>().offsetMin;
                speedDisplayTransform.offsetMax = weightCounter.GetComponent<RectTransform>().offsetMax;
                speedDisplayTransform.anchoredPosition = new Vector2(67, -32);
                speedDisplayTransform.localScale = Vector3.one;
                speedDisplayTransform.localRotation = Quaternion.identity;
            }

            if (speedDisplay == null)
            {
                return;
            }
            float speedValue = QuotaDependentSpeed.currentRatio * 100f;
            string displayText = speedValue >= 1000 ? speedValue.ToString("F0") : speedValue.ToString("F2");
            speedDisplay.text = $"{displayText}%";
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
            float ratio = 0f;
            if (!QuotaDependentSpeed.randomSpeed.Value)
            {
                if (QuotaDependentSpeed.inversed.Value)
                {
                    ratio = QuotaDependentSpeed.quotaBaseValue.Value / (float)TimeOfDay.Instance.profitQuota;
                }
                else
                {
                    ratio = (float)TimeOfDay.Instance.profitQuota / QuotaDependentSpeed.quotaBaseValue.Value;
                }
                ratio = Mathf.Pow(ratio, QuotaDependentSpeed.quotaEffectScalar.Value);
                QuotaDependentSpeed.currentRatio = ratio;
                return Mathf.Clamp(ratio, QuotaDependentSpeed.minSpeedMultiplier.Value, QuotaDependentSpeed.maxSpeedMultiplier.Value);
            }

            System.Random speedRandom = new(StartOfRound.Instance.randomMapSeed + 1397);
            ratio = speedRandom.NextFloat(QuotaDependentSpeed.minSpeedMultiplier.Value, QuotaDependentSpeed.maxSpeedMultiplier.Value);
            ratio = Mathf.Pow(ratio, QuotaDependentSpeed.quotaEffectScalar.Value);
            QuotaDependentSpeed.currentRatio = ratio;
            return ratio;
        }
    }
}
