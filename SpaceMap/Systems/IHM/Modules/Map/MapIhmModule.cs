using System;
using System.Collections.Generic;
using System.Linq;
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

        private  Vector2 BaseRatio = new Vector2(512, 512);
        private const int MapPadding = 40;
        private const int BreakerLength = 20;

        public MapIhmModule(Program program)
        {
            _program = program;
            _mapEntryRepository = program.RepositoryManager.GetRepository<IMapEntryRepository>();
            _userSettingsRepository = program.RepositoryManager.GetRepository<IUserSettingsRepository>();
        }

        public void InitSurface(Panel panel, PanelSurface surface)
        {
            surface.Surface.ContentType = ContentType.SCRIPT;
            surface.Surface.Script = "";
        }

        public IEnumerator<bool> RenderTo(Panel panel, PanelSurface surface)
        {
            // Build viewport
            var frame = surface.Surface.DrawFrame();
            var viewport = new RectangleF(
                (surface.Surface.TextureSize - surface.Surface.SurfaceSize) / 2f,
                surface.Surface.SurfaceSize
            );
            var displayDiameter = _userSettingsRepository.MapScale;
            var scaleFactor = new Vector2(viewport.Size.X / BaseRatio.X, viewport.Size.Y / BaseRatio.Y);
            var uniformScale = Math.Min(scaleFactor.X, scaleFactor.Y);

            // Build Map frame
            var uniformMapPadding = uniformScale * MapPadding;
            var minViewportAxis = Math.Min(viewport.Size.X, viewport.Size.Y);
            var mapSize = new Vector2(minViewportAxis - uniformMapPadding * 2, minViewportAxis - uniformMapPadding * 2);
            var mapFrame = new RectangleF(
                viewport.Center - mapSize / 2,
                mapSize
            );
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = "CircleHollow",
                Position = mapFrame.Center,
                Size = mapFrame.Size,
                Color = surface.Surface.ScriptForegroundColor,
                Alignment = TextAlignment.CENTER
            });

            // Add center
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                //Data = "AH_BoreSight", // TODO: show ship orientation
                Data = "Circle",
                Position = mapFrame.Center,
                Size = Vector2.One * 15 * uniformScale,
                Color = surface.Surface.ScriptForegroundColor,
                Alignment = TextAlignment.CENTER
            });

            yield return true;

            var points = BuildMapPoints(panel.Block, displayDiameter / 2);

            yield return true;

            // Add points
            var mapInnerFrame =  new RectangleF(
                mapFrame.Position + new Vector2(20 * uniformScale, 20 * uniformScale),
                new Vector2(mapFrame.Size.X - 40 * uniformScale, mapFrame.Size.Y - 40 * uniformScale)
            );
            var breaker = 0;
            var mapFactor = new Vector2(viewport.Size.X / displayDiameter, viewport.Size.Y / displayDiameter);
            
            foreach (var point in points)
            {
                var pos = mapInnerFrame.Center + new Vector2(
                    (float)(point.Position.X * mapFactor.X),
                    (float)(point.Position.Y * mapFactor.Y)
                );
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXTURE,
                    Data = "Circle",
                    Position = pos,
                    Size = Vector2.One * 20 * uniformScale + Vector2.One * 20 * (float)(point.Depth / displayDiameter),
                    Color = surface.Surface.ScriptForegroundColor,
                    Alignment = TextAlignment.CENTER
                });

                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXT,
                    Data = $"{point.Label}",
                    Color = surface.Surface.ScriptForegroundColor,
                    Position = pos + new Vector2(0, -40 * uniformScale),
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
                Color = surface.Surface.ScriptForegroundColor,
                Position = new Vector2(viewport.X + 20, viewport.Position.Y + 20),
                Alignment = TextAlignment.LEFT,
                RotationOrScale = uniformScale
            });

            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT,
                Data = $"Scale: {displayDiameter}m",
                Color = surface.Surface.ScriptForegroundColor,
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

            var points2D = new List<MapPoint>();
            var min = new Vector2D(double.MaxValue, double.MaxValue);
            var max = new Vector2D(double.MinValue, double.MinValue);

            foreach (var worldPoint in _mapEntryRepository
                         .GetAllInArea<IMapEntry>(_program.Me.GetPosition(), searchRadius)
                    )
            {
                var relative = worldPoint.Position - screenCenter;

                var x = (float)Vector3D.Dot(relative, right);
                var y = (float)Vector3D.Dot(relative, forward);
                var p = new Vector2(x, y);

                var mapPoint = new MapPoint(
                    string.IsNullOrEmpty(worldPoint.CustomName) ? worldPoint.BaseName : worldPoint.CustomName,
                    p,
                    (float)Vector3D.Dot(relative, up)
                );
                points2D.Add(mapPoint);

                min = Vector2D.Min(min, p);
                max = Vector2D.Max(max, p);
            }

            return points2D.ToArray();
        }

        private struct MapPoint
        {
            public string Label { get; }

            public Vector2D Position { get; }

            public double Depth { get; }


            public MapPoint(string label, Vector2D position, double depth)
            {
                Label = label;
                Position = position;
                Depth = depth;
            }
        }
    }
}