// Utilities.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RimWorld;
using UnityEngine;
using Verse;

namespace MusicManager
{
    public static class Utilities
    {
        private static readonly Regex _songPathNameRegex = new Regex( @"[^\/]+$" );

        public static List<Season> NewAllSeasonsList =>
            new List<Season>
            {
                Season.Spring,
                Season.Summer,
                Season.Fall,
                Season.Winter,
                Season.PermanentSummer,
                Season.PermanentWinter
            };

        public static bool Allowed( this SongDef song, Season season )
        {
            return song.allowedSeasons.Allowed( season );
        }

        public static bool Allowed( this List<Season> seasons, Season season )
        {
            // TODO: Leave Allowed as a simple contains check for all seasons?
            if ( seasons.NullOrEmpty() || season == Season.Undefined )
                return true;
            switch ( season )
            {
                case Season.Spring:
                case Season.Fall:
                    return seasons.Contains( season );
                case Season.Summer:
                case Season.PermanentSummer:
                    return seasons.Contains( Season.Summer ) || seasons.Contains( Season.PermanentSummer );
                case Season.Winter:
                case Season.PermanentWinter:
                    return seasons.Contains( Season.Winter ) || seasons.Contains( Season.PermanentWinter );
                default:
                    throw new ArgumentOutOfRangeException( nameof( season ), season, null );
            }
        }

        public static bool Allowed( this SongDef song, TimeOfDay time )
        {
            return song.allowedTimeOfDay == TimeOfDay.Any || time == TimeOfDay.Any || time == song.allowedTimeOfDay;
        }

        public static bool Alt()
        {
            return Input.GetKey( KeyCode.LeftAlt ) || Input.GetKey( KeyCode.AltGr ) || Input.GetKey( KeyCode.RightAlt );
        }

        public static string Bold( this string msg )
        {
            return $"<b>{msg}</b>";
        }

        public static Rect CenteredIn( this Rect canvas, Rect other )
        {
            canvas.center = other.center;
            return canvas;
        }

        public static bool DrawButton( Rect canvas, Texture icon, float? iconSize = null )
        {
            if ( !iconSize.HasValue ) iconSize = (int) Mathf.Min( canvas.width, canvas.height );
            var iconRect = new Rect( 0, 0, iconSize.Value, iconSize.Value )
                          .CenteredOnXIn( canvas ).CenteredOnYIn( canvas );
            GUI.DrawTexture( iconRect, icon );
            return DrawButton( canvas );
        }

        public static bool DrawButton( Rect canvas )
        {
            Widgets.DrawHighlightIfMouseover( canvas );
            return Widgets.ButtonInvisible( canvas );
        }

        public static void DrawButton( Rect canvas, Texture icon, Action action, float? iconSize = null )
        {
            if ( DrawButton( canvas, icon, iconSize ) ) action.Invoke();
        }

        public static void DrawSeekBar( Rect canvas )
        {
            var progress = MusicManager.AudioSource.time / MusicManager.AudioSource.clip.length;
            GUI.DrawTexture( canvas, Resources.SeekBackgroundTexture );
            GUI.DrawTexture( canvas.LeftPart( progress ), Resources.SeekForegroundTexture );

            Widgets.DrawHighlightIfMouseover( canvas );
            if ( Event.current.type != EventType.Repaint
              && Input.GetMouseButton( 0 )
              && Mouse.IsOver( canvas ) )
            {
                var seekPct = Mathf.Clamp01( ( Event.current.mousePosition.x - canvas.xMin ) / canvas.width );
                MusicManager.AudioSource.time = seekPct * MusicManager.AudioSource.clip.length;
                MusicManager.AudioSource.Pause();

                // stop event bubbling up.
                Event.current.Use();
            }
            else if ( !MusicManager.isPaused && !MusicManager.AudioSource.isPlaying )
            {
                MusicManager.AudioSource.UnPause();
            }
        }

        public static Rect HorizontalMidPartPixels( this Rect canvas, float width )
        {
            var margin = ( canvas.width - width ) / 2;
            return HorizontalMidPartPixels( canvas, margin, margin );
        }

        public static Rect HorizontalMidPartPixels( this Rect canvas, float leftMargin, float rightMargin )
        {
            return canvas.RightPartPixels( canvas.width             - leftMargin )
                         .LeftPartPixels( canvas.width - leftMargin - rightMargin );
        }

        public static string Italic( this string msg )
        {
            return $"<i>{msg}</i>";
        }

        public static string Name( this SongDef def )
        {
            if ( !def.label.NullOrEmpty() )
                return def.label.CapitalizeFirst();
            var match = _songPathNameRegex.Match( def.clipPath );
            if ( match.Success )
                return match.Groups[0].Value.Replace( '_', ' ' );
            return def.clipPath;
        }

        public static List<Season> SetAllowed( this List<Season> seasons, Season season, bool allowed )
        {
            if ( seasons == null )
                seasons = NewAllSeasonsList;

            if ( allowed && !seasons.Contains( season ) )
            {
                seasons.Add( season );
                if ( season == Season.Summer && !seasons.Contains( Season.PermanentSummer ) )
                    seasons.Add( Season.PermanentSummer );
                if ( season == Season.Winter && !seasons.Contains( Season.PermanentWinter) )
                    seasons.Add( Season.PermanentWinter );
            }

            if ( !allowed )
            {
                if (seasons.Contains( season ))
                    seasons.Remove( season );
                if ( season == Season.Summer && seasons.Contains( Season.PermanentSummer ) )
                    seasons.Remove( Season.PermanentSummer );
                if ( season == Season.Winter && seasons.Contains( Season.PermanentWinter ) )
                    seasons.Remove( Season.PermanentWinter );
            }

            if ( !seasons.Any() )
                seasons = NewAllSeasonsList;

            Window_MusicManager.SetDirty();
            return seasons;
        }

        public static void SetAllowed( this SongDef song, Season season, bool allowed )
        {
            song.allowedSeasons = song.allowedSeasons.SetAllowed( season, allowed );
        }

        public static void SetAllowed( this SongDef song, TimeOfDay time )
        {
            if ( song.allowedTimeOfDay == time ) return;
            song.allowedTimeOfDay = time;
            Window_MusicManager.SetDirty();
        }

        public static bool Shift()
        {
            return Input.GetKey( KeyCode.LeftShift ) || Input.GetKey( KeyCode.RightShift );
        }

        public static string ToStringTime( this float time )
        {
            var minutes = Mathf.FloorToInt( time / 60 );
            var seconds = Mathf.RoundToInt( time % 60 );
            return $"{minutes}:{seconds:D2}";
        }

        public static Rect VerticalMidPartPixels( this Rect canvas, float height )
        {
            var margin = ( canvas.height - height ) / 2;
            return VerticalMidPartPixels( canvas, margin, margin );
        }

        public static Rect VerticalMidPartPixels( this Rect canvas, float topMargin, float bottomMargin )
        {
            return canvas.TopPartPixels( canvas.height                - bottomMargin )
                         .BottomPartPixels( canvas.height - topMargin - bottomMargin );
        }

        public static Rect WidthContractedBy( this Rect canvas, float margin )
        {
            canvas.xMin += margin;
            canvas.xMax -= margin;
            return canvas;
        }
    }
}