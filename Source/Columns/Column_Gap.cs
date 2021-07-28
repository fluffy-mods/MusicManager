// Column_Gap.cs
// Copyright Karel Kroeze, -2020

using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MusicManager {
    public class Column_Gap: SongTableColumn {
        public Column_Gap(int width) : base(width) {
        }

        public override int Compare(SongDef a, SongDef b) {
            return 0;
        }

        public override void DrawHeader(Rect canvas, List<SongDef> songs) {
        }

        public override bool Filter(SongDef song) {
            return true;
        }
        public override bool Filtered => false;
        public override string HeaderTooltip => null;
    }
}
