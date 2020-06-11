// Resources.cs
// Copyright Karel Kroeze, 2020-2020

using UnityEngine;
using Verse;

namespace MusicManager
{
    [StaticConstructorOnStartup]
    public class Resources
    {
        public static Texture2D SeekBackgroundTexture =
            SolidColorMaterials.NewSolidColorTexture( 0,0,0, .3f );
        public static Texture2D SeekForegroundTexture = SolidColorMaterials.NewSolidColorTexture( Color.white );

        public static Texture2D Play = ContentFinder<Texture2D>.Get( "UI/Icons/play" );
        public static Texture2D Pause = ContentFinder<Texture2D>.Get( "UI/Icons/pause" );
        public static Texture2D Stop = ContentFinder<Texture2D>.Get( "UI/Icons/stop" );
        public static Texture2D Previous = ContentFinder<Texture2D>.Get( "UI/Icons/previous" );
        public static Texture2D Next = ContentFinder<Texture2D>.Get( "UI/Icons/next" );
        public static Texture2D List = ContentFinder<Texture2D>.Get( "UI/Icons/list" );
    }
}