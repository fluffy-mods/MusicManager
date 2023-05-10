using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MusicManager {
    public class Column_SongDisabled: SongTableColumn {

        public Column_SongDisabled(int width) : base(width) {}

        public override int Compare(SongDef a, SongDef b) {
            return a.commonality.CompareTo(b.commonality);
        }

        public override void DrawCell(Rect canvas, SongDef song) {
            base.DrawCell(canvas, song);
            TooltipHandler.TipRegion(canvas, () => Tooltip(song), song.GetHashCode() ^ canvas.GetHashCode());

            bool songEnabled = song.commonality > 0;
            if (Utilities.DrawButton(canvas, songEnabled ? Resources.Enabled : Resources.Disabled, IconSize.x)) {
                song.commonality = songEnabled ? 0 : 1;
                Window_MusicManager.SetDirty();
            }
        }

        public string Tooltip(SongDef song) {
            return I18n.SongDisabled(song.commonality);
        }

        public override void DrawHeader(Rect canvas, List<SongDef> songs) {
            if (Utilities.Shift()) {
                if (songs.All(s => s.commonality > 0)) {
                    if (Utilities.DrawButton(canvas, Resources.Enabled, IconSize.x)) {
                        foreach (SongDef song in songs) {
                            song.commonality = 0;
                        }

                        Window_MusicManager.SetDirty();
                    }
                } else if (songs.All(s => s.commonality == 0)) {
                    if (Utilities.DrawButton(canvas, Resources.Disabled, IconSize.x)) {
                        foreach (SongDef song in songs) {
                            song.commonality = 1;
                        }

                        Window_MusicManager.SetDirty();
                    }
                } else {
                    Rect iconRect = new Rect( Vector2.zero, IconSize ).CenteredIn( canvas );
                    GUI.DrawTextureWithTexCoords(iconRect.LeftHalf(), Resources.Enabled, new Rect(0, 0, .5f, 1));
                    GUI.DrawTextureWithTexCoords(iconRect.RightHalf(), Resources.Disabled, new Rect(.5f, 0, .5f, 1));
                    if (Utilities.DrawButton(iconRect)) {
                        foreach (SongDef song in songs.Where(s => s.commonality == 0)) {
                            song.commonality = 1;
                        }

                        Window_MusicManager.SetDirty();
                    }
                }
            } else {
                if (Utilities.DrawButton(canvas, Resources.Disabled, IconSize.x)) {
                    Window_MusicManager.SortBy = this;
                }
            }

            DrawOverlay(canvas);
            TooltipHandler.TipRegion(canvas, () => HeaderTooltip, GetHashCode());
        }

        public override bool Filter(SongDef song) {
            return true;
        }

        public override bool Filtered { get; }
        public override string HeaderTooltip => $"{I18n.SongDisabledColumn}\n\n{I18n.SongDisabledColumn_Tip}";
    }
}
