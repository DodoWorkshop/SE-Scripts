using System;
using System.Collections.Generic;

namespace IngameScript
{
    public class ShipSystem : IShipSystem
    {
        private readonly Program _program;
        private readonly IEventSink<ISpaceMapEvent> _eventSink;
        private readonly IDetectionDataRepository _detectionDataRepository;
        private readonly ShipBindings _bindings;
        private readonly IUserSettingsRepository _userSettingsRepository;

        private bool _systemReady;

        public ShipSystem(Program program)
        {
            _program = program;
            _bindings = new ShipBindings();
            _detectionDataRepository = program.Container.GetItem<IDetectionDataRepository>();
            _userSettingsRepository = program.Container.GetItem<IUserSettingsRepository>();
            _eventSink = program.Container.GetItem<IEventSink<ISpaceMapEvent>>();

            var eventStream = program.Container.GetItem<IEventStream<ISpaceMapEvent>>();
            eventStream.RegisterConsumer(@event =>
            {
                if (@event is BlocDetectionPulseEvent)
                    HandleBlocDetectionPulseEvents((BlocDetectionPulseEvent)@event);
            });
        }

        private void HandleBlocDetectionPulseEvents(BlocDetectionPulseEvent message)
        {
            LoadRequirements();
        }

        private void LoadRequirements()
        {
            if (_bindings.TryBind(_program))
            {
                _bindings.Camera.EnableRaycast = true;
                _systemReady = true;
                return;
            }

            _systemReady = false;
        }

        private IEnumerator<bool> HandleDetection()
        {
            if (!_systemReady)
            {
                yield return false;
                yield break;
            }

            var camera = _bindings.Camera;
            if (camera == null || !camera.IsWorking)
            {
                _systemReady = false;
                throw new Exception("Failed to run detection: No functional camera");
            }

            var scanDistance = _userSettingsRepository.DetectionDistance;
            if (!camera.CanScan(scanDistance))
            {
                _detectionDataRepository.RaycastCharge = (float)camera.AvailableScanRange / scanDistance;
            }
            else
            {
                _detectionDataRepository.RaycastCharge = 1;
                var result = camera.Raycast(scanDistance);
                if (result.IsEmpty())
                {
                    _detectionDataRepository.DetectedEntityInfo = null;
                }
                else
                {
                    if (_detectionDataRepository.DetectedEntityInfo.HasValue
                        && _detectionDataRepository.DetectedEntityInfo.Value.EntityId == result.EntityId
                       )
                    {
                        yield return false;
                        yield break;
                    }

                    _detectionDataRepository.DetectedEntityInfo = result;
                    _eventSink.Produce(new EntityDetectedEvent(result));
                }
            }

            yield return false;
        }

        public string Group => SystemGroups.Logic;

        public IEnumerator<bool> Run()
        {
            return HandleDetection();
        }
    }
}