// I18n.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MusicManager
{
    public static class I18n
    {
        public static string LockWidgetPosition        = Translate( "LockWidgetPosition" );
        public static string LockWidgetPositionTooltip = Translate( "LockWidgetPosition.Tooltip" );
        public static string MusicManager              = "Fluffy.MusicManager".Translate();
        public static string NameColumn                = Translate( "Columns.Name.Label" );
        public static string NameColumn_Tip            = Translate( "Columns.Name.Tooltip" );
        public static string LengthColumn              = Translate( "Columns.Length.Label" );
        public static string LengthColumn_Tip          = Translate( "Columns.Length.Tooltip" );
        public static string TimeOfDayColumn           = Translate( "Columns.TimeOfDay.Label" );
        public static string TimeOfDayColumn_Tip       = Translate( "Columns.TimeOfDay.Tooltip" );
        public static string TenseColumn               = Translate( "Columns.Tense.Label" );
        public static string TenseColumn_Tip           = Translate( "Columns.Tense.Tooltip" );
        public static string SeasonsColumn             = Translate( "Columns.Seasons.Label" );
        public static string SeasonsColumn_Tip         = Translate( "Columns.Seasons.Tooltip" );
        public static string Filtered                  = Translate( "Filtered" );
        public static string Sorted                    = Translate( "Sorted" );
        public static string SongIntervalPeace         = Translate( "SongIntervalPeace" );
        public static string SongIntervalWar           = Translate( "SongIntervalWar" );
        public static string SongIntervalPeace_Tip     = Translate( "SongIntervalPeace.Tip" );
        public static string SongIntervalWar_Tip       = Translate( "SongIntervalWar.Tip" );

        private static string Key( string key )
        {
            return $"Fluffy.MusicManager.{key}";
        }

        private static string Translate( string key, params NamedArgument[] args )
        {
            return Key( key ).Translate( args ).Resolve();
        }

        public static string AllowedTimeOfDay( TimeOfDay tod )
        {
            return Translate( "AllowedTimeOfDay", TimeOfDay( tod ) );
        }

        public static string TimeOfDay( TimeOfDay tod )
        {
            return Translate( $"TimeOfDay.{tod}" );
        }

        public static string AllowedSeasons( List<Season> seasons )
        {
            var _seasons = seasons.NullOrEmpty() ? Utilities.NewAllSeasonsList : seasons;
            return Translate( "AllowedSeasons", _seasons.Select( Season ).ToCommaList( true ) );
        }

        public static string Season( Season season )
        {
            return Translate( $"Season.{season}" );
        }

        public static string AllowedTense( bool tense )
        {
            return Translate( "AllowedTense", tense ? War : Peace);
        }

        public static string War   = Translate( "War" );
        public static string Peace = Translate( "Peace" );
        public static string ResetCustomMetaData = Translate( "ResetCustomMetaData" );
    }
}