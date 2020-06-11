// Utilities.cs
// Copyright Karel Kroeze, 2020-2020

using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace MusicManager
{
    public static class Utilities
    {
        private static Regex _songPathNameRegex = new Regex( @"[^\/]+$");
        public static string Name( this SongDef def )
        {
            if ( !def.label.NullOrEmpty() )
                return def.label.CapitalizeFirst();
            var match = _songPathNameRegex.Match( def.clipPath );
            if (match.Success)
                return match.Groups[0].Value.Replace( '_', ' ' );
            return def.clipPath;
        }

        public static string ToStringTime( this float time )
        {
            var minutes = Mathf.FloorToInt( time / 60 );
            var seconds = Mathf.RoundToInt( time % 60 );
            return $"{minutes}:{seconds:D2}";
        }
    }
}