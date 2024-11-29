using UniRx;
using System;
using UnityEngine;
using System.Collections;

public class Hunter : BaseEnemyShip
{
    [SerializeField] private Bullet _bulletPref;
    [SerializeField] private float _coolDown = 1.3f;
    private float _coolDownCurrent = 10.0f;
    private StageShip _currentShip;

    private Subject<(Transform, Bullet)> _fire = new Subject<(Transform, Bullet)> ();
    public UniRx.IObservable<(Transform, Bullet)> Fire => _fire;

    private IEnumerator LocalUpdate()
    {
        while (_currentShip == StageShip.Wait)
        {
            if(_coolDownCurrent < _coolDown)
            {
                _coolDownCurrent += Time.deltaTime;
            }
            else
            {
                _coolDownCurrent = 0;
                _fire.OnNext((transform, _bulletPref));
            }
            yield return null;
        }
    }
    protected override void UpdateStage(StageShip stage)
    {
        _currentShip = stage;
        if(stage == StageShip.Wait)
        {
            StartCoroutine(LocalUpdate());
        }
    }
}
