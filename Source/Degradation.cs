using UnityEngine;
using Verse;

namespace Degradation
{
    public class Degradation : Mod
    {
        private readonly Settings _settings;

        public Degradation(ModContentPack content) : base(content)
        {
            _settings = GetSettings<Settings>();
        }

        public override string SettingsCategory() => "Degradation";

        public override void DoSettingsWindowContents(Rect inRect) => _settings.DoWindowContents(inRect);
    }
}
