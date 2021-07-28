// Column_TimeOfDay.cs
// Copyright Karel Kroeze, -2020

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MusicManager {
    public class Column_TimeOfDay: SongTableColumn {
        private TimeOfDay _filterTime = TimeOfDay.Any;

        public Column_TimeOfDay(int width) : base(width) {
        }

        public TimeOfDay FilterTime {
            get => _filterTime;
            set {
                if (value != _filterTime) {
                    _filterTime = value;
                    Window_MusicManager.SetDirty();
                }
            }
        }

        public override int Compare(SongDef a, SongDef b) {
            return a.allowedTimeOfDay.CompareTo(b.allowedTimeOfDay);
        }

        public override void DrawCell(Rect canvas, SongDef song) {
            base.DrawCell(canvas, song);
            DrawTimeOfDayIcon(new Rect(Vector2.zero, IconSize).CenteredIn(canvas), song.allowedTimeOfDay);
            TooltipHandler.TipRegion(canvas, () => Tooltip(song), song.GetHashCode() ^ canvas.GetHashCode());
            if (Utilities.DrawButton(canvas)) {
                song.allowedTimeOfDay = (TimeOfDay) (((int) song.allowedTimeOfDay + 1) % 3);
            }
        }

        public string Tooltip(SongDef song) {
            return I18n.AllowedTimeOfDay(song.allowedTimeOfDay);
        }

        public override void DrawHeader(Rect canvas, List<SongDef> songs) {
            Rect iconRect = new Rect( Vector2.zero, IconSize ).CenteredIn( canvas );
            // mass assign
            if (Utilities.Shift()) {
                TimeOfDay time = songs.All(s => s.allowedTimeOfDay == TimeOfDay.Day)
                    ? TimeOfDay.Day
                    : songs.All(s => s.allowedTimeOfDay == TimeOfDay.Night) ? TimeOfDay.Night : TimeOfDay.Any;
                DrawTimeOfDayIcon(iconRect, time);
                if (Utilities.DrawButton(canvas)) {
                    time = (TimeOfDay) (((int) time + 1) % 3); // cast to int, limit to 0, 1, 2, cast back to time.
                    foreach (SongDef song in songs) {
                        song.SetAllowed(time);
                    }
                }
            } else if (Utilities.Alt()) {
                DrawTimeOfDayIcon(iconRect, FilterTime);
                if (Utilities.DrawButton(canvas)) {
                    FilterTime = (TimeOfDay) (((int) FilterTime + 1) % 3);
                }
            } else {
                if (Utilities.DrawButton(canvas, Resources.Night, IconSize.x)) {
                    Window_MusicManager.SortBy = this;
                }
            }

            DrawOverlay(canvas);
            TooltipHandler.TipRegion(canvas, () => HeaderTooltip, GetHashCode());
        }

        public override bool Filter(SongDef song) {
            return FilterTime == TimeOfDay.Any || song.allowedTimeOfDay == TimeOfDay.Any || FilterTime == song.allowedTimeOfDay;
        }

        public override bool Filtered => FilterTime != TimeOfDay.Any;

        private void DrawTimeOfDayIcon(Rect canvas, TimeOfDay time) {
            switch (time) {
                case TimeOfDay.Any:
                    GUI.DrawTextureWithTexCoords(canvas.LeftHalf(), Resources.Day, new Rect(0, 0, .5f, 1));
                    GUI.DrawTextureWithTexCoords(canvas.RightHalf(), Resources.Night, new Rect(.5f, 0, .5f, 1));
                    break;
                case TimeOfDay.Day:
                    GUI.DrawTexture(canvas, Resources.Day);
                    break;
                case TimeOfDay.Night:
                    GUI.DrawTexture(canvas, Resources.Night);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public override string HeaderTooltip => $"{I18n.TimeOfDayColumn}\n\n{I18n.TimeOfDayColumn_Tip}";
    }
}
