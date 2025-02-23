using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : SettingPageParent
{
    [SerializeField] private Slider masterAudioSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public override void SetPlayerSettingData(ref PlayerSettingsData data)
    {
        data.masterVolume = (int)masterAudioSlider.value;
        data.musicVolume = (int)musicSlider.value;
        data.sfxVolume = (int)sfxSlider.value;
    }

    public override void ApplyChanges(PlayerSettingsData changingData)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ChangeMixerValue("MasterVolume", (int)masterAudioSlider.value);
            AudioManager.Instance.ChangeMixerValue("BGMVolume", (int)musicSlider.value);
            AudioManager.Instance.ChangeMixerValue("SFXVolume", (int)sfxSlider.value);
        }

        changingData.masterVolume = (int)masterAudioSlider.value;
        changingData.musicVolume = (int)musicSlider.value;
        changingData.sfxVolume = (int)sfxSlider.value;
    }

    public override void LoadFromPSD(PlayerSettingsData data)
    {
        masterAudioSlider.value = data.masterVolume;
        musicSlider.value = data.musicVolume;
        sfxSlider.value = data.sfxVolume;
    }

    public override void ChangeToPlayerSettingData(PlayerSettingsData changingData)
    {
        AudioManager.Instance.ChangeMixerValue("MasterVolume", (int)changingData.masterVolume);
        AudioManager.Instance.ChangeMixerValue("BGMVolume", (int)changingData.musicVolume);
        AudioManager.Instance.ChangeMixerValue("SFXVolume", (int)changingData.sfxVolume);
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
