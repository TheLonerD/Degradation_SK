using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Degradation.Extensions
{
    public static class PawnExtensions
    {
        public static IEnumerable<Thing> GetHeldWeapons(this Pawn pawn) =>
            pawn.inventory.innerContainer.Where(t => t.def.IsWeapon);

        public static IEnumerable<Thing> GetAllWeapons(this Pawn pawn) =>
            pawn.equipment.GetDirectlyHeldThings().Where(t => t.def.IsWeapon).Concat(pawn.GetHeldWeapons());

        public static IEnumerable<Thing> GetDegradedWeapons(this Pawn pawn) =>
            pawn.GetAllWeapons().Where(t => t.HitPoints / t.MaxHitPoints * 100f <= Settings.Alert);
    }
}
