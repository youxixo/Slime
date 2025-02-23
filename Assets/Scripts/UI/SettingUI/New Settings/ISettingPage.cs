using UnityEngine;

/// <summary>
/// Maybe need to have another layer of inheritance: SettingPageParent, have this interface and let pages to inheritance from there
/// </summary>
public interface ISettingPage
{
    public void SetPlayerSettingData(ref PlayerSettingsData data);
    public void LoadFromPSD(PlayerSettingsData data);
    public void ApplyChanges(PlayerSettingsData changingData);
    public void DoNotSaveChanges();
    public void ChangeToPlayerSettingData(PlayerSettingsData changingData);
    public void Activate();
    public void Deactivate();
}
