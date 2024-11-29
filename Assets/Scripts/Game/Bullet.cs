using UnityEngine;
using UniRx;
using System;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 13;

    public int _damage = 1;

    private Subject<MonoBehaviour> _putMe = new Subject<MonoBehaviour>();
    public UniRx.IObservable<MonoBehaviour> PutMe => _putMe;

    private float _goTo;

    public bool _isEnemy;

    private void OnEnable()
    {
        var controller = Controller.Instance;
        _goTo = controller.LeftUpPoint.y + 2;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        if(_isEnemy)
        {
            while (transform.position.y > -_goTo)
            {
                transform.position -= new Vector3(0, Time.deltaTime * _speed, 0);
                yield return null;
            }
        }
        else
        {
            while(transform.position.y < _goTo)
            {
                transform.position += new Vector3(0, Time.deltaTime * _speed, 0);
                yield return null;
            }
        }
        _putMe.OnNext(this);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Hitme()
    {
        _putMe.OnNext(this);//Удаляем
    }
}
