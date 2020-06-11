// MusicManager.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace MusicManager
{
    public class Mod : Verse.Mod
    {
        public Mod( ModContentPack content ) : base( content )
        {
            // initialize settings
            Settings = GetSettings<Settings>();


#if DEBUG
            Harmony.DEBUG = true;
#endif
            var harmony = new Harmony( "Fluffy.MusicManager" );
            harmony.PatchAll();
        }

        public static Settings Settings { get; private set; }

        public override void DoSettingsWindowContents( Rect inRect )
        {
            base.DoSettingsWindowContents( inRect );
            GetSettings<Settings>().DoWindowContents( inRect );
        }

        public override string SettingsCategory()
        {
            return "Music Manager";
        }
    }

    public class MusicManager : GameComponent
    {
        public const int Margin        = 6;
        public const int RowHeight     = 24;
        public const int SeekBarHeight = 5;
        public const int WidgetWidth   = 150;

        private static readonly MethodInfo _appropriateNow_MI =
            AccessTools.Method( typeof( MusicManagerPlay ), "AppropriateNow" );

        private static readonly FieldInfo audioSource_FieldInfo =
            AccessTools.Field( typeof( MusicManagerPlay ), "audioSource" );

        private static readonly FieldInfo lastStartedSong_FieldInfo =
            AccessTools.Field( typeof( MusicManagerPlay ), "lastStartedSong" );

        public MusicManager()
        {
            // scribe
        }

        public MusicManager( Game game )
        {
        }

        public static AudioSource AudioSource => audioSource_FieldInfo.GetValue( Find.MusicManagerPlay ) as AudioSource;
        public static SongDef     CurrentSong => lastStartedSong_FieldInfo.GetValue( Find.MusicManagerPlay ) as SongDef;

        public static bool isPaused { get; private set; }

        public static List<SongDef> Songs =>
            DefDatabase<SongDef>.AllDefsListForReading.Where( AppropriateNow ).ToList();

        public static bool AppropriateNow( SongDef song )
        {
            return (bool) _appropriateNow_MI.Invoke( Find.MusicManagerPlay, new object[] {song} );
        }

        public static void DrawSeekBar( Rect canvas )
        {
            // todo: figure out why game starts next song on long click/drag.

            var progress = AudioSource.time / AudioSource.clip.length;
            GUI.DrawTexture( canvas, Resources.SeekBackgroundTexture );
            GUI.DrawTexture( canvas.LeftPart( progress ), Resources.SeekForegroundTexture );

            if ( Input.GetMouseButton( 0 ) && Mouse.IsOver( canvas ) )
            {
                var seekPct = ( Event.current.mousePosition.x - canvas.xMin ) / canvas.width;
                AudioSource.time = seekPct * AudioSource.clip.length;
                AudioSource.Pause();
            }
            else if ( !isPaused && !AudioSource.isPlaying )
            {
                AudioSource.UnPause();
            }
        }

        public static void Next()
        {
            Log.Debug( "next" );
            Play();
        }

        public static void Pause()
        {
            Log.Debug( "pause" );
            AudioSource.Pause();
            isPaused = true;
        }

        public static void Play( [CanBeNull] SongDef song = null )
        {
            Log.Debug( "play" );
            Find.MusicManagerPlay.ForceStartSong( song, false );
            AudioSource.time = 0;

            Messages.Message( $"Now playing: {CurrentSong?.Name() ?? "NONE"}",
                              MessageTypeDefOf.SilentInput );
        }

        public static void Previous()
        {
            Log.Debug( "previous" );
            if ( AudioSource.time < 5 )
                Play();
            else
                AudioSource.time = 0;
        }

        public static void Resume()
        {
            Log.Debug( "resume" );
            AudioSource.UnPause();
            isPaused = false;
        }

        public override void GameComponentOnGUI()
        {
            base.GameComponentOnGUI();


            // set up rects
            var startX        = UI.screenWidth - WidgetWidth - Margin;
            var buttonsRect   = new Rect( startX, Margin, WidgetWidth, RowHeight );
            var songTitleRect = new Rect( startX, buttonsRect.yMax, WidgetWidth, RowHeight );
            var seekBarRect   = new Rect( startX, songTitleRect.yMax, WidgetWidth, SeekBarHeight );
            var songTimeRect  = new Rect( startX, seekBarRect.yMax, WidgetWidth, RowHeight );

            DrawButtons( buttonsRect );
            if ( CurrentSong == null ) return;

            Widgets.Label( songTitleRect, CurrentSong.Name() );
            DrawSeekBar( seekBarRect );
            DrawSongTime( songTimeRect );
        }

        private static void DrawSongTime( Rect songTimeRect )
        {
            Text.Font   = GameFont.Tiny;
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label( songTimeRect, AudioSource.time.ToStringTime() );
            Text.Anchor = TextAnchor.UpperRight;
            Widgets.Label( songTimeRect, AudioSource.clip.length.ToStringTime() );
            Text.Font   = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawButton( Rect canvas, Texture2D icon, Action clickAction )
        {
            var iconRect = new Rect( 0, 0, 24, 24 )
                          .CenteredOnXIn( canvas )
                          .CenteredOnYIn( canvas );
            GUI.DrawTexture( iconRect, icon );
            Widgets.DrawHighlightIfMouseover( canvas );
            if ( Widgets.ButtonInvisible( canvas ) )
                clickAction.Invoke();
        }

        private void DrawButtons( Rect canvas )
        {
            var buttonRect = canvas.RightPart( 1 / 4f );
            DrawButton( buttonRect, Resources.List, () =>
            {
                var options = Songs.Select( s => new FloatMenuOption( s.Name(), () => Play( s ) ) ).ToList();
                Find.WindowStack.Add( new FloatMenu( options ) );
            } );
            buttonRect.x -= buttonRect.width;
            DrawButton( buttonRect, Resources.Next, Next );
            buttonRect.x -= buttonRect.width;
            if ( AudioSource.isPlaying )
            {
                DrawButton( buttonRect, Resources.Pause, Pause );
            }
            else
            {
                if ( isPaused )
                    DrawButton( buttonRect, Resources.Play, Resume );
                else
                    DrawButton( buttonRect, Resources.Play, () => Play() );
            }
            buttonRect.x -= buttonRect.width;
            DrawButton( buttonRect, Resources.Previous, Previous );
        }
    }
}