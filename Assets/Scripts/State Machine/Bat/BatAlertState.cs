using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatAlertState : NPCBaseState
{
    public BatAlertState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory) { }

    public override void EnterState()
    {
        Debug.Log("Entered alert state!");
    }

    public override void UpdateState()
    {

        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {

    }

    public override void InitializeSubState() { }
}
