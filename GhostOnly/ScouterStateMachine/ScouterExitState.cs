using UnityEngine;
using Util;

public class ScouterExitState : ScouterBaseState
{
    private float currentExitDuration = 0f;
    private const float DURATION = 5f;
    private bool _isAniTriggered = false;

    public ScouterExitState(ScouterStateMachine sm) : base(sm) { }

    public override EScouterState Key() => EScouterState.Exit;

    public override EScouterState NextState()
    {
        //if (stateMachine.IsExit) { return Key(); }
        //else { return base.NextState(); }

        if (stateMachine.IsExit)
        {
            if (stateMachine.IsDeath)
            {
                return EScouterState.Death;
            }
            else
            {
                return Key();
            }
        }
        else
        {
            return base.NextState();
        }
    }

    public override void Enter()
    {
        currentExitDuration = DURATION;
        _isAniTriggered = false;
    }

    public override void Exit()
    {
        stateMachine.Ani.SetBool(Constants.AniParams.Exit, false);
        stateMachine.Collider.enabled = true;
    }

    public override void FixedUpdate()
    {
        currentExitDuration -= Time.deltaTime;
        
        if (currentExitDuration < 0)
        {
            stateMachine.gameObject.transform.position = stateMachine.GetNewSpawnPosition();
            stateMachine.IsExit = false;
        }
        else if (currentExitDuration < 1 && !_isAniTriggered)
        {
            stateMachine.Ani.SetBool(Constants.AniParams.Exit, true);
            _isAniTriggered = true;
            stateMachine.Collider.enabled = false;
        }
    }
}
