using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class Map3DIhmModule : IIhmModule
    {
        private readonly Program _program;
        private readonly IMapEntryRepository _mapEntryRepository;
        private readonly IUserSettingsRepository _userSettingsRepository;

        private readonly Vector2 BaseRatio = new Vector2(512, 512);
        private const float MapPadding = 40f;
        private const int BreakerLength = 8;

        // Dashed line appearance
        private const float DashLength = 5f;
        private const float GapLength = 4f;
        private const float LineWidth = 1.5f;

        public Map3DIhmModule(Program program)
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
            var fgColor = surface.Surface.ScriptForegroundColor;

            // Header
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE, Data = "SquareSimple",
                Position = new Vector2(viewport.Center.X, viewport.Y + headerH / 2),
                Size = new Vector2(viewport.Width, headerH),
                Color = fgColor * 0.12f, Alignment = TextAlignment.CENTER
            });
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = "MAP 3D",
                Color = fgColor,
                Position = new Vector2(viewport.X + 18f * uniformScale, viewport.Y + headerH / 2 - 10f * uniformScale),
                Alignment = TextAlignment.LEFT, RotationOrScale = uniformScale * 1.1f
            });

            var uniformMapPadding = uniformScale * MapPadding;
            var mapAreaH = viewport.Size.Y - headerH;
            var mapWidth = Math.Min(viewport.Size.X, mapAreaH) - uniformMapPadding * 2;
            var mapCenter = new Vector2(viewport.Center.X, viewport.Y + headerH + mapAreaH / 2f);
            var scale = mapWidth / displayDiameter;

            // ---- Horizontal plane: the main ellipse ----
            var ellipseSize = new Vector2(mapWidth, mapWidth * Program.Map3DEllipseRatio);
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = "CircleHollow",
                Position = mapCenter,
                Size = ellipseSize,
                Color = surface.Surface.ScriptForegroundColor,
                Alignment = TextAlignment.CENTER
            });

            // Cross-hairs on the ellipse for orientation
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = mapCenter,
                Size = new Vector2(mapWidth, uniformScale),
                Color = surface.Surface.ScriptForegroundColor * 0.25f,
                Alignment = TextAlignment.CENTER
            });
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = mapCenter,
                Size = new Vector2(uniformScale, mapWidth * Program.Map3DEllipseRatio),
                Color = surface.Surface.ScriptForegroundColor * 0.25f,
                Alignment = TextAlignment.CENTER
            });

            // Center dot (ship)
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = "Circle",
                Position = mapCenter,
                Size = Vector2.One * 13 * uniformScale,
                Color = surface.Surface.ScriptForegroundColor,
                Alignment = TextAlignment.CENTER
            });

            yield return true;

            var points = BuildMapPoints(panel.Block, displayDiameter / 2);

            yield return true;

            var breaker = 0;
            var newEntryColor = new Color(80, 220, 140);
            var shadowColor = fgColor * 0.35f;
            var lineColor = fgColor * 0.5f;

            foreach (var point in points)
            {
                // Isometric projection onto the squished plane
                var planeX = mapCenter.X + (float)(point.Right * scale);
                var planeY = mapCenter.Y + (float)(point.Forward * scale * Program.Map3DEllipseRatio);
                var depthPixels = (float)(point.Depth * scale * Program.Map3DDepthScale);

                var planePos = new Vector2(planeX, planeY);
                // Depth goes up on screen for positive (above ship)
                var actualPos = new Vector2(planeX, planeY - depthPixels);

                var isNew = TimeUtils.IsNew(point.FirstDetectionDate);
                var dotColor = isNew ? newEntryColor : fgColor;

                // Dashed vertical line: from plane projection to actual 3D position
                if (Math.Abs(depthPixels) > 3f)
                    DrawDashedLine(frame, planeX, planeY, actualPos.Y, lineColor, uniformScale);

                // Shadow on the plane (projection)
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXTURE,
                    Data = "Circle",
                    Position = planePos,
                    Size = Vector2.One * 6 * uniformScale,
                    Color = shadowColor,
                    Alignment = TextAlignment.CENTER
                });

                // 3D position dot
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXTURE,
                    Data = "Circle",
                    Position = actualPos,
                    Size = Vector2.One * 15 * uniformScale,
                    Color = dotColor,
                    Alignment = TextAlignment.CENTER
                });

                // Outer ring for new entries
                if (isNew)
                {
                    frame.Add(new MySprite
                    {
                        Type = SpriteType.TEXTURE,
                        Data = "CircleHollow",
                        Position = actualPos,
                        Size = Vector2.One * 26 * uniformScale,
                        Color = dotColor,
                        Alignment = TextAlignment.CENTER
                    });
                }

                // Label to the right of the dot
                var ageStr = TimeUtils.FormatAge(point.UpdateDate, shortFormat: true);
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXT,
                    Data = $"{point.Label} ({ageStr})",
                    Color = fgColor,
                    Position = actualPos + new Vector2(12 * uniformScale, -6 * uniformScale),
                    Alignment = TextAlignment.LEFT,
                    RotationOrScale = uniformScale * 0.75f
                });

                breaker++;
                if (breaker > BreakerLength)
                {
                    breaker = 0;
                    yield return true;
                }
            }

            yield return true;

            // HUD info
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT,
                Data = $"{points.Length} detected",
                Color = fgColor * 0.6f,
                Position = new Vector2(viewport.X + 20, viewport.Y + headerH + 8f * uniformScale),
                Alignment = TextAlignment.LEFT,
                RotationOrScale = uniformScale
            });
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT,
                Data = $"Scale: {displayDiameter}m",
                Color = fgColor * 0.6f,
                Position = new Vector2(viewport.X + 20, viewport.Bottom - 50),
                Alignment = TextAlignment.LEFT,
                RotationOrScale = uniformScale
            });

            frame.Dispose();
            yield return false;
        }

        // Draws a dashed vertical line between two Y coordinates at a fixed X
        private void DrawDashedLine(MySpriteDrawFrame frame, float x, float fromY, float toY, Color color, float uniformScale)
        {
            var startY = Math.Min(fromY, toY);
            var endY = Math.Max(fromY, toY);
            var step = DashLength + GapLength;

            for (var t = 0f; t < endY - startY; t += step)
            {
                var segStart = startY + t;
                var segEnd = Math.Min(segStart + DashLength, endY);
                var segLen = segEnd - segStart;
                if (segLen < 1f) break;

                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = new Vector2(x, segStart + segLen * 0.5f),
                    Size = new Vector2(LineWidth * uniformScale, segLen),
                    Color = color,
                    Alignment = TextAlignment.CENTER
                });
            }
        }

        private MapPoint[] BuildMapPoints(IMyTerminalBlock referenceBlock, uint searchRadius)
        {
            var screenCenter = referenceBlock.CubeGrid.WorldVolume.Center;
            var right = referenceBlock.WorldMatrix.Right;
            var up = referenceBlock.WorldMatrix.Up;
            var forward = Vector3D.Cross(right, up);

            var result = new List<MapPoint>();
            foreach (var entry in _mapEntryRepository.GetAllInArea<IMapEntry>(_program.Me.GetPosition(), searchRadius))
            {
                var relative = entry.Position - screenCenter;
                var label = string.IsNullOrEmpty(entry.CustomName) ? entry.BaseName : entry.CustomName;
                result.Add(new MapPoint(
                    label,
                    -Vector3D.Dot(relative, right),
                    Vector3D.Dot(relative, forward),
                    Vector3D.Dot(relative, up),
                    entry.UpdateDate,
                    entry.FirstDetectionDate
                ));
            }
            return result.ToArray();
        }

        private struct MapPoint
        {
            public string Label { get; }
            public double Right { get; }
            public double Forward { get; }
            public double Depth { get; }
            public long UpdateDate { get; }
            public long FirstDetectionDate { get; }

            public MapPoint(string label, double right, double forward, double depth, long updateDate, long firstDetectionDate)
            {
                Label = label;
                Right = right;
                Forward = forward;
                Depth = depth;
                UpdateDate = updateDate;
                FirstDetectionDate = firstDetectionDate;
            }
        }
    }
}
