using UnityEngine.UI;
using UnityEngine;

public class SettingsWindow : MonoBehaviour
{
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Slider _sliderEffect;
    [SerializeField] private Button _buttonOk;
    private void OnEnable()
    {
        _buttonOk.onClick.AddListener(ClickButtonOk);
    }
    private void OnDisable()
    {
        _buttonOk.onClick.RemoveListener(ClickButtonOk);
    }
    public void PlayerPrefMusic()
    {
        if (PlayerPrefs.HasKey("MusicPref") == true && PlayerPrefs.HasKey("SoundPref") == true)
        {
            _sliderMusic.value = PlayerPrefs.GetFloat("MusicPref");
            _sliderEffect.value = PlayerPrefs.GetFloat("SoundPref");
        }
        else
        {
            _sliderMusic.value = Controller.Instance.AudioSource.volume;
            _sliderEffect.value = Controller.Instance.SoundSource.volume;
        }
    }
    public void ChangeValueMusic(float value)
    {
        Controller.Instance.AudioSource.volume = value;
        PlayerPrefs.SetFloat("MusicPref", value);
        
    }

    public void ChangeValueEffect(float value)
    {
        Controller.Instance.SoundSource.volume = value;
        PlayerPrefs.SetFloat("SoundPref", value);
    }

    public void ClickButtonOk()
    {
        Controller.Instance.PlaySound(Controller.Instance._effectSound[0]);
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }
}
