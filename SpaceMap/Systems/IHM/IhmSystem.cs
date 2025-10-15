using System.Collections.Generic;

namespace IngameScript
{
    public class IhmSystem : IIhmSystem
    {
        private readonly Program _program;
        private readonly IhmBindings _bindings;
        private readonly Dictionary<DisplayMode, IIhmModule> _modules;
        private readonly Queue<IEnumerator<bool>> _renderQueue = new Queue<IEnumerator<bool>>();

        public IhmSystem(Program program)
        {
            _program = program;
            _bindings = new IhmBindings();
            _modules = new Dictionary<DisplayMode, IIhmModule>
            {
                { DisplayMode.General, new GeneralIhmModule(_program) },
                { DisplayMode.Database, new DatabaseIhmModule() },
                { DisplayMode.Map, new MapIhmModule(_program) },
                { DisplayMode.Detection, new DetectionIhmModule(_program) },
            };

            _program.EventBus.RegisterConsumer(@event =>
            {
                if (@event is BlocDetectionPulseEvent)
                    HandleBlocDetectionPulseEvents((BlocDetectionPulseEvent)@event);
            });
        }

        private void HandleBlocDetectionPulseEvents(BlocDetectionPulseEvent message)
        {
            _bindings.SearchScreens(_program);

            foreach (var panel in _bindings.Panels)
            {
                foreach (var surface in panel.Surfaces)
                {
                    _modules[surface.Mode].InitSurface(panel, surface);
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
                    _renderQueue.Enqueue(_modules[surface.Mode].RenderTo(panel, surface));
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