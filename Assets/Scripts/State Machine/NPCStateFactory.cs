using UnityEngine;

public abstract class NPCStateFactory
{
    protected NPCStateMachine _context;

    public NPCStateFactory(NPCStateMachine currentContext)
    {
        _context = currentContext;
    }

    public abstract NPCBaseState Jump();
    public abstract NPCBaseState Grounded();
    public abstract NPCBaseState Idle();
    public abstract NPCBaseState Alert();
    public abstract NPCBaseState Combat();


}
