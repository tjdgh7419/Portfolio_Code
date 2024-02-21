using UnityEngine;

public abstract class ScouterBaseState : IState<ScouterBaseState.EScouterState>
{
    public enum EScouterState
    {
        Idle,
        Death,
        Exit,
    }

    protected ScouterStateMachine stateMachine;

    public ScouterBaseState(ScouterStateMachine sm) { stateMachine = sm; }

    public abstract EScouterState Key();

    public virtual EScouterState NextState()
    {
        if      (stateMachine.IsDeath)  { return EScouterState.Death; }
        else if (stateMachine.IsExit)   { return EScouterState.Exit; }
        else                            { return EScouterState.Idle; }
    }

    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void OnTriggerEnter2D(Collider2D other) { }
    public virtual void OnTriggerExit2D(Collider2D other) { }
    public virtual void OnTriggerStay2D(Collider2D other) { }
}
