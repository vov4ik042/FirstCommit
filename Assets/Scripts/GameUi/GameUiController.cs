using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUiController : MonoBehaviour
{
    [SerializeField] private Slider _barHealth;
    [SerializeField] private Text _countScore;
    [SerializeField] private GameObject _WindowGameOver;
    [SerializeField] private TMP_Text _timerScore;
    [SerializeField] private Button _buttonRestart;
    [SerializeField] private Button _buttonMainMenu;

    private float _elapsedTime = 0f; // Время, прошедшее с начала игры
    private bool _isRunning = true;
    //[SerializeField] private Text _countHealth;//?

    private CompositeDisposable _disposable = new CompositeDisposable();
    private void Start()
    {
        StartTimer();
        var controller = Controller.Instance;
        controller.OnGameOver.Subscribe(_ => ShowWindowGameOver()).AddTo(_disposable);
        controller.myShip.health.Subscribe(UpdateBar).AddTo(_disposable);//ADDto также дает возможность подписаться на compositedisposable
        controller.score.Subscribe(UpdateScore).AddTo(_disposable);
    }
    private void OnEnable()
    {
        _buttonRestart.onClick.AddListener(ClickRestart);
        _buttonMainMenu.onClick.AddListener(ClickToMainMenu);
    }
    private void OnDisable()
    {
        _buttonRestart.onClick.RemoveListener(ClickRestart);
        _buttonMainMenu.onClick.RemoveListener(ClickToMainMenu);
    }
    private void Update()
    {
        if (_isRunning)
        {
            _elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    public void StartTimer()
    {
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
    }
    private void UpdateTimerText()
    {
        // Преобразуем время в формат (например, 00:00.00)
        int minutes = Mathf.FloorToInt(_elapsedTime / 60); // Минуты
        int seconds = Mathf.FloorToInt(_elapsedTime % 60); // Секунды
        int milliseconds = Mathf.FloorToInt((_elapsedTime * 100) % 100); // Миллисекунды

        // Обновляем текст таймера
        _timerScore.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
    private void UpdateBar(int value)
    {
        _barHealth.value = ((float)value)/100;
        //_countHealth.text = value.ToString();//
    }

    private void UpdateScore(int score)
    {
        if (!_WindowGameOver.activeSelf)
        {
            _countScore.text = score.ToString();
        }
    }

    public void ShowWindowGameOver()
    {
        _countScore.text = Controller.Instance.score.ToString();
        UpdateTimerText();
        StopTimer();
        _WindowGameOver.SetActive(true);
    }

    public void ClickToMainMenu()
    {
        Controller.Instance.PlaySound(Controller.Instance._effectSound[0]);
        LevelManager.PlayScene(Scenes.MainMenu);
        gameObject.SetActive(false);
    }

    public void ClickRestart()
    {
        Controller.Instance.PlaySound(Controller.Instance._effectSound[0]);
        LevelManager.PlayScene(Scenes.Game);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if(_disposable != null)
        {
            _disposable.Dispose();
        }
        _disposable = null;//12. 10:27 Чтобы не было утечки данных
    }
}
