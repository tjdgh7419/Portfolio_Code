using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : PoolAble
{
    public float FireDamage { get; private set; } = 0;
    private LayerMask targetLayer;
    private float timer = 0;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > 2f)
        {  
            ReleaseObject();
            timer = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetLayer.value == (targetLayer.value | (1 << collision.gameObject.layer)))
        {
            if (collision.TryGetComponent(out IDamagable obj))
            {
                ObjectPoolManager.Instance.GetGo(PoolType.HitEffect).transform.position = collision.transform.position;
                obj.TakeDamage(FireDamage);          
            }
        }
    }

    public void Initialize(Vector2 dir, float rotZ, float range, float damage, float speed, LayerMask layer)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
 
        FireDamage = damage;

        targetLayer = layer;
    }
}
