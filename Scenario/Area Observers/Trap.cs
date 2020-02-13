using UnityEngine;
using System.Collections;

public abstract class Trap : AreaEffector {

    private bool trapActivated = false;
    private Animator animator;

    protected abstract void activeTrap();
    protected abstract void deactiveTrap();

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }

    protected override void LateUpdate()
    {
        if (!trapActivated)
            return;

        base.LateUpdate(); 
    }

    protected override void newVisitor(GameObject visitor)
    {
        if (visitor.GetComponent<Asderek>() != null) {
            trapActivated = true;
            animator.SetBool("active", true);
            activeTrap();
        }
    }

    protected override void exitVisitor(GameObject visitor)
    {
        if (visitor.GetComponent<Asderek>() != null)
        {
            trapActivated = false;
            animator.SetBool("active", false);
            deactiveTrap();
        }
    }

    protected override void onScreenEnter()
    { }
    protected override void onScreenExit()
    { }
}
