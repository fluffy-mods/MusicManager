// Column_Seasons.cs
// Copyright Karel Kroeze, -2020

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace MusicManager
{
    public class Column_Seasons : SongTableColumn
    {
        public Column_Seasons( int width ) : base( width )
        {
        }

        private List<Season> _filterSeasons = Utilities.NewAllSeasonsList;

        public List<Season> FilterSeasons
        {
            get => _filterSeasons;
            set
            {
                if ( value.NullOrEmpty() )
                {
                    _filterSeasons = Utilities.NewAllSeasonsList;
                    return;
                }
                if ( !value.Except( _filterSeasons ).Any() && !_filterSeasons.Except( value ).Any() ) return;
                
                _filterSeasons = value;
                _filtered = Utilities.NewAllSeasonsList.Except( FilterSeasons ).Any();
                Window_MusicManager.SetDirty();
            }
        }

        public override int Compare( SongDef a, SongDef b )
        {
            if ( a?.allowedSeasons == null && b?.allowedSeasons == null ) return 0;
            if ( a?.allowedSeasons == null ) return -1;
            if ( b?.allowedSeasons == null ) return 1;
            return a.allowedSeasons.Sum( s => (int) s + 10 ) - b.allowedSeasons.Sum( s => (int) s + 10 );
        }

        public override void DrawCell( Rect canvas, SongDef song )
        {

            base.DrawCell( canvas, song );
            var cell = canvas.RightPartPixels( canvas.width / 4 );
            TooltipHandler.TipRegion( cell, () => Tooltip( Season.Winter, song ), song.GetHashCode() ^ cell.GetHashCode() );
            DrawSongSeason( ref cell, ref song.allowedSeasons, Season.Winter, Resources.WinterOn, Resources.WinterOff );
            TooltipHandler.TipRegion( cell, () => Tooltip( Season.Fall, song ),
                                      song.GetHashCode() ^ cell.GetHashCode() );
            DrawSongSeason( ref cell, ref song.allowedSeasons, Season.Fall, Resources.FallOn, Resources.FallOff );
            TooltipHandler.TipRegion(cell, () => Tooltip(Season.Summer, song), song.GetHashCode() ^ cell.GetHashCode());
            DrawSongSeason( ref cell, ref song.allowedSeasons, Season.Summer, Resources.SummerOn, Resources.SummerOff );
            TooltipHandler.TipRegion( cell, () => Tooltip( Season.Spring, song ),
                                      song.GetHashCode() ^ cell.GetHashCode() );
            DrawSongSeason( ref cell, ref song.allowedSeasons, Season.Spring, Resources.SpringOn, Resources.SpringOff );
        }

        public string Tooltip( Season season, SongDef song )
        {
            return $"{I18n.Season( Season.Winter )}\n\n{I18n.AllowedSeasons( song.allowedSeasons )}";
        }

        public override void DrawHeader( Rect canvas, List<SongDef> songs )
        {
            var cell = canvas.RightPartPixels( canvas.width / 4 );
            // mass asignments
            if ( Utilities.Shift() )
            {
                DrawMassAssignSeason( ref cell, songs, Season.Winter, Resources.WinterOn, Resources.WinterOff );
                DrawMassAssignSeason( ref cell, songs, Season.Fall, Resources.FallOn, Resources.FallOff );
                DrawMassAssignSeason( ref cell, songs, Season.Summer, Resources.SummerOn, Resources.SummerOff );
                DrawMassAssignSeason( ref cell, songs, Season.Spring, Resources.SpringOn, Resources.SpringOff );
            }
            else if ( Utilities.Alt() )
            {
                // filtering
                var seasons = new List<Season>( FilterSeasons );
                DrawSongSeason( ref cell, ref seasons, Season.Winter, Resources.WinterOn, Resources.WinterOff );
                DrawSongSeason( ref cell, ref seasons, Season.Fall, Resources.FallOn, Resources.FallOff );
                DrawSongSeason( ref cell, ref seasons, Season.Summer, Resources.SummerOn, Resources.SummerOff );
                DrawSongSeason( ref cell, ref seasons, Season.Spring, Resources.SpringOn, Resources.SpringOff );
                FilterSeasons = seasons;
            }
            else
            {
                // sorting
                var iconRect = new Rect( Vector2.zero, IconSize ).CenteredIn( cell );
                GUI.DrawTexture(iconRect, Resources.WinterOn );
                iconRect.x -= cell.width;
                GUI.DrawTexture( iconRect, Resources.FallOn );
                iconRect.x -= cell.width;
                GUI.DrawTexture( iconRect, Resources.SummerOn );
                iconRect.x -= cell.width;
                GUI.DrawTexture( iconRect, Resources.SpringOn );

                if ( Utilities.DrawButton( canvas ) ) // event should have been used if it triggered either mass or filter.
                    Window_MusicManager.SortBy = this;
            }

            DrawOverlay( canvas );
            TooltipHandler.TipRegion( canvas, () => HeaderTooltip, GetHashCode() );
        }

        public void DrawMassAssignSeason( ref Rect cell, List<SongDef> songs, Season season, Texture2D iconOn,
                                          Texture2D iconOff )
        {
            if ( songs.All( s => s.Allowed( season ) ) )
            {
                if ( Utilities.DrawButton( cell, iconOn, IconSize.x ) )
                    foreach ( var song in songs )
                        song.SetAllowed( season, false );
            }
            else if ( songs.Any( s => s.Allowed( season ) ) )
            {
                var iconRect = new Rect( Vector2.zero, IconSize ).CenteredIn( cell );
                GUI.DrawTextureWithTexCoords( iconRect.LeftHalf(), iconOff, new Rect( 0, 0, .5f, 1 ) );
                GUI.DrawTextureWithTexCoords( iconRect.RightHalf(), iconOn, new Rect( .5f, 0, .5f, 1 ) );
                if ( Utilities.DrawButton( cell ) )
                    foreach ( var song in songs.Where( s => !s.Allowed( season ) ) )
                        song.SetAllowed( season, true );
            }
            else
            {
                if ( Utilities.DrawButton( cell, iconOff, IconSize.x ) )
                    foreach ( var song in songs )
                        song.SetAllowed( season, true );
            }

            cell.x -= cell.width;
        }

        public void DrawSongSeason( ref Rect cell, ref List<Season> seasons, Season season, Texture2D iconOn,
                                    Texture2D iconOff )
        {
            var before = seasons.Allowed( season );
            var after  = before;

            if ( Utilities.DrawButton( cell, before ? iconOn : iconOff, IconSize.x ) )
                after = !after;

            if ( after != before )
                seasons = seasons.SetAllowed( season, after );

            cell.x -= cell.width;
        }

        public override bool Filter( SongDef song )
        {
            return song.allowedSeasons.NullOrEmpty() || song.allowedSeasons.Any( s => FilterSeasons.Contains( s ) );
        }

        private bool _filtered = false;
        public override bool Filtered => _filtered;

        public override string HeaderTooltip => $"{I18n.SeasonsColumn}\n\n{I18n.SeasonsColumn_Tip}";
    }
}