using System;
using UnityEngine;

public class RangeAttack : PoolAble
{
    public float FireDamage { get; private set; } = 0;

    private Rigidbody2D rigid;

    private LayerMask targetLayer;
    private float attackRange = 0;
    private Vector3 startPos = Vector3.zero;
    private bool isFired = false;

    private Action<Collider2D> _onHit;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        _onHit = null;
    }

    private void FixedUpdate()
    {
        if (!isFired)
            return;

        if (OutOfRange())
        {
            isFired = false;
            ReleaseObject();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFired)
            return;

        if (targetLayer.value == (targetLayer.value | (1 << collision.gameObject.layer)))
        {
            if (collision.TryGetComponent(out IDamagable obj))
            {
                if (!obj.IsDeath())
                {
                    ObjectPoolManager.Instance.GetGo(PoolType.HitEffect).transform.position = collision.transform.position;
                    obj.TakeDamage(FireDamage);
                    isFired = false;
                    _onHit?.Invoke(collision);
                    ReleaseObject();
                }
            }
        }
    }

    public void Initialize(Vector2 dir, float rotZ, float range, float damage, float speed, LayerMask layer)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        startPos = transform.position;
        attackRange = range;

        rigid.velocity = dir * speed;

        FireDamage = damage;

        isFired = true;

        targetLayer = layer;
    }

    public void Initialize(Vector2 dir, float rotZ, float range, float damage, float speed, LayerMask layer,
        Action<Collider2D> onHit)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        startPos = transform.position;
        attackRange = range;

        rigid.velocity = dir * speed;

        FireDamage = damage;

        isFired = true;

        targetLayer = layer;
        _onHit = onHit;
    }

    private bool OutOfRange()
    {
        return (transform.position - startPos).magnitude > attackRange;
    }
}