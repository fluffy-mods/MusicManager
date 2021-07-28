using UnityEngine;
using Verse;

namespace MusicManager {
    public enum WidgetAnchor {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class Settings: ModSettings {
        public bool         Locked;
        public Vector2      WidgetPosition    = new Vector2( GameComp_MusicManager.Size.x, 0 );
        public FloatRange   SongIntervalPeace = new FloatRange( 85f, 105f );
        public FloatRange   SongIntervalWar = new FloatRange( 2,5 );
        public WidgetAnchor WidgetAnchor      = WidgetAnchor.TopRight;

        public void DoWindowContents(Rect canvas) {
            Listing_Standard options = new Listing_Standard();
            options.Begin(canvas);
            options.CheckboxLabeled(I18n.LockWidgetPosition, ref Locked, I18n.LockWidgetPositionTooltip);
            options.Gap();
            _ = options.Label(I18n.SongIntervalPeace, tooltip: I18n.SongIntervalPeace_Tip);
            Rect peaceIntervalRect = options.GetRect( Text.LineHeight );
            Widgets.FloatRange(peaceIntervalRect, 1, ref SongIntervalPeace, 0, 360,
                                valueStyle: ToStringStyle.Integer);

            _ = options.Label(I18n.SongIntervalWar, tooltip: I18n.SongIntervalWar_Tip);
            Rect warIntervalRect = options.GetRect( Text.LineHeight );
            Widgets.FloatRange(warIntervalRect, 2, ref SongIntervalWar, 0, 360,
                                valueStyle: ToStringStyle.Integer);
            options.Gap();
            if (options.ButtonText(I18n.ResetCustomMetaData)) {
                MusicManager.SongDatabase.ResetCustomMetaData();
            }

            options.End();
        }

        public override void ExposeData() {
            Scribe_Values.Look(ref Locked, "locked", true);
            Scribe_Values.Look(ref WidgetPosition, "position", new Vector2(GameComp_MusicManager.Size.x, 0));
            Scribe_Values.Look(ref WidgetAnchor, "anchor", WidgetAnchor.TopRight);
            Scribe_Values.Look(ref SongIntervalPeace, "SongIntervalPeace", new FloatRange(85, 105));
            Scribe_Values.Look(ref SongIntervalWar, "SongIntervalWar", new FloatRange(2, 5));
        }
    }
}
