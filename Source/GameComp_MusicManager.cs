// GameComp_MusicManager.cs
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
    public class GameComp_MusicManager : GameComponent
    {
        public const int Margin        = 6;
        public const int RowHeight     = 24;
        public const int SeekBarHeight = 5;
        public const int WidgetWidth   = 150;


        private static bool    _dragging;
        private static Vector2 _mousePos = Vector2.zero;

        public GameComp_MusicManager()
        {
            // scribe
        }

        public GameComp_MusicManager( Game game )
        {
        }

        public static Vector2 Position
        {
            get
            {
                var pos = MusicManager.Settings.WidgetPosition;
                switch ( MusicManager.Settings.WidgetAnchor )
                {
                    case WidgetAnchor.TopLeft:
                        return pos;
                    case WidgetAnchor.TopRight:
                        return new Vector2( Screen.x - pos.x, pos.y );
                    case WidgetAnchor.BottomLeft:
                        return new Vector2( pos.x, Screen.y - pos.y );
                    case WidgetAnchor.BottomRight:
                        return Screen - pos;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                var pos = new Vector2( Mathf.Clamp( value.x, 0, Screen.x - Size.x ),
                                       Mathf.Clamp( value.y, 0, Screen.y - Size.y ) );
                if ( pos.x + Size.x / 2 > Screen.x / 2 )
                {
                    if ( pos.y + Size.y / 2 > Screen.y / 2 )
                    {
                        MusicManager.Settings.WidgetAnchor   = WidgetAnchor.BottomRight;
                        MusicManager.Settings.WidgetPosition = Screen - pos;
                    }
                    else
                    {
                        MusicManager.Settings.WidgetAnchor   = WidgetAnchor.TopRight;
                        MusicManager.Settings.WidgetPosition = new Vector2( Screen.x - pos.x, pos.y );
                    }
                }
                else
                {
                    if ( pos.y + Size.y / 2 > Screen.y / 2 )
                    {
                        MusicManager.Settings.WidgetAnchor   = WidgetAnchor.BottomLeft;
                        MusicManager.Settings.WidgetPosition = new Vector2( pos.x, Screen.y - pos.y );
                    }
                    else
                    {
                        MusicManager.Settings.WidgetAnchor   = WidgetAnchor.TopLeft;
                        MusicManager.Settings.WidgetPosition = pos;
                    }
                }
            }
        }

        public static Vector2 Screen => new Vector2( UI.screenWidth, UI.screenHeight );

        public static Vector2 Size => new Vector2( WidgetWidth, RowHeight * 3 );

        private static bool Dragging
        {
            get => _dragging;
            set
            {
                _dragging = value;
                Log.Debug( $"Dragging: {_dragging}" );
                if ( !_dragging ) MusicManager.Settings.Write();
            }
        }


        private static void HandleDragging( Rect canvas )
        {
            Widgets.DrawHighlightIfMouseover( canvas );
            if ( Mouse.IsOver( canvas ) && Event.current.type == EventType.MouseDown )
            {
                Dragging  = true;
                _mousePos = Event.current.mousePosition;
                Event.current.Use();
            }

            if ( Dragging && Event.current.type == EventType.MouseUp )
            {
                Dragging = false;
                Event.current.Use();
            }

            if ( Dragging && Event.current.type == EventType.MouseDrag )
            {
                Position  += Event.current.mousePosition - _mousePos;
                _mousePos =  Event.current.mousePosition;
                Event.current.Use();
            }
        }

        public override void GameComponentOnGUI()
        {
            base.GameComponentOnGUI();
            var canvas = new Rect( Position, Size );
            var row    = canvas.TopPartPixels( RowHeight );

            if ( MusicManager.CurrentSong != null )
            {
                Widgets.Label( row.WidthContractedBy( Margin ), MusicManager.CurrentSong.Name() );
                row.y += RowHeight;
                Utilities.DrawSeekBar( row.TopPartPixels( SeekBarHeight ) );
                row.y += SeekBarHeight;
            }

            DrawButtons( row );
            if ( !MusicManager.Settings.Locked ) HandleDragging( canvas );
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
            DrawButton( buttonRect, Resources.List, () => Find.WindowStack.Add( new Window_MusicManager() ) );
            buttonRect.x -= buttonRect.width;
            DrawButton( buttonRect, Resources.Next, MusicManager.Next );
            buttonRect.x -= buttonRect.width;
            if ( MusicManager.AudioSource.isPlaying && !MusicManager.isPaused )
            {
                DrawButton( buttonRect, Resources.Pause, MusicManager.Pause );
            }
            else
            {
                if ( MusicManager.isPaused )
                    DrawButton( buttonRect, Resources.Play, MusicManager.Resume );
                else
                    DrawButton( buttonRect, Resources.Play, () => MusicManager.Play() );
            }

            buttonRect.x -= buttonRect.width;
            DrawButton( buttonRect, Resources.Previous, MusicManager.Previous );
        }
    }
}