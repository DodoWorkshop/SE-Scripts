using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class DatabaseIhmModule : IIhmModule
    {
        private readonly Program _program;
        private readonly IMapEntryRepository _mapEntryRepository;
        private readonly DatabaseViewSettings _viewSettings;

        private const float BaseSize = 512f;
        private const float Margin = 18f;
        private const float RowHeight = 24f;

        public DatabaseIhmModule(Program program)
        {
            _program = program;
            _mapEntryRepository = program.Container.GetItem<IMapEntryRepository>();
            _viewSettings = program.Container.GetItem<DatabaseViewSettings>();
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

            var margin = Margin * s;
            var contentWidth = viewport.Width - margin * 2;
            var contentRight = viewport.X + viewport.Width - margin;
            var rowH = RowHeight * s;
            var y = viewport.Y;

            var selfPosition = _program.Me.GetPosition();
            var entries = GetSortedEntries(selfPosition);
            var total = entries.Count;

            var headerH = 40f * s;
            var infoBarH = 22f * s;
            var divH = 14f * s;
            var pageSize = Math.Max(1, (int)((viewport.Height - headerH - infoBarH - divH * 2) / rowH));

            _viewSettings.ScrollOffset = Math.Max(0, Math.Min(_viewSettings.ScrollOffset, Math.Max(0, total - 1)));
            var from = _viewSettings.ScrollOffset;
            var to = Math.Min(from + pageSize, total);

            // Header
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE, Data = "SquareSimple",
                Position = new Vector2(viewport.Center.X, y + headerH / 2),
                Size = new Vector2(viewport.Width, headerH),
                Color = fg * 0.12f, Alignment = TextAlignment.CENTER
            });
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = "DATABASE",
                Color = fg,
                Position = new Vector2(viewport.X + margin, y + headerH / 2 - 10f * s),
                Alignment = TextAlignment.LEFT, RotationOrScale = s * 1.1f
            });
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = $"{total}",
                Color = dim,
                Position = new Vector2(contentRight, y + headerH / 2 - 10f * s),
                Alignment = TextAlignment.RIGHT, RotationOrScale = s * 1.1f
            });
            y += headerH;

            // Info bar
            var infoTextY = y + infoBarH / 2 - 10f * s;
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXT, Data = $"Sort: {_viewSettings.SortMode}",
                Color = dim,
                Position = new Vector2(viewport.X + margin, infoTextY),
                Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.7f
            });
            if (total > 0)
            {
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXT, Data = $"{from + 1}-{to} / {total}",
                    Color = dim,
                    Position = new Vector2(contentRight, infoTextY),
                    Alignment = TextAlignment.RIGHT, RotationOrScale = s * 0.7f
                });
            }
            y += infoBarH;

            // Top divider
            frame.Add(new MySprite
            {
                Type = SpriteType.TEXTURE, Data = "SquareSimple",
                Position = new Vector2(viewport.Center.X, y + divH / 2),
                Size = new Vector2(contentWidth, s),
                Color = dim, Alignment = TextAlignment.CENTER
            });
            y += divH;

            // Entry list
            if (total == 0)
            {
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXT, Data = "No entries",
                    Color = dim,
                    Position = new Vector2(viewport.Center.X, y + rowH / 2 - 10f * s),
                    Alignment = TextAlignment.CENTER, RotationOrScale = s * 0.9f
                });
            }
            else
            {
                var dotR = 5f * s;
                var dotX = viewport.X + margin + dotR;
                var nameColX = viewport.X + margin + dotR * 3f;
                var distColX = contentRight - 52f * s;
                var ageColX = contentRight;
                var fontSize = s * 0.75f;

                for (var i = from; i < to; i++)
                {
                    var entry = entries[i];
                    var midY = y + rowH / 2;
                    var textY = midY - 10f * s;
                    var isNew = TimeUtils.IsNew(entry.FirstDetectionDate);
                    var entryColor = isNew ? newColor : fg;

                    if (isNew)
                    {
                        frame.Add(new MySprite
                        {
                            Type = SpriteType.TEXTURE, Data = "Circle",
                            Position = new Vector2(dotX, midY),
                            Size = Vector2.One * dotR * 2f,
                            Color = newColor, Alignment = TextAlignment.CENTER
                        });
                    }

                    var displayName = string.IsNullOrEmpty(entry.CustomName)
                        ? entry.BaseName
                        : $"{entry.BaseName} · {entry.CustomName}";

                    frame.Add(new MySprite
                    {
                        Type = SpriteType.TEXT, Data = displayName,
                        Color = entryColor,
                        Position = new Vector2(nameColX, textY),
                        Alignment = TextAlignment.LEFT, RotationOrScale = fontSize
                    });

                    var dist = (long)Vector3D.Distance(entry.Position, selfPosition);
                    frame.Add(new MySprite
                    {
                        Type = SpriteType.TEXT, Data = $"{dist}m",
                        Color = dim,
                        Position = new Vector2(distColX, textY),
                        Alignment = TextAlignment.RIGHT, RotationOrScale = fontSize
                    });

                    var age = TimeUtils.FormatAge(entry.UpdateDate, shortFormat: true);
                    frame.Add(new MySprite
                    {
                        Type = SpriteType.TEXT, Data = age,
                        Color = dim,
                        Position = new Vector2(ageColX, textY),
                        Alignment = TextAlignment.RIGHT, RotationOrScale = fontSize
                    });

                    y += rowH;

                    if ((i - from) % 10 == 9)
                        yield return true;
                }
            }

            frame.Dispose();
            yield return false;
        }

        private List<IMapEntry> GetSortedEntries(Vector3D selfPosition)
        {
            var entries = _mapEntryRepository.GetAll<IMapEntry>();
            switch (_viewSettings.SortMode)
            {
                case DatabaseSortMode.Name:
                    return entries.OrderBy(e =>
                        string.IsNullOrEmpty(e.CustomName) ? e.BaseName : e.CustomName
                    ).ToList();
                case DatabaseSortMode.Age:
                    return entries.OrderByDescending(e => e.UpdateDate).ToList();
                case DatabaseSortMode.New:
                    return entries.OrderByDescending(e => e.FirstDetectionDate).ToList();
                default:
                    return entries.OrderBy(e =>
                        Vector3D.DistanceSquared(e.Position, selfPosition)
                    ).ToList();
            }
        }
    }
}
