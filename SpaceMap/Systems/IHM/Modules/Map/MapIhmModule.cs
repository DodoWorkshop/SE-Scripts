using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class MapIhmModule : IIhmModule
    {
        private readonly Program _program;
        private readonly IMapEntryRepository _mapEntryRepository;
        private readonly IUserSettingsRepository _userSettingsRepository;

        private readonly Vector2 BaseRatio = new Vector2(512, 512);
        private const int MapPadding = 40;
        private const int BreakerLength = 20;


        public MapIhmModule(Program program)
        {
            _program = program;
            _mapEntryRepository = program.Container.GetItem<IMapEntryRepository>();
            _userSettingsRepository = program.Container.GetItem<IUserSettingsRepository>();
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
            var displayDiameter = _userSettingsRepository.MapScale;
            var scaleFactor = new Vector2(viewport.Size.X / BaseRatio.X, viewport.Size.Y / BaseRatio.Y);
            var uniformScale = Math.Min(scaleFactor.X, scaleFactor.Y);

            var headerH = 30f * uniformScale;
            var fg = surface.Surface.ScriptForegroundColor;;

            // Header
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE, Data = "SquareSimple",
                Position = new Vector2(viewport.Center.X, viewport.Y + headerH / 2),
                Size = new Vector2(viewport.Width, headerH),
                Color = fg * 0.12f, Alignment = TextAlignment.CENTER
            });
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = "MAP",
                Color = fg,
                Position = new Vector2(viewport.X + 18f * uniformScale, viewport.Y + headerH / 2 - 10f * uniformScale),
                Alignment = TextAlignment.LEFT, RotationOrScale = uniformScale * 1.1f
            });

            var uniformMapPadding = uniformScale * MapPadding;
            var mapAreaH = viewport.Size.Y - headerH;
            var minViewportAxis = Math.Min(viewport.Size.X, mapAreaH);
            var mapSize = new Vector2(minViewportAxis - uniformMapPadding * 2, minViewportAxis - uniformMapPadding * 2);
            var mapAreaCenter = new Vector2(viewport.Center.X, viewport.Y + headerH + mapAreaH / 2f);
            var mapFrame = new RectangleF(mapAreaCenter - mapSize / 2, mapSize);

            // Map border
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = "CircleHollow",
                Position = mapFrame.Center,
                Size = mapFrame.Size,
                Color = fg,
                Alignment = TextAlignment.CENTER
            });

            // Center dot (ship position)
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = "Circle",
                Position = mapFrame.Center,
                Size = Vector2.One * 15 * uniformScale,
                Color = fg,
                Alignment = TextAlignment.CENTER
            });

            yield return true;

            var points = BuildMapPoints(panel.Block, displayDiameter / 2);

            yield return true;

            var mapInnerFrame = new RectangleF(
                mapFrame.Position + new Vector2(20 * uniformScale, 20 * uniformScale),
                new Vector2(mapFrame.Size.X - 40 * uniformScale, mapFrame.Size.Y - 40 * uniformScale)
            );
            var breaker = 0;
            var mapFactor = new Vector2(viewport.Size.X / displayDiameter, viewport.Size.Y / displayDiameter);
            var newEntryColor = new Color(80, 220, 140);

            foreach (var point in points)
            {
                // Isometric perspective: objects above (positive depth) shift up-right on screen
                var depthInScreen = (float)(point.Depth * mapFactor.X);
                var perspectiveShift = depthInScreen * Program.MapPerspectiveStrength;

                var pos = mapInnerFrame.Center + new Vector2(
                    (float)(point.Position.X * mapFactor.X) + perspectiveShift * 0.5f,
                    (float)(point.Position.Y * mapFactor.Y) - perspectiveShift
                );

                var isNew = TimeUtils.IsNew(point.FirstDetectionDate);
                var dotColor = isNew ? newEntryColor : fg;
                var dotSize = Math.Max(8f, 18 * uniformScale + 8 * (float)(Math.Abs(point.Depth) / displayDiameter));

                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXTURE,
                    Data = "Circle",
                    Position = pos,
                    Size = Vector2.One * dotSize,
                    Color = dotColor,
                    Alignment = TextAlignment.CENTER
                });

                if (isNew)
                {
                    frame.Add(new MySprite
                    {
                        Type = SpriteType.TEXTURE,
                        Data = "CircleHollow",
                        Position = pos,
                        Size = Vector2.One * dotSize * 2.2f,
                        Color = dotColor,
                        Alignment = TextAlignment.CENTER
                    });
                }

                var ageStr = TimeUtils.FormatAge(point.UpdateDate, shortFormat: true);
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXT,
                    Data = $"{point.Label} ({ageStr})",
                    Color = fg,
                    Position = pos + new Vector2(0, -38 * uniformScale),
                    Alignment = TextAlignment.CENTER,
                    RotationOrScale = uniformScale * 0.8f
                });

                breaker++;
                if (breaker > BreakerLength)
                {
                    breaker = 0;
                    yield return true;
                }
            }

            yield return true;

            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT,
                Data = $"{points.Length} detected",
                Color = fg * 0.6f,
                Position = new Vector2(viewport.X + 20, viewport.Y + headerH + 8f * uniformScale),
                Alignment = TextAlignment.LEFT,
                RotationOrScale = uniformScale
            });

            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT,
                Data = $"Scale: {displayDiameter}m",
                Color = fg * 0.6f,
                Position = new Vector2(viewport.X + 20, viewport.Bottom - 50),
                Alignment = TextAlignment.LEFT,
                RotationOrScale = uniformScale
            });

            frame.Dispose();
            yield return false;
        }

        private MapPoint[] BuildMapPoints(IMyTerminalBlock referenceBlock, uint searchRadius)
        {
            var screenCenter = referenceBlock.CubeGrid.WorldVolume.Center;
            var right = referenceBlock.WorldMatrix.Right;
            var up = referenceBlock.WorldMatrix.Up;
            var forward = Vector3D.Cross(right, up);

            var points = new List<MapPoint>();

            foreach (var entry in _mapEntryRepository.GetAllInArea<IMapEntry>(_program.Me.GetPosition(), searchRadius))
            {
                var relative = entry.Position - screenCenter;
                var x = -(float)Vector3D.Dot(relative, right);
                var y = (float)Vector3D.Dot(relative, forward);
                var depth = Vector3D.Dot(relative, up);

                var label = string.IsNullOrEmpty(entry.CustomName) ? entry.BaseName : entry.CustomName;
                points.Add(new MapPoint(label, new Vector2(x, y), depth, entry.UpdateDate, entry.FirstDetectionDate));
            }

            return points.ToArray();
        }

        private struct MapPoint
        {
            public string Label { get; }
            public Vector2 Position { get; }
            public double Depth { get; }
            public long UpdateDate { get; }
            public long FirstDetectionDate { get; }

            public MapPoint(string label, Vector2 position, double depth, long updateDate, long firstDetectionDate)
            {
                Label = label;
                Position = position;
                Depth = depth;
                UpdateDate = updateDate;
                FirstDetectionDate = firstDetectionDate;
            }
        }
    }
}
