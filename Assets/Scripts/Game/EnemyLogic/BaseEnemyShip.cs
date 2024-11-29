using UniRx;
using UnityEngine;
using System.Collections;

public enum StageShip
{
    In,
    Wait,
    Out
}

public abstract class BaseEnemyShip : MonoBehaviour
{
    [SerializeField] private float _normalSpeed = 6.0f; 
    [SerializeField] private float _delayTurbo = 4.0f;//Время которое корабль будет думать что ему делать
    [SerializeField] private float _turboSpeed = 5.0f;
    [SerializeField] private float _speedRotation = 0.07f;
    [SerializeField] private int _collissionDamage = 10;
    [SerializeField] private int _maxHealth = 4;
    [SerializeField] private int _costPointesScore = 10;
    [SerializeField] private ParticleSystem _engineEffect;
    [SerializeField] private GameObject _enemyBoomEffect;
    
    public int CostPointessScore => _costPointesScore;//Свойство

    [HideInInspector] public PlayerShip _player;
    [HideInInspector] public Transform _myRoot;
    [HideInInspector] public Vector3 _playerLastPose = Vector3.up;

    private Subject<MonoBehaviour> _putMe = new Subject<MonoBehaviour>();
    public UniRx.IObservable<MonoBehaviour> PutMe => _putMe;

    private Vector3 DirectionToPlayer => transform.position - new Vector3(_playerLastPose.x, _playerLastPose.y, 0);
    private int _health = 100;
    private float _goto;//Конечная точка движения вражеского корабля
    private float _gotoPointTurbo;//Точка, на которой корабль будет следить за кораблем игрока
    private float _timerDelay;//Текущее показание таймера

    private IEnumerator Core()
    {
        UpdateStage(StageShip.In);
        _engineEffect.Play();
        while(transform.position.y > _gotoPointTurbo)
        {
            Look(new Vector3(0, _gotoPointTurbo, 0));
            transform.position -= new Vector3(0, Time.deltaTime * _normalSpeed, 0);
            yield return null;
        }

        UpdateStage(StageShip.Wait);
        _engineEffect.Stop();
        while (_timerDelay < _delayTurbo)
        {
            _timerDelay += Time.deltaTime;
            yield return null;
        }

        UpdateStage(StageShip.Out);
        _engineEffect.Play();
        if (_playerLastPose != Vector3.up)
        {
            var dir = DirectionToPlayer / DirectionToPlayer.magnitude;
            while (transform.position.y > _goto && transform.position.y < -_goto)
            {
                Look(dir);
                transform.position -= dir * (Time.deltaTime * _turboSpeed);
                yield return null;
            }
        }
        else
        {
            while (transform.position.y > _goto)
            {
                transform.position -= new Vector3(0, Time.deltaTime * _turboSpeed, 0);
                yield return null;
            }
        }

        _putMe.OnNext(this);

    }

    private void OnEnable()
    {
        _timerDelay = 0;
        var controller = Controller.Instance;
        _goto = controller.RightDownPoint.y - 2;
        _gotoPointTurbo = UnityEngine.Random.Range((controller.CentrCam.y + 1), (controller.RightUpPoint.y - 1));
        _health = _maxHealth;
        StartCoroutine(Core());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    protected void Look(Vector3 dir, bool lerp = false, bool inversion = false)
    {
        float signedAngle = Vector2.SignedAngle(Vector2.down, dir);
        if (inversion == true) signedAngle += 180;
        if(Mathf.Abs(signedAngle) >= 1e-3f)
        {
            var angles = transform.eulerAngles;
            angles.z = signedAngle;
            if (lerp)
            {
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, angles, _speedRotation);
            }
            else
            {
                transform.eulerAngles = angles;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Bullet"))
        {
            var bull = obj.GetComponent<Bullet>();
            bull.Hitme();
            DamageMe(bull._damage, this);
            return;
        }
        if (obj.CompareTag("Player"))
        {
            //Controller.Instance.PlaySound(Controller.Instance._effectSound[3]);
            SpawnDestroyEffect();
            obj.GetComponent<PlayerShip>().DamageMe(_collissionDamage);
            Controller.Instance.score.Value += (_costPointesScore / 2);
            _putMe.OnNext(this);
        }
    }
    private void DamageMe(int damage, BaseEnemyShip baseEnemy)
    {
        _health -= damage;
        if(_health <= 0)
        {
            Controller.Instance.PlaySound(Controller.Instance._effectSound[6]);
            SpawnDestroyEffect();
            _health = _maxHealth;
            SpawnBonus();
            Controller.Instance.score.Value += _costPointesScore;
            _putMe.OnNext(this);
        }
    }

    private void SpawnDestroyEffect()
    {
        var pos = transform.position;
        GameObject effect = Instantiate(_enemyBoomEffect, new Vector3(pos.x, pos.y, -2), transform.rotation);
        Destroy(effect, 2f);
    }


    protected abstract void UpdateStage(StageShip ship);

    private void SpawnBonus()
    {
        var random = UnityEngine.Random.Range(0, 100);
        if(random < Controller.Instance.procentBonusHealth)
        {
            Instantiate(Controller.Instance.HealthBounsPref, transform.position, new Quaternion(0,0,0,0));
        }
    }
}
