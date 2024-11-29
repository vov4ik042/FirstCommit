using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;

public class PlayerShip : MonoBehaviour
{
    [SerializeField] private float _speed = 13.0f;
    [SerializeField] private float _coolDown = 0.1f;//Час перезарядки
    [SerializeField] private ParticleSystem _fireEffect;//Час перезарядки
    //[SerializeField] private float _shipRollEuler = 45.0f;//Максимальний угол нахилу
    //[SerializeField] private float _shipRollSpeed = 80.0f;//Швидкість нахилу
    [SerializeField] private float _smothness = 1.2f;//Плавність переміщення
    public int maxHealth = 100;

    private Subject<Unit> _fireClick = new Subject<Unit>();
    public UniRx.IObservable<Unit> FireClick => _fireClick;

    private Rigidbody2D _rigidbody;
    private float _coolDownCurrent = 10.0f;//Текущее время между пострілами
    private SpriteRenderer _mR;//Відповідає за відображення об'єкту у сцені
    private Vector3 _sizeWorldShip;//Размер корабля по 3 осям
    private Controller _controller;

    [HideInInspector] public ReactiveProperty<int> health = new ReactiveProperty<int>();

    private void Awake()
    {
        if(Controller.Instance == null)
        {
            SceneManager.LoadScene(0);
            return;
        }
        _rigidbody = GetComponent<Rigidbody2D>();
        _mR = GetComponent<SpriteRenderer>();
        _controller = Controller.Instance;
        _controller.myShip = this;//Чтобы скрипты имели ссылку на PlayerShip
        _sizeWorldShip = _mR.bounds.extents;
    }
    private void Start()
    {
        Controller.Instance.PlaySound(Controller.Instance._effectSound[4]);
        _controller.UpdateCameraSetting();
        health.Value = maxHealth;
    }

    private void Update()
    {
        UpdateKey();
        FireButtonClick();
    }

    public void FireButtonClick()
    {
        if(Input.GetMouseButton(0))
        {
            _fireEffect.Play();
            if(_coolDownCurrent >= _coolDown)
            {
                _coolDownCurrent = 0;
                _fireClick.OnNext(Unit.Default);
            }
        }
        if(_coolDownCurrent < _coolDown)
        {
            _coolDownCurrent += Time.deltaTime;
        }
    }

    private void UpdateKey()
    {

        float moveHoriz = Input.GetAxis("Horizontal");
        float moveVerti = Input.GetAxis("Vertical");

        _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, new Vector2(moveHoriz * _speed * 1.2f, moveVerti * _speed), _smothness);

        transform.position = CheckBoardWorld();

        //var targetRotation = Quaternion.Euler(0, 180 + (-moveHoriz * _shipRollEuler), 0);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _shipRollSpeed * Time.deltaTime);

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LevelManager.PlayScene(Scenes.MainMenu);
        }
    }

    private Vector3 CheckBoardWorld()
    {
        var pos = transform.position;//текущее местоположение обьекта
        var x = pos.x;
        var y = pos.y;

        x = Mathf.Clamp(x, _controller.LeftDownPoint.x + _sizeWorldShip.x, _controller.RightDownPoint.x - _sizeWorldShip.x);
        y = Mathf.Clamp(y, _controller.LeftDownPoint.y + _sizeWorldShip.y, _controller.LeftUpPoint.y - _sizeWorldShip.y);
        return new Vector3(x, y, 0);
    }

    public void DamageMe(int damage)
    {
        health.Value -= damage;
        if(health.Value <= 0)
        {
            var tr = transform;
            var pos = tr.position;
            gameObject.SetActive(false);
            _controller.GameOver();
        }
        else
        {
            Controller.Instance.PlaySound(Controller.Instance._effectSound[3]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if(obj.CompareTag("EnemyBullet"))
        {
            var bull = obj.GetComponent<Bullet>();
            DamageMe(bull._damage);
            bull.Hitme();
        }
        if (obj.CompareTag("AddHealthShip"))
        {
            Controller.Instance.PlaySound(Controller.Instance._effectSound[5]);
            var bonus = obj.GetComponent<AddHealthShip>();
            bonus.CallMoveToHealthBar();
            health.Value += bonus.Health;
            if(health.Value > maxHealth)
            {
                health.Value = maxHealth ;
            }
        }
    }
}
