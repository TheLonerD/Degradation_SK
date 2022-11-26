using Degradation.Helpers;
using HarmonyLib;
using Verse;

namespace Degradation.Harmony
{
    [HarmonyPatch(typeof(Verb_LaunchProjectile), DegradationPatcher.WarmupCompleteKey)]
    [HarmonyPatch(DegradationPatcher.CELaunchProjectileKey, DegradationPatcher.WarmupCompleteKey)]
    internal class WarmupComplete_Patch
    {
        [HarmonyPriority(Priority.Low)]
        public static bool Prefix(Verb __instance)
        {
            if (__instance.EquipmentSource.DestroyedOrNull())
                return true;
            if (!__instance.EquipmentSource.def.IsWeapon)
                return true;
            return Utility.JamCheck(__instance.EquipmentSource, __instance.CasterPawn);
        }

        [HarmonyPriority(Priority.VeryLow)]
        public static void Postfix(Verb __instance)
        {
            if (__instance.EquipmentSource.DestroyedOrNull())
                return;
            if (!__instance.EquipmentSource.def.IsWeapon)
                return;
            Utility.DegradeCheck(__instance.EquipmentSource, __instance.CasterPawn);
        }
    }
}
