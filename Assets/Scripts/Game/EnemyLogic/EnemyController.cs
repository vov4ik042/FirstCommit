using UnityEngine;
using UniRx;
using System.Collections;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _minDelay = 2;//time seconds
    [SerializeField] private float _maxDelay = 4;//time seconds
    [SerializeField] private int _maxOneSpawnEnemy = 5;
    private float _timerDelay;//Текущее время таймера
    private int _countOnePull;//Текущее кол-во кораблей который будут созданы
    private SpawnManager _spawnManager;
    private CompositeDisposable _disposablesEnemy = new CompositeDisposable();
    private Coroutine _coroutine;

    private void Awake()
    {
        _spawnManager = GetComponent<SpawnManager>();
        _timerDelay = Random.Range(_minDelay, _maxDelay);
    }

    private void OnEnable()
    {
        _disposablesEnemy = new CompositeDisposable();
        _coroutine = StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        while (true) { 
            _timerDelay -= Time.deltaTime;
            if(_timerDelay < 0)
            {
                _countOnePull = Random.Range(1, _maxOneSpawnEnemy);
                _timerDelay = Random.Range(_minDelay, _maxDelay);
                for(int i = 0; i < _countOnePull; i++)
                {
                    var hunter = _spawnManager.SpawnEnemy();
                    if(hunter != null)
                    {
                        hunter.Fire.Subscribe((param) => Fire(param.Item1, param.Item2)).AddTo(_disposablesEnemy);
                    }
                    yield return null;
                }
                _countOnePull = Random.Range(1, _maxOneSpawnEnemy);
            }
            yield return null;
        }
    }

    private void Fire(Transform tr, Bullet bullet)
    {
        _spawnManager.SpawnBullet(tr, bullet); 
    }

    private void OnDisable()
    {
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _disposablesEnemy.Dispose();
        _disposablesEnemy = null;
    }
}
