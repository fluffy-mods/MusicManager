// Window_MusicManager.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using static MusicManager.Resources;

namespace MusicManager
{
    public class Window_MusicManager : Window
    {
        public const  int         ColWidth   = 24;
        public const  int         RowHeight  = 24;
        public const  int         TimeWidth  = 80;
        public static Column_Name NameColumn = new Column_Name( 0 );
        public static Vector2     WindowSize = new Vector2( 800, 640 );

        public static List<SongTableColumn> Columns = new List<SongTableColumn>
        {
            new Column_Gap( _margin ),
            NameColumn,
            new Column_Length( TimeWidth ),
            new Column_Gap( _margin ),
            new Column_Tense( ColWidth ),
            new Column_Gap( _margin ),
            new Column_TimeOfDay( ColWidth ),
            new Column_Gap( _margin ),
            new Column_Seasons( ColWidth * 4 ),
            new Column_Gap( _margin )
        };


        private const int _margin = 6;

        private static List<SongDef> _filteredSongs = new List<SongDef>();

        private static SongTableColumn _sortBy;

        private static bool    dirty             = true;
        public         Vector2 songListScrollPos = Vector2.zero;

        public Window_MusicManager()
        {
            Instance = this;
        }

        public static List<SongDef> FilteredSongs
        {
            get
            {
                if ( dirty )
                {
                    // filter
                    _filteredSongs = MusicManager.Songs.Where( s => Columns.All( c => c.Filter( s ) ) ).ToList();

                    // sort
                    if ( _sortBy == null ) return _filteredSongs;
                    if ( SortDescending )
                        _filteredSongs.SortStable( ( a, b ) => _sortBy.Compare( b, a ) );
                    else
                        _filteredSongs.SortStable( _sortBy.Compare );
                }

                return _filteredSongs;
            }
        }

        public static Window_MusicManager Instance { get; private set; }

        public static SongTableColumn SortBy
        {
            get => _sortBy;
            set
            {
                if ( _sortBy == value )
                {
                    if ( SortDescending )
                    {
                        _sortBy        = null;
                        SortDescending = false;
                    }
                    else
                    {
                        SortDescending = true;
                    }
                }
                else
                {
                    _sortBy        = value;
                    SortDescending = false;
                }

                SetDirty();
            }
        }

        public static bool SortDescending { get; private set; }

        public override Vector2 InitialSize => WindowSize;

        protected override float Margin => _margin;

        public static void SetDirty()
        {
            dirty = true;
        }


        public override void DoWindowContents( Rect canvas )
        {
            canvas = canvas.ContractedBy( Margin );
            var titleRect    = canvas.TopPartPixels( 50 );
            var songListRect = canvas.VerticalMidPartPixels( 50, 50 );
            var detailsRect  = canvas.BottomPartPixels( 50 );

            DrawTitle( titleRect );
            DrawSongList( songListRect );
            DrawSongDetails( detailsRect, MusicManager.CurrentSong );
        }

        public void DrawPlayButton( Rect canvas, SongDef song, float size )
        {
            if ( MusicManager.AudioSource.clip == song.clip )
            {
                if ( MusicManager.isPaused )
                    Utilities.DrawButton( canvas, size > 24 ? PlayXL : Play, MusicManager.Resume, size );
                else
                    Utilities.DrawButton( canvas, size > 24 ? PauseXL : Pause, MusicManager.Pause, size );
            }
            else
            {
                Utilities.DrawButton( canvas, size > 24 ? PlayXL : Play, () => MusicManager.Play( song ), size );
            }
        }

        public void DrawSongDetails( Rect canvas, SongDef song )
        {
            /**
             * Song title      <|   >/||  |>       1:34 / 3:12
             * -----------------------
             */

            var topRect     = canvas.TopPartPixels( 42 );
            var titleRect   = topRect.LeftPartPixels( ( canvas.width - 100 ) / 2 );
            var buttonsRect = topRect.HorizontalMidPartPixels( titleRect.width, titleRect.width );
            var timeRect    = topRect.RightPartPixels( titleRect.width );
            var seekRect    = canvas.BottomPartPixels( 8 );

            // labels
            if ( song == null )
            {
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label( titleRect, "---" );
                Text.Anchor = TextAnchor.MiddleRight;
                Widgets.Label( timeRect, "-:-- / -:--" );
            }
            else
            {
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label( titleRect, song.Name() );
                Text.Anchor = TextAnchor.MiddleRight;
                Widgets.Label(
                    timeRect, $"{MusicManager.AudioSource.time.ToStringTime()} / {song.clip.length.ToStringTime()}" );

                Utilities.DrawButton( buttonsRect.LeftPartPixels( 24 ).VerticalMidPartPixels( 24 ), Previous,
                                      MusicManager.Previous );
                DrawPlayButton( buttonsRect.HorizontalMidPartPixels( 48 ), song, 42 );
                Utilities.DrawButton( buttonsRect.RightPartPixels( 24 ).VerticalMidPartPixels( 24 ), Next,
                                      MusicManager.Next );
                Utilities.DrawSeekBar( seekRect );
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }

        public void DrawSongList( Rect canvas )
        {
            var cur   = canvas.min;
            var songs = FilteredSongs;

            // draw headers
            foreach ( var column in Columns )
            {
                var rect = new Rect( cur.x, cur.y, column.Width, RowHeight );
                column.DrawHeader( rect, songs );
                cur.x += column.Width;
            }

            // draw cells
            var contentRect = new Rect( 0, 0, canvas.width, songs.Count * RowHeight );
            if ( contentRect.height > canvas.height - RowHeight ) contentRect.width -= 16;
            NameColumn.Width -= Columns.Sum( c => c.Width ) - (int) contentRect.width;
            Widgets.BeginScrollView( canvas.BottomPartPixels( canvas.height - RowHeight ), ref songListScrollPos,
                                     contentRect );
            GUI.DrawTexture( contentRect, SlightlyDarkBackgroundColor );
            var alternate = true;
            cur = Vector2.zero;
            foreach ( var song in songs )
            {
                if ( alternate )
                    GUI.DrawTexture( new Rect( cur.x, cur.y, contentRect.width, RowHeight ),
                                     SlightlyDarkBackgroundColor );
                alternate = !alternate;
                foreach ( var column in Columns )
                {
                    var rect = new Rect( cur.x, cur.y, column.Width, RowHeight );
                    column.DrawCell( rect, song );
                    cur.x += column.Width;
                }

                cur.x =  0;
                cur.y += RowHeight;
            }

            Widgets.EndScrollView();
        }

        public void DrawTitle( Rect canvas )
        {
            // giant label.
            Text.Font = GameFont.Medium;
            Widgets.Label( canvas, I18n.MusicManager );
            Text.Font = GameFont.Small;

            // options
            if ( Widgets.ButtonImage( canvas.RightPartPixels( 24 ).TopPartPixels( 24 ), Cog ) )
            {
                var dialog = new Dialog_ModSettings();
                Traverse.Create( dialog ).Field<Mod>( "selMod" ).Value = MusicManager.Instance;
                Find.WindowStack.Add( dialog );
            }
        }
    }
}