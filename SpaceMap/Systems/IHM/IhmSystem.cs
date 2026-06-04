using System.Collections.Generic;

namespace IngameScript
{
    public class IhmSystem : IIhmSystem
    {
        private readonly Program _program;
        private readonly IhmBindings _bindings;
        private readonly Dictionary<DisplayMode, IIhmModule> _modules;
        private readonly DisplayCycleSettings _cycleSettings;
        private readonly Queue<IEnumerator<bool>> _renderQueue = new Queue<IEnumerator<bool>>();

        public IhmSystem(Program program)
        {
            _program = program;
            _bindings = new IhmBindings();
            _cycleSettings = program.Container.GetItem<DisplayCycleSettings>();
            _modules = new Dictionary<DisplayMode, IIhmModule>
            {
                { DisplayMode.Map,       new MapIhmModule(_program) },
                { DisplayMode.Map3D,     new Map3DIhmModule(_program) },
                { DisplayMode.Database,  new DatabaseIhmModule(_program) },
                { DisplayMode.Detection, new DetectionIhmModule(_program) },
                { DisplayMode.Help,      new HelpIhmModule(_program) },
                { DisplayMode.Sync,      new SyncIhmModule(_program) },
            };

            var eventStream = program.Container.GetItem<IEventStream<ISpaceMapEvent>>();
            eventStream.RegisterConsumer(@event =>
            {
                if (@event is BlocDetectionPulseEvent)
                    HandleBlocDetectionPulseEvents((BlocDetectionPulseEvent)@event);
            });
        }

        public void RefreshCycleSurfaces()
        {
            IIhmModule module;
            if (!_modules.TryGetValue(_cycleSettings.CurrentMode, out module)) return;

            foreach (var panel in _bindings.Panels)
            {
                foreach (var surface in panel.Surfaces)
                {
                    if (surface.Mode == DisplayMode.Cycle)
                        module.InitSurface(panel, surface);
                }
            }
        }

        private DisplayMode ResolveMode(DisplayMode mode)
        {
            return mode == DisplayMode.Cycle ? _cycleSettings.CurrentMode : mode;
        }

        private void HandleBlocDetectionPulseEvents(BlocDetectionPulseEvent message)
        {
            _bindings.SearchScreens(_program);

            foreach (var panel in _bindings.Panels)
            {
                foreach (var surface in panel.Surfaces)
                {
                    IIhmModule module;
                    if (_modules.TryGetValue(ResolveMode(surface.Mode), out module))
                        module.InitSurface(panel, surface);
                }
            }
        }

        public IEnumerator<bool> Run()
        {
            _renderQueue.Clear();
            foreach (var panel in _bindings.Panels)
            {
                foreach (var surface in panel.Surfaces)
                {
                    IIhmModule module;
                    if (_modules.TryGetValue(ResolveMode(surface.Mode), out module))
                        _renderQueue.Enqueue(module.RenderTo(panel, surface));
                }
            }

            while (_renderQueue.Count > 0)
            {
                var coroutine = _renderQueue.Dequeue();
                while (coroutine.MoveNext())
                {
                    yield return true;
                }
            }

            yield return false;
        }
    }
}
