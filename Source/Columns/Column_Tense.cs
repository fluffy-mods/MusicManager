// Column_Tense.cs
// Copyright Karel Kroeze, -2020

using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using UnityEngine;
using Verse;

namespace MusicManager
{
    public class Column_Tense : SongTableColumn
    {
        private bool? _filterTense;

        public Column_Tense( int width ) : base( width )
        {
        }

        public bool? FilterTense
        {
            get => _filterTense;
            set
            {
                if ( _filterTense == value ) return;
                _filterTense = value;
                Window_MusicManager.SetDirty();
            }
        }

        public override int Compare( SongDef a, SongDef b )
        {
            return a.tense.CompareTo( b.tense );
        }

        public override void DrawCell( Rect canvas, SongDef song )
        {
            TooltipHandler.TipRegion( canvas, () => Tooltip( song ), song.GetHashCode() ^ canvas.GetHashCode() );
            if ( Utilities.DrawButton( canvas, song.tense ? Resources.Explosion : Resources.Dove, IconSize.x ) )
            {
                song.tense = !song.tense;
                Window_MusicManager.SetDirty();
            }
        }

        public string Tooltip( SongDef song )
        {
            return I18n.AllowedTense( song.tense );
        }

        public override void DrawHeader( Rect canvas, List<SongDef> songs )
        {
            if (Utilities.Shift() )
            {
                if ( songs.All( s => s.tense ) )
                {
                    if ( Utilities.DrawButton( canvas, Resources.Explosion, IconSize.x ) )
                    {
                        foreach ( var song in songs )
                            song.tense = false;

                        Window_MusicManager.SetDirty();
                    }
                }
                else if ( songs.All( s => !s.tense ) )
                {
                    if ( Utilities.DrawButton( canvas, Resources.Dove, IconSize.x ) )
                    {
                        foreach ( var song in songs )
                            song.tense = true;

                        Window_MusicManager.SetDirty();
                    }
                }
                else
                {
                    var iconRect = new Rect( Vector2.zero, IconSize ).CenteredIn( canvas );
                    GUI.DrawTextureWithTexCoords( iconRect.LeftHalf(), Resources.Dove, new Rect( 0, 0, .5f, 1 ) );
                    GUI.DrawTextureWithTexCoords( iconRect.RightHalf(), Resources.Explosion, new Rect( .5f, 0, .5f, 1 ) );
                    if ( Utilities.DrawButton(iconRect ) )
                    {
                        foreach ( var song in songs.Where( s => !s.tense ) )
                            song.tense = true;

                        Window_MusicManager.SetDirty();
                    }
                }
            }
            else if ( Utilities.Alt()) 
            {
                var iconRect = new Rect( Vector2.zero, IconSize ).CenteredIn( canvas );
                if ( FilterTense.HasValue && FilterTense.Value )
                {
                    GUI.DrawTexture( iconRect, Resources.Explosion );
                    if ( Utilities.DrawButton( canvas ) )
                        FilterTense = false;
                }
                else if (FilterTense.HasValue && !FilterTense.Value )
                {
                    GUI.DrawTexture( iconRect, Resources.Dove );
                    if ( Utilities.DrawButton( canvas ) )
                        FilterTense = null;
                }
                else
                {
                    GUI.DrawTextureWithTexCoords( iconRect.LeftPart( 1 / 2f ), Resources.Dove, new Rect( 0, 0, .5f, 1 ) );
                    GUI.DrawTextureWithTexCoords( iconRect.RightPart( 1 / 2f ), Resources.Explosion, new Rect( .5f, 0, .5f, 1 ) );
                    if ( Utilities.DrawButton( canvas ) )
                        FilterTense = true;
                }
            }
            else
            {
                if ( Utilities.DrawButton( canvas, Resources.Explosion, IconSize.x ) )
                    Window_MusicManager.SortBy = this;
            }

            DrawOverlay( canvas );
            TooltipHandler.TipRegion( canvas, () => HeaderTooltip, GetHashCode() );
        }

        public override bool Filter( SongDef song )
        {
            if ( !FilterTense.HasValue )
                return true;
            return song.tense == FilterTense;
        }

        public override bool Filtered => FilterTense.HasValue;

        public override string HeaderTooltip => $"{I18n.TenseColumn}\n\n{I18n.TenseColumn_Tip}";
    }
}