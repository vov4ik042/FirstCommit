using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private SettingsWindow _settingsWindows;
    [SerializeField] private Button _buttonStart;
    [SerializeField] private Button _buttonSettings;
    [SerializeField] private Button _buttonExit;

    private void Awake()
    {
        _settingsWindows.PlayerPrefMusic();
        Controller.Instance.StartPlayMusic();
    }
    private void OnEnable()
    {
        _buttonStart.onClick.AddListener(ClickStart);
        _buttonSettings.onClick.AddListener(ClickSettings);
        _buttonExit.onClick.AddListener(ClickExit);
    }
    private void OnDisable()
    {
        _buttonStart.onClick.RemoveListener(ClickStart);
        _buttonSettings.onClick.RemoveListener(ClickSettings);
        _buttonExit.onClick.RemoveListener(ClickExit);
    }
    public void ClickStart()
    {
        Controller.Instance.PlaySound(Controller.Instance._effectSound[0]);
        LevelManager.PlayScene(Scenes.Game);
    }
    public void ClickSettings()
    {
        Controller.Instance.PlaySound(Controller.Instance._effectSound[0]);
        _settingsWindows.gameObject.SetActive(true);
    }
    public void ClickExit()
    {
        Controller.Instance.PlaySound(Controller.Instance._effectSound[0]);
        Application.Quit();
    }
}
