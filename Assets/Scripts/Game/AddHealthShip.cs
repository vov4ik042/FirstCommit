using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AddHealthShip : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _turboSpeed = 15.0f;
    [SerializeField] private float _stopDistance = 0.5f;
    [SerializeField] private int _addHealth;

    private Transform _healthBar;
    private Vector3 _goTo;
    public int Health => _addHealth;

    private void OnEnable()
    {
        _healthBar = GameObject.Find("Background").transform;
        Controller controller = Controller.Instance;
        _goTo = new Vector3(0, controller.RightDownPoint.y - 2, 0);
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while(transform.position.y > _goTo.y)
        {
            transform.position -= new Vector3(0, Time.deltaTime * _speed, 0);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void CallMoveToHealthBar()
    {
        StopAllCoroutines();
        _goTo = _healthBar.transform.position;
        var g = GetComponent<CircleCollider2D>();
        g.enabled = false;
        StartCoroutine(MoveToHealthBar());
    }

    private IEnumerator MoveToHealthBar()
    {
        var tr = transform;
        var position = tr.position;
        var absoluteDir = position - _goTo;
        var dirNormalized = absoluteDir / absoluteDir.magnitude;
        while(Vector3.Distance(transform.position, _goTo) > _stopDistance)
        {
            transform.position -= dirNormalized * (Time.deltaTime * _turboSpeed);
            yield return null;
        }
        Destroy(gameObject);
    }
}
