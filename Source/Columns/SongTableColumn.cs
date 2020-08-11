// SongTableColumn.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace MusicManager
{
    public abstract class SongTableColumn
    {
        protected SongTableColumn( int width )
        {
            Width = width;
        }

        public virtual  Vector2 IconSize { get; set; } = new Vector2( 20, 20 );
        public virtual  int     Width    { get; set; }
        public abstract int     Compare( SongDef a, SongDef b );
        public abstract void    DrawCell( Rect canvas, SongDef song );
        public abstract void    DrawHeader( Rect canvas, List<SongDef> songs );
        public abstract bool    Filter( SongDef song );
        public abstract bool Filtered { get; }

        public virtual void DrawOverlay( Rect canvas )
        {
            if ( Window_MusicManager.SortBy == this )
                GUI.DrawTextureWithTexCoords( canvas.RightPartPixels( 12 ).BottomPartPixels( 12 ),
                                              Window_MusicManager.SortDescending
                                                  ? Resources.ArrowDown
                                                  : Resources.ArrowUp,
                                              new Rect( 0, 0, 12f / Resources.ArrowDown.width,
                                                        12f       / Resources.ArrowDown.height )
                                                 .CenteredIn( new Rect( 0, 0, 1, 1 ) ) );
            if ( Filtered )
                GUI.DrawTexture( canvas.RightPartPixels( 12 ).TopPartPixels( 12 ), Resources.Funnel );
        }

        public abstract string HeaderTooltip { get; }
    }
}