using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class HelpIhmModule : IIhmModule
    {
        private readonly Program _program;
        private ICommand[] _commands;

        private const float BaseSize = 512f;
        private const float Margin = 18f;
        private const float SignatureLineH = 21f;
        private const float DescLineH = 17f;
        private const float CommandGap = 5f;

        public HelpIhmModule(Program program)
        {
            _program = program;
        }

        public void InitSurface(Panel panel, PanelSurface surface)
        {
            surface.Surface.ContentType = ContentType.SCRIPT;
            surface.Surface.Script = "";
        }

        public IEnumerator<bool> RenderTo(Panel panel, PanelSurface surface)
        {
            if (_commands == null)
                _commands = _program.Container.GetItem<CommandSystem>().Commands;

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
                Type = SpriteType.TEXT, Data = "COMMANDS",
                Color = fg,
                Position = new Vector2(viewport.Center.X, y + headerH / 2 - 10f * s),
                Alignment = TextAlignment.CENTER, RotationOrScale = s * 1.1f
            });
            y += headerH + 10f * s;

            foreach (var command in _commands)
            {
                var lines = command.GetUsage().Split('\n');

                // Command signature (line 0)
                frame.Add(new MySprite
                {
                    Type = SpriteType.TEXT, Data = lines[0],
                    Color = accent,
                    Position = new Vector2(viewport.X + margin, y),
                    Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.72f
                });
                y += SignatureLineH * s;

                // Description (line 1 only — skip examples to save space)
                if (lines.Length > 1)
                {
                    var desc = lines[1].TrimStart();
                    if (!string.IsNullOrEmpty(desc))
                    {
                        frame.Add(new MySprite
                        {
                            Type = SpriteType.TEXT, Data = desc,
                            Color = dim,
                            Position = new Vector2(viewport.X + margin + 6f * s, y),
                            Alignment = TextAlignment.LEFT, RotationOrScale = s * 0.62f
                        });
                        y += DescLineH * s;
                    }
                }

                y += CommandGap * s;
            }

            frame.Dispose();
            yield return false;
        }
    }
}
