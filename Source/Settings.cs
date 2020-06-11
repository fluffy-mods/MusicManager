using UnityEngine;
using Verse;

namespace MusicManager
{
	public class Settings : ModSettings
	{
		public void DoWindowContents(Rect canvas)
		{
			var options = new Listing_Standard();
			options.Begin(canvas);
			options.End();
		}
		
		public override void ExposeData()
		{
		}
	}
}