using UnityEngine;

public abstract class SettingPageParent : MonoBehaviour, ISettingPage
{
    public GameObject firstSelectObj;

    public abstract void Activate();
    public abstract void ApplyChanges(PlayerSettingsData changingData);
    public abstract void DoNotSaveChanges();
    public abstract void ChangeToPlayerSettingData(PlayerSettingsData changingData);
    public abstract void Deactivate();
    public abstract void LoadFromPSD(PlayerSettingsData data);
    public abstract void SetPlayerSettingData(ref PlayerSettingsData data);
}
