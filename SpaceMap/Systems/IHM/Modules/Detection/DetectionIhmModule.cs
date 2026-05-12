using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class DetectionIhmModule : IIhmModule
    {
        private readonly Program _program;
        private readonly IDetectionDataRepository _detectionDataRepository;
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly IMapEntryRepository _mapEntryRepository;

        private const float BaseSize = 512f;
        private const float LabelRatio = 0.38f;
        private const float RowHeight = 26f;
        private const float Margin = 18f;

        public DetectionIhmModule(Program program)
        {
            _program = program;
            _detectionDataRepository = program.Container.GetItem<IDetectionDataRepository>();
            _userSettingsRepository = program.Container.GetItem<IUserSettingsRepository>();
            _mapEntryRepository = program.Container.GetItem<IMapEntryRepository>();
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
            var newColor = new Color(80, 220, 140);
            var knownColor = new Color(100, 160, 220);

            var margin = Margin * s;
            var contentWidth = viewport.Width - margin * 2;
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
                Type = SpriteType.TEXT, Data = "DETECTION",
                Color = fg,
                Position = new Vector2(viewport.Center.X, y + headerH / 2 - 10f * s),
                Alignment = TextAlignment.CENTER, RotationOrScale = s * 1.1f
            });
            y += headerH + 10f * s;

            // Sensor section
            AddSectionLabel(frame, viewport.X + margin, ref y, "SENSOR", dim, s);
            AddDataRow(frame, viewport.X + margin, ref y, contentWidth, "Range", $"{_userSettingsRepository.DetectionDistance}m", fg, dim, s);
            AddChargeBar(frame, viewport.X + margin, ref y, contentWidth, (float)_detectionDataRepository.RaycastCharge, fg, dim, newColor, s);
            AddDivider(frame, viewport.Center.X, ref y, contentWidth, dim, s);

            // Entity section
            AddSectionLabel(frame, viewport.X + margin, ref y, "ENTITY", dim, s);

            if (_detectionDataRepository.DetectedEntityInfo.HasValue)
            {
                var result = _detectionDataRepository.DetectedEntityInfo.Value;
                var distance = (long)Vector3D.Distance(result.Position, _program.Me.GetPosition());

                if (result.Type != MyDetectedEntityType.Asteroid)
                {
                    AddStatusDot(frame, viewport.X + margin, ref y, "NOT HANDLED", fg * 0.5f, s);
                    AddDataRow(frame, viewport.X + margin, ref y, contentWidth, "Type", result.Type.ToString(), fg, dim, s);
                }
                else
                {
                    var entry = _mapEntryRepository.GetOneById<IMapEntry>(result.EntityId);
                    if (entry == null)
                    {
                        AddStatusDot(frame, viewport.X + margin, ref y, "SAVING...", fg * 0.6f, s);
                    }
                    else
                    {
                        var isNew = TimeUtils.IsNew(entry.FirstDetectionDate);
                        AddStatusDot(frame, viewport.X + margin, ref y, isNew ? "NEW CONTACT" : "KNOWN", isNew ? newColor : knownColor, s);
                        AddDataRow(frame, viewport.X + margin, ref y, contentWidth, "Type", entry.GetType().Name, fg, dim, s);
                        AddDataRow(frame, viewport.X + margin, ref y, contentWidth, "Name", entry.BaseName, fg, dim, s);
                        if (!string.IsNullOrEmpty(entry.CustomName))
                            AddDataRow(frame, viewport.X + margin, ref y, contentWidth, "Alias", entry.CustomName, fg, dim, s);
                    }
                }

                AddDataRow(frame, viewport.X + margin, ref y, contentWidth, "Dist", $"{distance}m", fg, dim, s);
                AddDataRow(frame, viewport.X + margin, ref y, contentWidth, "ID", result.EntityId.ToString(), fg, dim, s);
            }
            else
            {
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXT, Data = "No signal",
                    Color = dim,
                    Position = new Vector2(viewport.Center.X, y + RowHeight * s / 2 - 10f * s),
                    Alignment = TextAlignment.CENTER, RotationOrScale = s * 0.9f
                });
            }

            frame.Dispose();
            yield return false;
        }

        private void AddSectionLabel(MySpriteDrawFrame frame, float x, ref float y, string label, Color color, float s)
        {
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = label,
                Color = color, Position = new Vector2(x, y),
                Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.65f
            });
            y += 18f * s;
        }

        private void AddDataRow(MySpriteDrawFrame frame, float x, ref float y, float width, string label, string value, Color fg, Color dim, float s)
        {
            var h = RowHeight * s;
            var textY = y + h / 2 - 10f * s;
            var fontSize = s * 0.8f;

            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = label,
                Color = dim, Position = new Vector2(x, textY),
                Alignment = TextAlignment.LEFT, RotationOrScale = fontSize
            });
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = value,
                Color = fg, Position = new Vector2(x + width * LabelRatio, textY),
                Alignment = TextAlignment.LEFT, RotationOrScale = fontSize
            });

            y += h;
        }

        private void AddChargeBar(MySpriteDrawFrame frame, float x, ref float y, float width, float charge, Color fg, Color dim, Color fillColor, float s)
        {
            var labelWidth = width * LabelRatio;
            var barWidth = width - labelWidth;
            var barH = 14f * s;
            var barX = x + labelWidth;
            var barCenterX = barX + barWidth / 2;
            var textY = y + barH / 2 - 10f * s;

            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = "Charge",
                Color = dim, Position = new Vector2(x, textY),
                Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.8f
            });

            // Bar background
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE, Data = "SquareSimple",
                Position = new Vector2(barCenterX, y + barH / 2),
                Size = new Vector2(barWidth, barH),
                Color = dim * 0.5f, Alignment = TextAlignment.CENTER
            });

            // Bar fill
            if (charge > 0f)
            {
                var fillW = barWidth * charge;
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXTURE, Data = "SquareSimple",
                    Position = new Vector2(barX + fillW / 2, y + barH / 2),
                    Size = new Vector2(fillW, barH),
                    Color = fillColor, Alignment = TextAlignment.CENTER
                });
            }

            // Percentage overlaid on bar
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = $"{charge:P0}",
                Color = fg, Position = new Vector2(barCenterX, textY),
                Alignment = TextAlignment.CENTER, RotationOrScale = s * 0.72f
            });

            y += barH + 8f * s;
        }

        private void AddStatusDot(MySpriteDrawFrame frame, float x, ref float y, string status, Color color, float s)
        {
            var h = RowHeight * s;
            var dotR = 7f * s;
            var midY = y + h / 2;

            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE, Data = "Circle",
                Position = new Vector2(x + dotR, midY),
                Size = Vector2.One * dotR * 2f,
                Color = color, Alignment = TextAlignment.CENTER
            });
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = status,
                Color = color,
                Position = new Vector2(x + dotR * 2f + 6f * s, midY - 10f * s),
                Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.85f
            });

            y += h;
        }

        private void AddDivider(MySpriteDrawFrame frame, float centerX, ref float y, float width, Color color, float s)
        {
            y += 6f * s;
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE, Data = "SquareSimple",
                Position = new Vector2(centerX, y),
                Size = new Vector2(width, s),
                Color = color, Alignment = TextAlignment.CENTER
            });
            y += 10f * s;
        }
    }
}
