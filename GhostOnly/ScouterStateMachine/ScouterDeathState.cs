using UnityEngine;
using Util;

public class ScouterDeathState : ScouterBaseState
{
    private float currentDeathDuration = 0f;
    private const float DURATION = 1.5f;

    public ScouterDeathState(ScouterStateMachine sm) : base(sm) { }

    public override EScouterState Key() => EScouterState.Death;

    public override EScouterState NextState()
    {
        return Key();
    }

    public override void Enter()
    {
        stateMachine.Ani.SetBool(Constants.AniParams.Die, true);
        stateMachine.Collider.enabled = false;

        Managers.Soul.GetSoul(Constants.Scouter.DeathReward);

        currentDeathDuration = DURATION;
    }

    public override void FixedUpdate()
    {
        currentDeathDuration -= Time.deltaTime;

        if (currentDeathDuration < 0)
        {
            stateMachine.Ani.SetBool(Constants.AniParams.Die, false);
            
            stateMachine.gameObject.SetActive(false);
        }
    }
}
