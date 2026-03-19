using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//State machine behaviour
public abstract class CombatStateMachine : MonoBehaviour
{
    protected CombatState cState;

    public void SetState(CombatState ste)
    {
        this.cState = ste;
        StartCoroutine(this.cState.Start());
    }

    public void StartAttack()
    {
        StartCoroutine(this.cState.Attack());
    }

    public CombatState GetCombatState()
    {
        return cState;
    }
}
