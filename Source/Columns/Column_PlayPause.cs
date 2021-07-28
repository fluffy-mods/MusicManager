// Column_PlayPause.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MusicManager {
    public class Column_PlayPause: SongTableColumn {
        private readonly float _iconSize;
        public Column_PlayPause(int width) : base(width) {
            _iconSize = width * 2 / 3f;
        }

        public override int Compare(SongDef a, SongDef b) {
            return 0;
        }

        public override void DrawCell(Rect canvas, SongDef song) {
            base.DrawCell(canvas, song);
            Utilities.DrawPlayButton(canvas, song, _iconSize);
        }

        public override void DrawHeader(Rect canvas, List<SongDef> songs) {
        }

        public override Color Color(SongDef song) {
            return song == MusicManager.CurrentSong ? GenUI.MouseoverColor : UnityEngine.Color.white;
        }

        public override bool Filter(SongDef song) {
            return true;
        }

        public override bool Filtered { get; }
        public override string HeaderTooltip { get; }
    }
}
