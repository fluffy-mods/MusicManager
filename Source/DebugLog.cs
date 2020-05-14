using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicManager
{
	static class Log
	{
		[System.Diagnostics.Conditional("DEBUG")]
		public static void Debug( string msg ){
			Message( msg );
		}

		public static void Message(string msg )
		{
			Verse.Log.Message( $"MusicManager :: {msg}");
		}
	}
}
