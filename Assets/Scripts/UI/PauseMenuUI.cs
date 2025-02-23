using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PauseMenuUI : PanelParent
{
    public GameObject selectedButtonOnPause; 

    [Header("Events")]
    public static UnityEvent resumeGameEvent = new UnityEvent();

    private void Start()
    {
        SettingUIController.finishSetting.AddListener(BackFromSetting);
    }

    private void BackFromSetting()
    {
        if (this.enabled)
            EventSystem.current.SetSelectedGameObject(selectedButtonOnPause);
    }

    public void Resume()
    {
        this.gameObject.SetActive(false);
        resumeGameEvent.Invoke();
    }

    public void OpenSetting()
    {
        MainUIController.Instance.OpenSetting();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
