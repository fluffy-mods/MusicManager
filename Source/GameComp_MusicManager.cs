// GameComp_MusicManager.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using UnityEngine;
using Verse;
using static MusicManager.Resources;

namespace MusicManager {
    public class GameComp_MusicManager: GameComponent {
        public const int Margin = 6;
        public const int RowHeight = 24;
        public const int SeekBarHeight = 6;
        public const int WidgetWidth = 150;
        private static bool _dragging;
        private static Vector2 _mousePos = Vector2.zero;
        public GameComp_MusicManager() {
            // scribe
        }

        public GameComp_MusicManager(Game game) {
        }

        public static bool Active => GenScene.InPlayScene;

        public static Vector2 Position {
            get {
                Vector2 pos = MusicManager.Settings.WidgetPosition;
                switch (MusicManager.Settings.WidgetAnchor) {
                    case WidgetAnchor.TopLeft:
                        return pos;
                    case WidgetAnchor.TopRight:
                        return new Vector2(Screen.x - pos.x, pos.y);
                    case WidgetAnchor.BottomLeft:
                        return new Vector2(pos.x, Screen.y - pos.y);
                    case WidgetAnchor.BottomRight:
                        return Screen - pos;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                Vector2 pos = new Vector2(Mathf.Clamp(value.x, 0, Screen.x - Size.x),
                                       Mathf.Clamp(value.y, 0, Screen.y - Size.y));
                if (pos.x + (Size.x / 2) > Screen.x / 2) {
                    if (pos.y + (Size.y / 2) > Screen.y / 2) {
                        MusicManager.Settings.WidgetAnchor = WidgetAnchor.BottomRight;
                        MusicManager.Settings.WidgetPosition = Screen - pos;
                    } else {
                        MusicManager.Settings.WidgetAnchor = WidgetAnchor.TopRight;
                        MusicManager.Settings.WidgetPosition = new Vector2(Screen.x - pos.x, pos.y);
                    }
                } else {
                    if (pos.y + (Size.y / 2) > Screen.y / 2) {
                        MusicManager.Settings.WidgetAnchor = WidgetAnchor.BottomLeft;
                        MusicManager.Settings.WidgetPosition = new Vector2(pos.x, Screen.y - pos.y);
                    } else {
                        MusicManager.Settings.WidgetAnchor = WidgetAnchor.TopLeft;
                        MusicManager.Settings.WidgetPosition = pos;
                    }
                }
            }
        }

        public static Vector2 Screen => new Vector2(UI.screenWidth, UI.screenHeight);

        public static Vector2 Size => new Vector2(WidgetWidth, (RowHeight * 2) + SeekBarHeight);

        private static bool Dragging {
            get => _dragging;
            set {
                _dragging = value;
                Log.Debug($"Dragging: {_dragging}");
                if (!_dragging) {
                    MusicManager.Settings.Write();
                }
            }
        }


        private static void HandleDragging(Rect canvas) {
            Widgets.DrawHighlightIfMouseover(canvas);
            if (Mouse.IsOver(canvas) && Event.current.type == EventType.MouseDown) {
                Dragging = true;
                _mousePos = Event.current.mousePosition;
                Event.current.Use();
            }

            if (Dragging && Event.current.type == EventType.MouseUp) {
                Dragging = false;
                Event.current.Use();
            }

            if (Dragging && Event.current.type == EventType.MouseDrag) {
                Position += Event.current.mousePosition - _mousePos;
                _mousePos = Event.current.mousePosition;
                Event.current.Use();
            }
        }

        public override void GameComponentOnGUI() {
            if (!Active) {
                return;
            }

            base.GameComponentOnGUI();
            Rect canvas = new Rect(Position, Size);
            Rect row = canvas.TopPartPixels(RowHeight);

            if (MusicManager.CurrentSong != null) {
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(row.WidthContractedBy(Margin), MusicManager.CurrentSong.Name());
                Text.Anchor = TextAnchor.UpperLeft;
                row.y += RowHeight;
                Utilities.DrawSeekBar(row.TopPartPixels(SeekBarHeight).WidthContractedBy(Margin));
                row.y += SeekBarHeight;
            }

            DrawButtons(row);
            if (!MusicManager.Settings.Locked) {
                HandleDragging(canvas);
            }
        }

        public override void GameComponentUpdate() {
            if (!Active) {
                return;
            }

            base.GameComponentUpdate();
            if (MusicManager.Seeking && MusicManager.LastSeek > 0.1f) {
                MusicManager.Resume();
                MusicManager.Seeking = false;
            }
        }

        private void DrawButtons(Rect canvas) {
            Rect buttonRect = canvas.RightPart(1 / 4f);
            Utilities.DrawButton(buttonRect, List, () => Find.WindowStack.Add(new Window_MusicManager()), 24);
            buttonRect.x -= buttonRect.width;
            Utilities.DrawButton(buttonRect, Next, MusicManager.Next, 24);
            buttonRect.x -= buttonRect.width;
            Utilities.DrawPlayButton(buttonRect);
            buttonRect.x -= buttonRect.width;
            Utilities.DrawButton(buttonRect, Previous, MusicManager.Previous, 24);
        }
    }
}
