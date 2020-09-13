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
    public class MusicManager : Mod
    {
        private static readonly MethodInfo _appropriateNow_MI =
            AccessTools.Method( typeof( MusicManagerPlay ), "AppropriateNow" );

        private static readonly FieldInfo audioSource_FieldInfo =
            AccessTools.Field( typeof( MusicManagerPlay ), "audioSource" );

        private static readonly FieldInfo lastStartedSong_FieldInfo =
            AccessTools.Field( typeof( MusicManagerPlay ), "lastStartedSong" );

        public MusicManager( ModContentPack content ) : base( content )
        {
            // initialize settings
            Settings = GetSettings<Settings>();
            ApplySongInterval();
            Instance = this;
            SongDatabase = new SongDatabase();
            LongEventHandler.ExecuteWhenFinished(SongDatabase.Initialize);

#if DEBUG
            Harmony.DEBUG = true;
#endif
            var harmony = new Harmony( "Fluffy.GameComp_MusicManager" );
            harmony.PatchAll();
        }

        public static List<SongDef> AppropriateSongs => Songs.Where( AppropriateNow ).ToList();

        public static SongDatabase SongDatabase;

        public static AudioSource AudioSource =>
            audioSource_FieldInfo.GetValue( Find.MusicManagerPlay ) as AudioSource;

        public static SongDef CurrentSong
        {
            get
            {
                if ( !AudioSource.isPlaying && !isPaused ) return null;
                return lastStartedSong_FieldInfo.GetValue( Find.MusicManagerPlay ) as SongDef;
            }
        }

        public static MusicManager Instance { get; private set; }

        public static bool isPaused { get; private set; }

        public static Settings Settings { get; private set; }

        public static List<SongDef> Songs   => DefDatabase<SongDef>.AllDefsListForReading;

        private static bool _seeking;
        public static bool Seeking
        {
            get => _seeking;
            set
            {
                if ( value )
                {
                    Pause();
                    LastSeek = Time.time;
                }

                _seeking = value;
            }
        }

        private static float _lastSeek;
        public static float LastSeek
        {
            get => Time.time - _lastSeek;
            set => _lastSeek = value;
        }


        public static bool AppropriateNow( SongDef song )
        {
            return (bool) _appropriateNow_MI.Invoke( Find.MusicManagerPlay, new object[] {song} );
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

        public override void DoSettingsWindowContents( Rect inRect )
        {
            base.DoSettingsWindowContents( inRect );
            GetSettings<Settings>().DoWindowContents( inRect );
        }

        public override string SettingsCategory()
        {
            return I18n.MusicManager;
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            ApplySongInterval();
        }

        public void ApplySongInterval()
        {
            typeof( MusicManagerPlay ).GetField( "SongIntervalTension", BindingFlags.NonPublic | BindingFlags.Static )
                                      .SetValue( null, Settings.SongIntervalWar );
            typeof( MusicManagerPlay ).GetField( "SongIntervalRelax", BindingFlags.NonPublic | BindingFlags.Static )
                                      .SetValue( null, Settings.SongIntervalPeace );

            if ( Current.Root is Root_Play root )
            {
                typeof( MusicManagerPlay )
                   .GetField( "nextSongStartTime", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( root.musicManagerPlay, 0 );
            }

            Log.Debug( "interval applied(?)");
        }
    }
}