using UnityEngine;
using Verse;

namespace MusicManager
{
    public enum WidgetAnchor
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

	public class Settings : ModSettings
	{
        public bool Locked;
        public Vector2 WidgetPosition = new Vector2( GameComp_MusicManager.Size.x, 0 );
        public WidgetAnchor WidgetAnchor = WidgetAnchor.TopRight;

        public void DoWindowContents(Rect canvas)
		{
			var options = new Listing_Standard();
			options.Begin(canvas);

			options.CheckboxLabeled( I18n.LockWidgetPosition, ref Locked, I18n.LockWidgetPositionTooltip );

			options.End();
		}
		
		public override void ExposeData()
        {
            Scribe_Values.Look( ref Locked, "locked", true );
            Scribe_Values.Look( ref WidgetPosition, "position", new Vector2( GameComp_MusicManager.Size.x, 0 ) );
            Scribe_Values.Look( ref WidgetAnchor, "anchor", WidgetAnchor.TopRight );
        }
	}
}