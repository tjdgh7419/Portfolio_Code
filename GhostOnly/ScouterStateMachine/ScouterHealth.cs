using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Util;
using UI.SubItem;

public class ScouterHealth : MonoBehaviour, IDamagable
{
    public float MaxHealth { get; private set; }
    [field: SerializeField] public float CurrentHealth { get; private set; }

    public event Action<float> OnGetDamageEvent;
    public event Action OnDeathEvent;

    private void Start()
    {
        OnGetDamageEvent += ShowDamageText;
    }

    public void SetInitialized(float _maxHp)
    {
        MaxHealth = _maxHp;
        CurrentHealth = _maxHp;
    }

    public bool IsDeath() => CurrentHealth <= 0;

    public void TakeDamage(float damage)
    {
        if (IsDeath()) { return; }

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        OnGetDamageEvent?.Invoke(damage);

        if (IsDeath())
        {
            OnDeathEvent?.Invoke();

            Managers.Sound.PlaySound(Data.SoundType.Death, transform.position, true);
        }

        Managers.Sound.PlaySound(Data.SoundType.SkullHit, transform.position, true);
    }

    private void ShowDamageText(float damage)
    {
        UI_DamageText.ShowDamageText(damage, Constants.Colors.HitColor, transform.position);
    }
}
