using UnityEngine;
using System.Collections;
using System;

public class Guardian : Enemy
{

    public float attackCD;

    protected override void Start()
    {
        base.Start();
        lastAttack = -attackCD;

    }

    protected override string CurrentUpdate()
    {


        switch (currentState.ToLower())
        {
            case "sleeping":
                return Sleeping();

            case "attacking":
                return Attacking();

            case "chilling":
                return Chilling();

        }

        return base.CurrentUpdate();
    }

    protected virtual string Sleeping()
    {
        if (Wake())
        {
            animator.SetTrigger("triggerAwaking");
        }
        return null;
    }

    protected virtual bool Wake()
    {
        return isOnRange(viewRange);
    }

    protected virtual bool Sleep()
    {
        return !isOnRange(1.5f * viewRange);
    }

    protected virtual string Attacking()
    {
        return "activateChilling";
    }

    protected virtual string Chilling()
    {
        if (Sleep())
        {
            return "activateSleeping";
        }
        else
        {

            FightOrFlight();
            string ret = DecideAttack();
            if (ret != null)
                return ret;
        }
        return null;
    }

}
