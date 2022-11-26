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
        public static readonly Texture2D MissingDefIcon = ContentFinder<Texture2D>.Get("missingIcon", true);

        public static void WeaponList(
          this Listing_Standard instance,
          int columns,
          Action<ThingDef> onChange = null,
          params List<ThingDef>[] thingDefs)
        {
            var width = instance.GetRect(0).width;
            var columnRectWidth = width / columns;
            Utility.CalcRowsCols(columnRectWidth, (int)FinalIconSize, thingDefs.Max(t => t.Count), out var rows, out var cols);
            var listingRect = instance.GetRect(rows * FinalIconSize);

            for (int colNum = 0; colNum < columns; colNum++)
            {
                var rect = new Rect(columnRectWidth * colNum, listingRect.y, columnRectWidth, listingRect.height);
                WeaponList(rect, thingDefs[colNum], cols, onChange);
                if (colNum != columns - 1)
                {
                    var gap = columnRectWidth - ((cols * FinalIconSize) - IconGap - 2f);
                    Widgets.DrawLineVertical((columnRectWidth * (colNum + 1)) - (gap / 2), SeparatorPadding, listingRect.height - SeparatorPadding - 16f);
                }
            }
        }

        public static void WeaponList(
          Rect rect,
          List<ThingDef> weapons,
          int cols,
          Action<ThingDef> onChange = null)
        {
            Color color = GUI.color;
            for (var pos = 0; pos < weapons.Count(); pos++)
            {
                var col = pos % cols;
                int row = pos / cols;
                var offset = new Vector2(col * FinalIconSize, row * FinalIconSize);

                var clicked = DrawIconForWeapon(weapons[pos], rect, offset);
                if (clicked)
                    onChange?.Invoke(weapons[pos]);
            }
            GUI.color = color;
        }

        public static bool DrawIconForWeapon(ThingDef weapon, Rect contentRect, Vector2 offset)
        {
            var graphic = weapon.graphicData.Graphic;
            var color = weapon.graphicData.color;
            var colorTwo = weapon.graphicData.colorTwo;
            var coloredVersion = weapon.graphicData.Graphic.GetColoredVersion(graphic.Shader, color, colorTwo);
            var iconRect = new Rect(contentRect.x + offset.x, contentRect.y + offset.y, IconSize, IconSize);

            // Handle tooltip and draw background
            TooltipHandler.TipRegion(iconRect, weapon.label);
            MouseoverSounds.DoRegion(iconRect, SoundDefOf.Mouseover_Command);
            if (Mouse.IsOver(iconRect))
            {
                GUI.color = IconBaseColor;
                GUI.DrawTexture(iconRect, DrawPocket);
            }
            else
            {
                GUI.color = IconBaseColor;
                GUI.DrawTexture(iconRect, DrawPocket);
                GUI.DrawTextureWithTexCoords(iconRect, DrawPocket, new Rect(0.0f, 0.0f, 1f, 1f));
            }

            // Draw weapon icon
            GUI.color = color;
            var weaponTexture = weapon.uiIcon ?? coloredVersion.MatSingle.mainTexture ?? MissingDefIcon;
            GUI.DrawTexture(iconRect, weaponTexture);
            GUI.color = Color.white;
            return Widgets.ButtonInvisible(iconRect);
        }
    }
}
