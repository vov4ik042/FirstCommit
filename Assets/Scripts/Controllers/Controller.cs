using UnityEngine;
using UniRx;
using System;

public class Controller : MonoBehaviour
{
    public AddHealthShip HealthBounsPref;
    public int procentBonusHealth = 30;

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundSource;
    public AudioClip[] _effectSound;

    public ReactiveProperty<int> score = new ReactiveProperty<int>();
    public AudioSource AudioSource => _musicSource;
    public AudioSource SoundSource => _soundSource;

    public static Controller Instance;

    public PlayerShip myShip;

    private Vector3 _leftDownPoint;
    private Vector3 _leftUpPoint;
    private Vector3 _RightDownPoint;
    private Vector3 _RightUpPoint;
    private Vector2 _centrCam;

    public Vector3 LeftDownPoint => _leftDownPoint;
    public Vector3 LeftUpPoint => _leftUpPoint;
    public Vector3 RightDownPoint => _RightDownPoint;
    public Vector3 RightUpPoint => _RightUpPoint;
    public Vector3 CentrCam => _centrCam;

    private Subject<Unit> _gameOver = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnGameOver => _gameOver;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySound(AudioClip clip)
    {
        _soundSource.PlayOneShot(clip);
    }
    public void StartPlayMusic()
    {
        _musicSource.Play();
    }

    public void UpdateCameraSetting()
    {
        var cameraMain = Camera.main;
        if(cameraMain != null )
        {
            _centrCam = cameraMain.transform.position;

            float distance = cameraMain.farClipPlane;
            _leftDownPoint = cameraMain.ScreenToWorldPoint(new Vector3(0,0, distance));
            _leftUpPoint = cameraMain.ScreenToWorldPoint(new Vector3(0, cameraMain.pixelHeight, distance));
            _RightUpPoint = cameraMain.ScreenToWorldPoint(new Vector3(cameraMain.pixelWidth, cameraMain.pixelHeight, distance));
            _RightDownPoint = cameraMain.ScreenToWorldPoint(new Vector3(cameraMain.pixelWidth, 0, distance));
        }
    }

    public void GameOver()
    {
        PlaySound(_effectSound[2]);
        _gameOver.OnNext(Unit.Default);
    }
}
