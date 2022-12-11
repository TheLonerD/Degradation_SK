using Degradation.Helpers;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Degradation.Harmony
{
    [HarmonyPatch(typeof(Verb_MeleeAttack), DegradationPatcher.TryCastShotKey)]
    [HarmonyPatch(DegradationPatcher.CEMeleeAttackKey, DegradationPatcher.TryCastShotKey)]
    internal class TryCastShot_Patch
    {
        [HarmonyPriority(Priority.VeryLow)]
        public static void Postfix(Verb __instance)
        {
            if (__instance.EquipmentSource.DestroyedOrNull())
                return;
            var def = __instance.EquipmentSource.def;
            if (!def.IsMeleeWeapon)
                return;
            Utility.DegradeCheck(__instance.EquipmentSource, __instance.CasterPawn);
        }
    }
}
