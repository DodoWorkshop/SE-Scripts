namespace IngameScript
{
    public interface IUserSettingsRepository : ISavableRepository
    {
        uint DetectionDistance { get; set; }

        uint MapScale { get; set; }
    }
}