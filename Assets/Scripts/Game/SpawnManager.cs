using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    private PlayerShip _playerShip;
    [SerializeField] private GameObject _bulletPref;
    [SerializeField] private Transform _poolBulletMy;
    [SerializeField] private Transform _poolEnemyRoot;
    [SerializeField] private Transform _poolEnemyBullet;
    [SerializeField] private List<GameObject> _enemyPrefabs = new List<GameObject>();

    private List<Transform> _rootEnemyType = new List<Transform>();
    private CompositeDisposable _disposable = new CompositeDisposable();
    private void Start()
    {
        Controller.Instance.score.Value = 0;
        _playerShip = Controller.Instance.myShip;
        _playerShip.FireClick.Subscribe((_) => SpawnBullet());

        foreach(var enemy in _enemyPrefabs)
        {
            GameObject root = new GameObject("root" + enemy.name);
            root.transform.parent = _poolEnemyRoot;
            _rootEnemyType.Add(root.transform);
        }
    }

    public void SpawnBullet(Transform enemyTransfrom = null, Bullet enemyBullet = null)
    {
        GameObject bullet;

        if(enemyTransfrom != null && enemyBullet != null)
        {
            if(_poolEnemyBullet.childCount > 0)
            {
                bullet = _poolEnemyBullet.GetChild(0).gameObject;
            }
            else
            {
                bullet = Instantiate(enemyBullet).gameObject;
                var bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.PutMe.Subscribe(PutObject).AddTo(_disposable);
            }
            bullet.transform.parent = transform;
            var pos = enemyTransfrom.transform.position;
            bullet.transform.position = new Vector3(pos.x, pos.y - 1.2f, 0);
        }
        else
        {
            Controller.Instance.PlaySound(Controller.Instance._effectSound[1]);
            if (_poolBulletMy.childCount > 0)
            {
                bullet = _poolBulletMy.GetChild(0).gameObject;
            }
            else
            {
                bullet = Instantiate(_bulletPref);
                bullet.GetComponent<Bullet>().PutMe.Subscribe(PutObject).AddTo(_disposable);
            }
            bullet.transform.parent = transform;
            var pos = _playerShip.transform.position;
            bullet.transform.position = new Vector3(pos.x, pos.y + 1.2f, 0);
        }
        bullet.gameObject.SetActive(true);
    }

    public Hunter SpawnEnemy()
    {
        var controller = Controller.Instance;
        GameObject ship;
        int type = Random.Range(0,_enemyPrefabs.Count);
        var pool = _rootEnemyType[type];

        if(pool.childCount > 0)
        {
            ship = pool.GetChild(0).gameObject;
        }
        else
        {
            ship = Instantiate(_enemyPrefabs[type]);
            var enemyShip = ship.GetComponent<BaseEnemyShip>();
            enemyShip.PutMe.Subscribe(PutObject).AddTo(_disposable);
            enemyShip._myRoot = pool;
            enemyShip._player = _playerShip;
        }

        ship.transform.parent = _poolEnemyRoot;
        var height = controller.RightUpPoint.y;
        Vector3 spawnPos = new Vector3(Random.Range(controller.LeftUpPoint.x + 0.5f, controller.RightUpPoint.x - 0.5f), height - 1f, 0);

        ship.transform.position = spawnPos;
        ship.SetActive(true);

        return ship.GetComponent<Hunter>();
    }

    private void PutObject(MonoBehaviour mono)
    {
        var objBull = mono as Bullet;
        if(objBull != null)
        {
            if (objBull._isEnemy)
            {
                objBull.transform.parent = _poolEnemyBullet;
            }
            else
            {
                objBull.transform.parent = _poolBulletMy;
            }
            objBull.gameObject.SetActive(false);
            return;
        }

        var objShip = mono as BaseEnemyShip;
        if(objShip != null)
        {
            objShip.transform.parent = objShip._myRoot;
            objShip.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        _disposable = new CompositeDisposable();
    }

    private void OnDisable()
    {
        _disposable.Dispose();
        _disposable = null;
    }
}
