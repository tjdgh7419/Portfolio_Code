using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : PoolAble
{
    [SerializeField] private BoxCollider2D _collider;
    public float MeeleDamage { get; private set; } = 0;
    private LayerMask targetLayer;
    private Coroutine co = null;

    private Action<Collider2D> _hitCallback;

    IEnumerator CoRelease()
    {
        yield return new WaitForSeconds(0.5f);

        ReleaseObject();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {     
        if (targetLayer.value == (targetLayer.value | (1 << collision.gameObject.layer)))
        {
            if (collision.TryGetComponent(out IDamagable obj))
            {
                if (!obj.IsDeath())
                {
                    obj.TakeDamage(MeeleDamage);
                    ObjectPoolManager.Instance.GetGo(PoolType.HitEffect).transform.position = collision.transform.position;
                    _hitCallback?.Invoke(collision);
                    _collider.enabled = false;
                }                                        
            }
        }

    }

    public void Initialize(float rotZ, float damage, LayerMask layer)
    {
        if (co != null)
            StopCoroutine(co);

        _collider.enabled = true;

        transform.rotation = Quaternion.Euler(0, 0, rotZ);
        MeeleDamage = damage;
        targetLayer = layer;

        co = StartCoroutine(CoRelease());
    }
    
    public void Initialize(float rotZ, float damage, LayerMask layer, Action<Collider2D> hitCallback)
    {   
        _hitCallback = hitCallback;
        Initialize(rotZ,damage,layer);
    }
}