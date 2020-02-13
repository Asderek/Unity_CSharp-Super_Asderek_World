using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class YokaiFireBall : SentryScript
{
    private float verticalSpeed;
    private bool alreadyJump = false;

    protected override string CurrentUpdate()
    {
        if (!isOnGround)
        {
            verticalSpeed = rigidBody.velocity.y;
            if (verticalSpeed > 0)
                animator.SetInteger("risingSpeed", 1);
            else if (verticalSpeed < 0)
                animator.SetInteger("risingSpeed", -1);
            else
                animator.SetInteger("risingSpeed", 0);
        }
        else
        {
            verticalSpeed = 0;
            animator.SetInteger("risingSpeed", 0);
        }
        
        string activateState = null;
        activateState = base.CurrentUpdate();
        switch (currentState.ToLower())
        { 
            case "rising":
                break;
            case "falling":
                break;
        }

        return activateState;
    }

    protected override string Running()
    {
        float direction;

        if (isOnRange(viewRange)) //se o player está na distancia de chasing
        {
            direction = player.transform.position.x - transform.position.x;

            if (direction == 0)
                return "activateRunning";

            direction = direction / Mathf.Abs(direction);
            if (isOnGround)
                transform.eulerAngles = new Vector2(0, (direction - 1) * (-90));

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.90f)
                AchieveMaxSpeed(jumpForce);

            return "activateRunning";
        }

        float CurrentDistance = transform.position.x - initialPosition;
        direction = (((int)transform.eulerAngles.y) / -90) + 1; /*+1 right -1 left*/

        string nextState = "activateRunning";
        if (direction * CurrentDistance > (scoutRange + AUXRANGE))
        {
            transform.eulerAngles = new Vector2(0, (direction + 1) * 90);
        }
        else if (direction * CurrentDistance > (scoutRange))
        {
            nextState = "activateSitting";
        }
        
        if ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.90f) && (nextState != "activateSitting"))
            AchieveMaxSpeed(jumpForce);

        return nextState;
    }

    protected override void AchieveMaxSpeed(float jumpForce)
    {
        float direction = (((int)transform.eulerAngles.y) / -90) + 1;
        accelerating = true;

       
        if( (isOnGround) && (!alreadyJump))
        {
            alreadyJump = true;
            rigidBody.AddForce(new Vector2(direction, 1.75f) * jumpForce);
        }
    }

    protected override void ControlVelocity() { 
    }

    protected override void ChangeStateEvent(string currentState, string nextState) {
        base.ChangeStateEvent(currentState, nextState);
        alreadyJump = false;
    }

}
