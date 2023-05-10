// Resources.cs
// Copyright Karel Kroeze, 2020-2020

using UnityEngine;
using Verse;

namespace MusicManager {
    [StaticConstructorOnStartup]
    public class Resources {
        public static Texture2D DarkBackgroundColor =
            SolidColorMaterials.NewSolidColorTexture( new Color( 0, 0, 0, .8f ) );

        public static Texture2D List  = ContentFinder<Texture2D>.Get( "UI/Icons/list" );
        public static Texture2D Next  = ContentFinder<Texture2D>.Get( "UI/Icons/next" );
        public static Texture2D Pause = ContentFinder<Texture2D>.Get( "UI/Icons/pause" );

        public static Texture2D Play     = ContentFinder<Texture2D>.Get( "UI/Icons/play" );
        public static Texture2D Previous = ContentFinder<Texture2D>.Get( "UI/Icons/previous" );

        public static Texture2D SeekBackgroundTexture =
            SolidColorMaterials.NewSolidColorTexture( 0, 0, 0, .3f );

        public static Texture2D SeekForegroundTexture = SolidColorMaterials.NewSolidColorTexture( Color.white );

        public static Texture2D SlightlyDarkBackgroundColor =
            SolidColorMaterials.NewSolidColorTexture( new Color( 0, 0, 0, .3f ) );

        public static Texture2D Stop = ContentFinder<Texture2D>.Get( "UI/Icons/stop" ),
                                Cog  = ContentFinder<Texture2D>.Get( "UI/Icons/cog" );

        public static Texture2D SummerOn  = ContentFinder<Texture2D>.Get( "UI/Icons/summer-on" ),
                                SummerOff = ContentFinder<Texture2D>.Get( "UI/Icons/summer-off" ),
                                WinterOn  = ContentFinder<Texture2D>.Get( "UI/Icons/winter-on" ),
                                WinterOff = ContentFinder<Texture2D>.Get( "UI/Icons/winter-off" ),
                                SpringOn  = ContentFinder<Texture2D>.Get( "UI/Icons/spring-on" ),
                                SpringOff = ContentFinder<Texture2D>.Get( "UI/Icons/spring-off" ),
                                FallOn    = ContentFinder<Texture2D>.Get( "UI/Icons/fall-on" ),
                                FallOff   = ContentFinder<Texture2D>.Get( "UI/Icons/fall-off" ),
                                Explosion = ContentFinder<Texture2D>.Get( "UI/Icons/explosion" ),
                                Dove      = ContentFinder<Texture2D>.Get( "UI/Icons/dove" ),
                                Day       = ContentFinder<Texture2D>.Get( "UI/Icons/day" ),
                                Night     = ContentFinder<Texture2D>.Get( "UI/Icons/night" ),
                                Enabled     = ContentFinder<Texture2D>.Get( "UI/Icons/enabled" ),
                                Disabled     = ContentFinder<Texture2D>.Get( "UI/Icons/disabled" ),
                                ArrowDown = ContentFinder<Texture2D>.Get( "UI/Icons/arrow-down" ),
                                ArrowUp   = ContentFinder<Texture2D>.Get( "UI/Icons/arrow-up" ),
                                Funnel    = ContentFinder<Texture2D>.Get( "UI/Icons/funnel" ),
                                PlayXL    = ContentFinder<Texture2D>.Get( "UI/Icons/play-xl" ),
                                PauseXL   = ContentFinder<Texture2D>.Get( "UI/Icons/pause-xl" );


    }
}
