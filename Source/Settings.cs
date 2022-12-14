using System;
using System.Collections.Generic;
using System.Linq;
using Degradation.Extensions;
using Degradation.Helpers;
using UnityEngine;
using Verse;

namespace Degradation
{
    public class Settings : ModSettings
    {
        #region Fields

        private static bool _showAlert;
        private static int _awful;
        private static int _poor;
        private static int _normal;
        private static int _good;
        private static int _excellent;
        private static int _masterwork;
        private static int _legendary;
        private static int _alert;
        private static bool _jamming;
        private static HashSet<string> _excluded = new HashSet<string>();
        private string _filter = null;

        private Vector2 _scrollPosition = Vector2.zero;
        private float _lastViewRectHeight = 0;

        #endregion

        #region Properties

        public static bool ShowAlert => _showAlert;
        public static int Awful => _awful;
        public static int Poor => _poor;
        public static int Normal => _normal;
        public static int Good => _good;
        public static int Excellent => _excellent;
        public static int Masterwork => _masterwork;
        public static int Legendary => _legendary;
        public static int Alert => _alert;
        public static bool Jamming => _jamming;
        public static HashSet<string> Excluded => _excluded;

        #endregion

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _awful, "Awful", defaultValue: 20);
            Scribe_Values.Look(ref _poor, "Poor", defaultValue: 15);
            Scribe_Values.Look(ref _normal, "Normal", defaultValue: 10);
            Scribe_Values.Look(ref _good, "Good", defaultValue: 8);
            Scribe_Values.Look(ref _excellent, "Excellent", defaultValue: 6);
            Scribe_Values.Look(ref _masterwork, "Masterwork", defaultValue: 4);
            Scribe_Values.Look(ref _legendary, "Legendary", defaultValue: 2);
            Scribe_Values.Look(ref _jamming, "Jamming", defaultValue: true);
            Scribe_Values.Look(ref _alert, "Alert", defaultValue: 25);
            Scribe_Collections.Look(ref _excluded, "Excluded");
        }

        public void DoWindowContents(Rect inRect)
        {
            inRect.yMin += 20f;
            inRect.yMax -= 20f;

            var outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            var viewRect = new Rect(0f, 0f, inRect.width - 30f, _lastViewRectHeight);
            Widgets.BeginScrollView(outRect, ref _scrollPosition, viewRect);

            var listing = new Listing_Standard { maxOneColumn = false };
            listing.Begin(viewRect);

            // -- Jamming alert
            listing.CheckboxLabeled("Jamming".Translate(), ref _jamming);

            listing.Label($"{"Alert".Translate()} - {_alert}%");
            _alert = Mathf.RoundToInt(listing.Slider(_alert, 0f, 100f));

            listing.GapLine(6f);

            // -- Quality-based chance
            listing.Label("Degradation".Translate());

            listing.Label($"{"Awful".Translate()} - {_awful}%");
            _awful = Mathf.RoundToInt(listing.Slider(_awful, 0f, 100f));

            listing.Label($"{"Poor".Translate()} - {_poor}%");
            _poor = Mathf.RoundToInt(listing.Slider(_poor, 0f, 100f));
            if (_poor > _awful)
                _awful = _poor;

            listing.Label($"{"Normal".Translate()} - {_normal}%");
            _normal = Mathf.RoundToInt(listing.Slider(_normal, 0f, 100f));
            if (_normal > _poor)
                _poor = _normal;

            listing.Label($"{"Good".Translate()} - {_good}%");
            _good = Mathf.RoundToInt(listing.Slider(_good, 0f, 100f));
            if (_good > _normal)
                _normal = _good;

            listing.Label($"{"Excellent".Translate()} - {_excellent}%");
            _excellent = Mathf.RoundToInt(listing.Slider(_excellent, 0f, 100f));
            if (_excellent > _good)
                _good = _excellent;

            listing.Label($"{"Masterwork".Translate()} - {_masterwork}%");
            _masterwork = Mathf.RoundToInt(listing.Slider(_masterwork, 0f, 100f));
            if (_masterwork > _excellent)
                _excellent = _masterwork;

            listing.Label($"{"Legendary".Translate()} - {_legendary}%");
            _legendary = Mathf.RoundToInt(listing.Slider(_legendary, 0f, 100f));
            if (_legendary > _masterwork)
                _masterwork = _legendary;

            listing.GapLine();

            // --- Weapons listing with filter
            listing.Label("Exclude".Translate());

            _filter = listing.TextEntryLabeled($"{"Filter".Translate()}   ", _filter);

            // MaxColumnHeightSeen?
            var beforeListingHeight = listing.CurHeight + 48f;

            var filteredWeapons = Utility.GetRangedOrMeleeWeapons()
                .Where(w => string.IsNullOrEmpty(_filter) || w.label.ToUpper().Contains(_filter.ToUpper()))
                .ToList();
            var excludedWeapons = filteredWeapons
                .Where(w => _excluded.Contains(w.defName))
                .ToList();
            var includedWeapons = filteredWeapons
                .Where(w => !excludedWeapons.Contains(w))
                .ToList();

            // Calc weapons listing virtual height
            Utility.CalcRowsCols(
                width: listing.GetRect(0).width / 2,
                size: (int) ListingStandartExtensions.FinalIconSize,
                count: Math.Max(excludedWeapons.Count, includedWeapons.Count),
                out var rows,
                out var _
            );

            var weaponsListingHeight = rows * ListingStandartExtensions.FinalIconSize;
            var weaponsListing = listing.BeginSection(weaponsListingHeight + 20f);
            weaponsListing.WeaponList(2,
                (selectedWeapon) =>
                {
                    if (!_excluded.Contains(selectedWeapon.defName))
                        _excluded.Add(selectedWeapon.defName);
                    else
                        _excluded.Remove(selectedWeapon.defName);
                },
                new ListingStandartExtensions.WeaponColumn($"Excluded ({excludedWeapons.Count}):", excludedWeapons, ListingStandartExtensions.DrawPocketForbidden),
                new ListingStandartExtensions.WeaponColumn($"Included ({includedWeapons.Count}):", includedWeapons, ListingStandartExtensions.DrawPocketAllowed)
            );

            listing.EndSection(weaponsListing);

            listing.End();
            Widgets.EndScrollView();

            _lastViewRectHeight = beforeListingHeight + weaponsListingHeight + 64f;

            Write();
        }
    }
}
