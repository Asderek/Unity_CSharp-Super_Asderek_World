using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System.Collections.Generic;

public class Kasha : GuardianPursuer {
    
    public float jumpForce;

    protected override void Start()
    {
        base.Start();
        /*foreach (Attack at in attacks)
        {
            if (at.ranged == false)
            {
                if (maxMeleeRange < at.range)
                    maxMeleeRange = at.range;
            }
        }*/
    }

    protected override void ChangeStateEvent(string currentState, string nextState)
    {
        if ((nextState.ToLower() == "prepareattacking"))
        {
            float direction = (((int)transform.eulerAngles.y) / -90) + 1;

            rigidBody.AddForce(Utilities.standardVector(direction,30) * jumpForce);
        }


        base.ChangeStateEvent(currentState, nextState);
    }


    protected override void ControlVelocity()
    {
        if ((currentState.ToLower() == "rising") || (currentState.ToLower() == "falling") || (currentState.ToLower() == "prepareattacking"))
            return;
        base.ControlVelocity();
    }
}
