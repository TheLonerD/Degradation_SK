using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace Degradation.Helpers
{
    public static class Utility
    {
        public static void Degrade(ThingWithComps thing, Pawn pawn)
        {
            thing.TakeDamage(new DamageInfo(DamageDefOf.Deterioration, 1f));
            if (!thing.Destroyed || !PawnUtility.ShouldSendNotificationAbout(pawn) || pawn.Dead)
                return;

            var taggedString = "MessageWornApparelDeterioratedAway".Translate(GenLabel.ThingLabel(thing.def, thing.Stuff), pawn);
            Messages.Message(taggedString.CapitalizeFirst(), pawn, MessageTypeDefOf.NegativeEvent);
        }

        public static void DegradeCheck(ThingWithComps thing, Pawn pawn)
        {
            if (thing.DestroyedOrNull())
                return;
            if (pawn.DestroyedOrNull())
                return;
            if (Settings.Excluded.Contains(thing.def.defName))
                return;
            if (QualityCheck(thing))
                return;

            Degrade(thing, pawn);
        }

        public static bool JamCheck(ThingWithComps thing, Pawn pawn)
        {
            if (!Settings.Jamming)
                return true;
            if (thing.DestroyedOrNull() || pawn.DestroyedOrNull())
                return true;
            if (Settings.Excluded.Contains(thing.def.defName))
                return true;
            if (!Rand.Bool)
                return true;
            if (QualityCheck(thing))
                return true;

            thing.def.soundInteract.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
            return false;
        }

        public static bool QualityCheck(ThingWithComps thing)
        {
            if (!QualityUtility.TryGetQuality(thing, out var qualityCategory))
                return Rand.RangeInclusive(0, 100) > Settings.Normal;

            switch (qualityCategory)
            {
                case QualityCategory.Awful:
                    return Rand.RangeInclusive(0, 100) > Settings.Awful;
                case QualityCategory.Poor:
                    return Rand.RangeInclusive(0, 100) > Settings.Poor;
                case QualityCategory.Normal:
                    return Rand.RangeInclusive(0, 100) > Settings.Normal;
                case QualityCategory.Good:
                    return Rand.RangeInclusive(0, 100) > Settings.Good;
                case QualityCategory.Excellent:
                    return Rand.RangeInclusive(0, 100) > Settings.Excellent;
                case QualityCategory.Masterwork:
                    return Rand.RangeInclusive(0, 100) > Settings.Masterwork;
                case QualityCategory.Legendary:
                    return Rand.RangeInclusive(0, 100) > Settings.Legendary;
                default:
                    return true;
            }
        }

        public static List<ThingDef> GetRangedOrMeleeWeapons() => DefDatabase<ThingDef>.AllDefsListForReading
            .Where(t => t.IsRangedWeapon || t.IsMeleeWeapon)
            .OrderBy(t => t.label)
            .ToList();

        public static void CalcRowsCols(float width, int size, int count, out int rows, out int cols)
        {
            cols = (int)Math.Truncate((double)width / size);
            rows = (int)Math.Ceiling((double)count / cols);
        }
    }
}
