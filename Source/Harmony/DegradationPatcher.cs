using Verse;

namespace Degradation.Harmony
{
    [StaticConstructorOnStartup]
    internal static class DegradationPatcher
    {
        public const string CELaunchProjectileKey = "CombatExtended.Verb_LaunchProjectileCE";
        public const string CEMeleeAttackKey = "CombatExtended.Verb_MeleeAttackCE";
        public const string WarmupCompleteKey = "WarmupComplete";
        public const string TryCastShotKey = "TryCastShot";

        static DegradationPatcher()
        {
            var harmony = new HarmonyLib.Harmony("skyarkhangel.degradation");
            harmony.PatchAll();
        }
    }
}
