using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScouterIdleState : ScouterBaseState
{
    public ScouterIdleState(ScouterStateMachine sm) : base(sm) { }

    public override EScouterState Key() => EScouterState.Idle;

    public override EScouterState NextState()
    {
        return base.NextState();
    }
}
