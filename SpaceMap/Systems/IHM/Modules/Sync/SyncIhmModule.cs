using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class SyncIhmModule : IIhmModule
    {
        private readonly IMapEntryRepository _mapEntryRepository;
        private readonly SyncStats _syncStats;

        private const float BaseSize = 512f;
        private const float Margin = 18f;
        private const float LineH = 20f;
        private const float SmallLineH = 16f;
        private const float SectionGap = 10f;

        public SyncIhmModule(Program program)
        {
            _mapEntryRepository = program.Container.GetItem<IMapEntryRepository>();
            _syncStats = program.Container.GetItem<SyncStats>();
        }

        public void InitSurface(Panel panel, PanelSurface surface)
        {
            surface.Surface.ContentType = ContentType.SCRIPT;
            surface.Surface.Script = "";
        }

        public IEnumerator<bool> RenderTo(Panel panel, PanelSurface surface)
        {
            var frame = surface.Surface.DrawFrame();
            var viewport = new RectangleF(
                (surface.Surface.TextureSize - surface.Surface.SurfaceSize) / 2f,
                surface.Surface.SurfaceSize
            );
            var s = Math.Min(viewport.Size.X / BaseSize, viewport.Size.Y / BaseSize);
            var fg = surface.Surface.ScriptForegroundColor;
            var dim = fg * 0.4f;
            var accent = new Color(100, 180, 255);
            var margin = Margin * s;
            var y = viewport.Y;

            // Header
            var headerH = 40f * s;
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE, Data = "SquareSimple",
                Position = new Vector2(viewport.Center.X, y + headerH / 2),
                Size = new Vector2(viewport.Width, headerH),
                Color = fg * 0.12f, Alignment = TextAlignment.CENTER
            });
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = "SYNC STATUS",
                Color = fg,
                Position = new Vector2(viewport.Center.X, y + headerH / 2 - 10f * s),
                Alignment = TextAlignment.CENTER, RotationOrScale = s * 1.1f
            });
            y += headerH + 12f * s;

            // Database row
            var totalEntries = _mapEntryRepository.GetAll<IMapEntry>().Count;
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = "Database",
                Color = fg, Position = new Vector2(viewport.X + margin, y),
                Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.72f
            });
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = totalEntries + " entries",
                Color = dim, Position = new Vector2(viewport.Right - margin, y),
                Alignment = TextAlignment.RIGHT, RotationOrScale = s * 0.72f
            });
            y += LineH * s + SectionGap * s;

            if (_syncStats.Mode == ScriptMode.Server || _syncStats.Mode == ScriptMode.ServerShip)
            {
                // Server section: list sources
                var sources = _syncStats.Sources;
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXT,
                    Data = sources.Count == 0 ? "Sources" : "Sources (" + sources.Count + ")",
                    Color = accent, Position = new Vector2(viewport.X + margin, y),
                    Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.72f
                });
                y += LineH * s;

                if (sources.Count == 0)
                {
                    frame.Add(new MySprite
                    {
                        Type = SpriteType.TEXT, Data = "  No sources connected",
                        Color = dim, Position = new Vector2(viewport.X + margin, y),
                        Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.62f
                    });
                    y += SmallLineH * s;
                }
                else
                {
                    foreach (var src in sources)
                    {
                        var age = TimeUtils.FormatAge(src.LastSyncDate, shortFormat: true);
                        frame.Add(new MySprite
                        {
                            Type = SpriteType.TEXT, Data = "  " + src.Name,
                            Color = fg, Position = new Vector2(viewport.X + margin, y),
                            Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.67f
                        });
                        frame.Add(new MySprite
                        {
                            Type = SpriteType.TEXT, Data = age,
                            Color = dim, Position = new Vector2(viewport.Right - margin, y),
                            Alignment = TextAlignment.RIGHT, RotationOrScale = s * 0.67f
                        });
                        y += SmallLineH * s;
                        frame.Add(new MySprite
                        {
                            Type = SpriteType.TEXT, Data = "    " + src.EntryCount + " entries",
                            Color = dim, Position = new Vector2(viewport.X + margin, y),
                            Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.58f
                        });
                        y += SmallLineH * s + 4f * s;
                    }
                }
            }
            else
            {
                // Ship section: last broadcast status
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXT, Data = "Server sync",
                    Color = accent, Position = new Vector2(viewport.X + margin, y),
                    Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.72f
                });
                y += LineH * s;

                if (_syncStats.LastBroadcastDate == 0)
                {
                    frame.Add(new MySprite
                    {
                        Type = SpriteType.TEXT, Data = "  Waiting for first sync...",
                        Color = dim, Position = new Vector2(viewport.X + margin, y),
                        Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.62f
                    });
                    y += SmallLineH * s;
                }
                else
                {
                    var age = TimeUtils.FormatAge(_syncStats.LastBroadcastDate, shortFormat: true);
                    frame.Add(new MySprite
                    {
                        Type = SpriteType.TEXT, Data = "  Last broadcast",
                        Color = fg, Position = new Vector2(viewport.X + margin, y),
                        Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.67f
                    });
                    frame.Add(new MySprite
                    {
                        Type = SpriteType.TEXT, Data = age,
                        Color = dim, Position = new Vector2(viewport.Right - margin, y),
                        Alignment = TextAlignment.RIGHT, RotationOrScale = s * 0.67f
                    });
                    y += SmallLineH * s;
                    frame.Add(new MySprite
                    {
                        Type = SpriteType.TEXT, Data = "    " + _syncStats.LastBroadcastEntryCount + " entries sent",
                        Color = dim, Position = new Vector2(viewport.X + margin, y),
                        Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.58f
                    });
                    y += SmallLineH * s;
                }
            }

            frame.Dispose();
            yield return false;
        }
    }
}
