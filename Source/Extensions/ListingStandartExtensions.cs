using System;
using System.Collections.Generic;
using System.Linq;
using Degradation.Helpers;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Degradation.Extensions
{
    public static class ListingStandartExtensions
    {
        public const float IconSize = 64f;
        public const float IconGap = 4f;
        public const float SeparatorPadding = 16f;
        public static float FinalIconSize => IconSize + IconGap;

        public static readonly Color IconBaseColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Color IconMouseOverColor = new Color(0.6f, 0.6f, 0.4f, 1f);

        public static readonly Texture2D DrawPocket = ContentFinder<Texture2D>.Get("drawPocket", true);
        public static readonly Texture2D DrawPocketAllowed = ContentFinder<Texture2D>.Get("drawPocketAllowed", true);
        public static readonly Texture2D DrawPocketForbidden = ContentFinder<Texture2D>.Get("drawPocketForbidden", true);
        public static readonly Texture2D MissingUiIcon = ContentFinder<Texture2D>.Get("missingIcon", true);

        public static void WeaponList(
          this Listing_Standard instance,
          int columns,
          Action<ThingDef> onChange = null,
          params WeaponColumn[] weaponColumns)
        {
            var width = instance.GetRect(0).width;
            var columnRectWidth = width / columns;
            Utility.CalcRowsCols(columnRectWidth, (int) FinalIconSize, weaponColumns.Max(wc => wc.Weapons.Count), out var rows, out var cols);
            var listingRect = instance.GetRect(rows * FinalIconSize);

            for (int colNum = 0; colNum < columns; colNum++)
            {
                var column = weaponColumns[colNum];
                var rect = new Rect(columnRectWidth * colNum, listingRect.y, columnRectWidth, listingRect.height);

                if (!column.Label.NullOrEmpty())
                {
                    Widgets.Label(rect, column.Label);
                    rect.y += 20f;
                    rect.height += 20f;
                }
                WeaponList(rect, column, cols, onChange);

                if (colNum != columns - 1)
                {
                    var gap = columnRectWidth - ((cols * FinalIconSize) - IconGap - 2f);
                    Widgets.DrawLineVertical((columnRectWidth * (colNum + 1)) - (gap / 2), SeparatorPadding, listingRect.height - SeparatorPadding - 16f);
                }
            }
        }

        public static void WeaponList(
          Rect rect,
          WeaponColumn weaponColumn,
          int cols,
          Action<ThingDef> onChange = null)
        {
            Color color = GUI.color;
            for (var pos = 0; pos < weaponColumn.Weapons.Count(); pos++)
            {
                var col = pos % cols;
                int row = pos / cols;
                var offset = new Vector2(col * FinalIconSize, row * FinalIconSize);

                var clicked = DrawIconForWeapon(weaponColumn, pos, rect, offset);
                if (clicked)
                    onChange?.Invoke(weaponColumn.Weapons[pos]);
            }
            GUI.color = color;
        }

        public static bool DrawIconForWeapon(WeaponColumn weaponColumn, int itemNum, Rect contentRect, Vector2 offset)
        {
            var weapon = weaponColumn.Weapons[itemNum];
            var backgroundTexture = weaponColumn.Background ?? DrawPocket;
            var graphic = weapon.graphicData.Graphic;
            var color = weapon.graphicData.color;
            var colorTwo = weapon.graphicData.colorTwo;
            var iconRect = new Rect(contentRect.x + offset.x, contentRect.y + offset.y, IconSize, IconSize);

            // Handle tooltip and draw background
            TooltipHandler.TipRegion(iconRect, weapon.label);
            MouseoverSounds.DoRegion(iconRect, SoundDefOf.Mouseover_Command);
            if (Mouse.IsOver(iconRect))
            {
                GUI.color = IconBaseColor;
                GUI.DrawTexture(iconRect, backgroundTexture);
            }
            else
            {
                GUI.color = IconBaseColor;
                GUI.DrawTexture(iconRect, backgroundTexture);
                GUI.DrawTextureWithTexCoords(iconRect, backgroundTexture, new Rect(0.0f, 0.0f, 1f, 1f));
            }

            // Draw weapon icon
            GUI.color = color;
            var weaponTexture = weapon.uiIcon ?? MissingUiIcon;
            GUI.DrawTexture(iconRect, weaponTexture);
            GUI.color = Color.white;
            return Widgets.ButtonInvisible(iconRect);
        }

        public struct WeaponColumn
        {
            public string Label;
            public List<ThingDef> Weapons;
            public Texture2D Background;

            public WeaponColumn(List<ThingDef> weapons)
            {
                Label = null;
                Weapons = weapons;
                Background = null;
            }

            public WeaponColumn(List<ThingDef> weapons, Texture2D background)
            {
                Label = null;
                Weapons = weapons;
                Background = background;
            }

            public WeaponColumn(string label, List<ThingDef> weapons, Texture2D background)
            {
                Label = label;
                Weapons = weapons;
                Background = background;
            }
        }
    }
}
