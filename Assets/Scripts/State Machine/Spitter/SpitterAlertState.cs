using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterAlertState : NPCBaseState
{
    public SpitterAlertState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
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
