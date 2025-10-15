using System;
using System.Collections.Generic;

namespace IngameScript
{
    public class ShipSystem : IShipSystem
    {
        private readonly Program _program;
        private readonly IEventBus<ISpaceMapEvent> _eventBus;
        private readonly IDetectionDataStore _detectionDataStore;
        private readonly ShipBindings _bindings;
        private readonly IUserSettingsRepository _userSettingsRepository;

        private bool _systemReady;

        public ShipSystem(Program program)
        {
            _program = program;
            _eventBus = program.EventBus;
            _bindings = new ShipBindings();
            _detectionDataStore = program.StoreManager.GetStore<IDetectionDataStore>();
            _userSettingsRepository = program.RepositoryManager.GetRepository<IUserSettingsRepository>();

            _program.EventBus.RegisterConsumer(@event =>
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
                _detectionDataStore.RaycastCharge = (float)camera.AvailableScanRange / scanDistance;
            }
            else
            {
                _detectionDataStore.RaycastCharge = 1;
                var result = camera.Raycast(scanDistance);
                if (result.IsEmpty())
                {
                    _detectionDataStore.DetectedEntityInfo = null;
                }
                else
                {
                    if (_detectionDataStore.DetectedEntityInfo.HasValue
                        && _detectionDataStore.DetectedEntityInfo.Value.EntityId == result.EntityId
                       )
                    {
                        yield return false;
                        yield break;
                    }

                    _detectionDataStore.DetectedEntityInfo = result;
                    _eventBus.Produce(new EntityDetectedEvent(result));
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