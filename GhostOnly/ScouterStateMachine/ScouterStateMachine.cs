using System;
using UnityEngine;
using Util;
using DG.Tweening;

public class ScouterStateMachine : StateMachine<ScouterBaseState.EScouterState>
{
    [field: SerializeField] public Animator Ani { get; private set; }
    [field: SerializeField] public ScouterHealth Health { get; private set; }

    [field: SerializeField] public BoxCollider2D Collider { get; private set; }

    public bool IsExit { get; set; } = false;
    public bool IsDeath { get; set; } = false;

    private readonly Vector3 DEFAULT_LOCATE = Vector3.zero;

    private void Start()
    {
        foreach (ScouterBaseState.EScouterState state in Enum.GetValues(typeof(ScouterBaseState.EScouterState)))
        {
            switch (state)
            {
                case ScouterBaseState.EScouterState.Idle:
                    _states.Add(state, new ScouterIdleState(this));
                    break;
                case ScouterBaseState.EScouterState.Death:
                    _states.Add(state, new ScouterDeathState(this));
                    break;
                case ScouterBaseState.EScouterState.Exit:
                    _states.Add(state, new ScouterExitState(this));
                    break;
            }
        }

        CurrentState = _states[ScouterBaseState.EScouterState.Idle];

        Health.OnGetDamageEvent += GetDamage;
        Health.OnDeathEvent += SetOnDeath;
        DayManager.Instance.OnChangedDayStatus += SetOnNight;

        SetOnNight(true);
    }

    public void Initialize(float hp)
    {
        if (_states.ContainsKey(ScouterBaseState.EScouterState.Idle))
        {
            CurrentState = _states[ScouterBaseState.EScouterState.Idle];
        }

        Health.SetInitialized(hp);
        Ani.SetBool(Constants.AniParams.Die, false);
        Ani.SetBool(Constants.AniParams.Exit, false);
        Collider.enabled = true;
        IsDeath = false;
        IsExit = false;

        gameObject.transform.position = GetNewSpawnPosition();
    }

    private void GetDamage(float _)
    {
        Managers.Soul.GetSoul(Constants.Scouter.HitReward);

        Ani.SetTrigger(Constants.AniParams.Hit);
        ObjectPoolManager.Instance.GetGo(PoolType.GoblinSoul).transform.position = transform.position;

        IsExit = true;
    }

    private void SetOnDeath()
    {
        IsDeath = true;
    }

    private void SetOnNight(bool isNight)
    {
        if (isNight)
        {
            Initialize(Constants.Scouter.InitialHp + DayManager.Instance.CurrentDay * Constants.Scouter.HpCoeff);
            gameObject.SetActive(true);
        }
        else
        {
            Collider.enabled = false;
            Ani.SetBool(Constants.AniParams.Exit, true);
            Invoke(nameof(Vanish), 1.5f);
        }
    }

    private void Vanish()
    {
        gameObject.SetActive(false);
    }

    public Vector3 GetNewSpawnPosition()
    {
        //float angle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;

        //float x = _defaultLocate.x + SPAWN_RANGE * Mathf.Cos(angle);
        //float y = _defaultLocate.y + SPAWN_RANGE * Mathf.Sin(angle);

        //return new Vector3(x, y, _defaultLocate.z);
        return (Vector2)DEFAULT_LOCATE + UnityEngine.Random.insideUnitCircle * Constants.Scouter.SpawnRange;
    }
}
