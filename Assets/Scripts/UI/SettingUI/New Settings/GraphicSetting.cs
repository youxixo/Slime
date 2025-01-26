using TMPro;
using UnityEditor;
using UnityEngine;

public class GraphicSetting : SettingPageParent
{
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private FullScreenMode GetScreenMode(int screenMode)
    {
        string screenModeText = screenModeDropdown.options[screenMode].text;
        if (screenModeText == "Fullscreen")
        {
            return FullScreenMode.FullScreenWindow;
        }
        else if (screenModeText == "Windowed")
        {
            return FullScreenMode.Windowed;
        }
        else if (screenModeText == "Borderless Windowed")
        {
            return FullScreenMode.MaximizedWindow;
        }
        Debug.LogWarning("Screen Mode有問題, 默認全屏");
        return FullScreenMode.FullScreenWindow;
    }

    public override void SetPlayerSettingData(ref PlayerSettingsData data)
    {
        data.resolutionMode = resolutionDropdown.value;
        data.screenSizeMode = screenModeDropdown.value;
    }

    public override void ApplyChanges(PlayerSettingsData changingData)
    {
        string[] resolutionList = resolutionDropdown.options[resolutionDropdown.value].text.Split("x");
        Screen.SetResolution(int.Parse(resolutionList[0].Trim()), int.Parse(resolutionList[1].Trim()), GetScreenMode(screenModeDropdown.value));

        changingData.resolutionMode = resolutionDropdown.value;
        changingData.screenSizeMode = screenModeDropdown.value;
    }

    public override void LoadFromPSD(PlayerSettingsData data)
    {
        screenModeDropdown.value = data.screenSizeMode;
        resolutionDropdown.value = data.resolutionMode;
    }

    public override void ChangeToPlayerSettingData(PlayerSettingsData changingData)
    {
        string[] resolutionList = resolutionDropdown.options[changingData.resolutionMode].text.Split("x");
        Screen.SetResolution(int.Parse(resolutionList[0].Trim()), int.Parse(resolutionList[1].Trim()), GetScreenMode(changingData.screenSizeMode));
    }

    public override void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    public override void DoNotSaveChanges()
    {

    }
}
