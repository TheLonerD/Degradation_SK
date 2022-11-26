using System.Collections.Generic;
using System.Linq;
using System.Text;
using Degradation.Extensions;
using RimWorld;
using Verse;

namespace Degradation.Alerts
{
    public class Alert_Degradation : Alert
    {
        private IEnumerable<Pawn> _pawnsWithDegradedWeapons
        {
            get
            {
                foreach (var pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
                {
                    if (HaveDegradedWeapon(pawn))
                        yield return pawn;
                }
            }
        }

        public Alert_Degradation()
        {
            defaultPriority = AlertPriority.High;
        }

        public override string GetLabel()
        {
            var colonistsCount = _pawnsWithDegradedWeapons
                .Count();
            return "DegradeWarning".Translate(colonistsCount.ToStringCached());
        }

#if RW10
        public override string GetExplanation()
#else
        public override TaggedString GetExplanation()
#endif
        {
            var sb = new StringBuilder();
            foreach (var pawn in _pawnsWithDegradedWeapons)
            {
                var weapons = pawn.GetDegradedWeapons()
                    .Select(thing => GenLabel.ThingLabel(thing.def, thing.Stuff))
                    .OrderBy(thing => thing)
                    .ToCommaList();
                sb.AppendLine($"    {pawn} ({weapons})");
            }
            return "DegradeWarningDesc".Translate(sb.ToString().TrimEnd());
        }

        public override AlertReport GetReport()
        {
            if (!Settings.ShowAlert)
                return false;

            var culprits = _pawnsWithDegradedWeapons.ToList();
            return AlertReport.CulpritsAre(culprits);
        }

        private static bool HaveDegradedWeapon(Pawn p)
        {
            return p.GetDegradedWeapons().Any();
        }
    }
}
