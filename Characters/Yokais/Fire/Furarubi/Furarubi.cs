using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Furarubi : SentryScript {

    public Vector2 explosionRange;
    private Vector3 initialPositionVect;
    
    protected override void Start()
    {
        base.Start();
        initialPositionVect = transform.position;
    }

    protected override void ChangeStateEvent(string currentState, string nextState)
    {
        animator.ResetTrigger(takingDamageParameter.name);
        if (nextState == "charging")
        {
            maxSpeed *= 1.1f;
            Physics2D.IgnoreCollision(player.GetComponentInChildren<BoxCollider2D>(), GetComponentInChildren<BoxCollider2D>(),true);
        }
        
        if (currentState == "charging")
        {
            maxSpeed /= 1.1f;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            Physics2D.IgnoreCollision(player.GetComponentInChildren<BoxCollider2D>(), GetComponentInChildren<BoxCollider2D>(), false);
        }

        if (nextState == "dying")
        {
            rigidBody.isKinematic = false;
        }

    }

    protected override string CurrentUpdate()
    {
        string nextState;        
        nextState = base.CurrentUpdate();
        if (nextState == null)
            switch (currentState)
            {
                case "flying":
                    return Running();

                case "charging":
                    return Charging();

                case "exploding":
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75f)
                    {
                        Dying();
                    }
                    return null;


                default:
                    break;
            }
        return nextState;
    }

    protected string Charging()
    {
        AchieveMaxSpeed(maxSpeed);

        if(isOnRange(explosionRange))
        {
            return "activateExploding";
        }

        if (!isOnRange(viewRange))
        {
            return "activateRunning";
        }

        return null;
    }

    protected override string OnLedge()
    {
        return null;
    }


    protected override void ControlVelocity()
    { 
        if (currentState != "charging")
            base.ControlVelocity();
    }

    protected override void AchieveMaxSpeed(float MaxSpeed)
    {
        if (currentState == "charging")
        {
            accelerating = true;
            Vector3 goalDirection;


            //goalDirection = new Vector3(player.transform.position.x - transform.position.x, (player.transform.position.y + playerScript.mySize.y) - transform.position.y, 0);
            goalDirection = playerScript.getPlayerPosition() - (Vector2)transform.position;
            goalDirection.x += getDirection() * AUXRANGE;

            displayHP.offset = goalDirection;


            goalDirection.Normalize();
            goalDirection = Mathf.Abs(maxSpeed / goalDirection.x) * goalDirection;
            
            goalDirection.y = Mathf.Sign(goalDirection.y) * Mathf.Min (Mathf.Abs(goalDirection.y),maxSpeed);
            rigidBody.velocity = goalDirection;

            if (rigidBody.velocity.x != 0)
            {
                float direction = rigidBody.velocity.x / Mathf.Abs(rigidBody.velocity.x);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, (direction - 1) * -90, transform.eulerAngles.z);
            }

        }
        else
            base.AchieveMaxSpeed(MaxSpeed);
    }

    protected override void OnCollisionEnter2D(Collision2D coll)
    {
        //print(("Hit: " + coll.collider.gameObject);
        if (Utilities.HitAsderek(coll))
            return;

        //print(("Not Asderek");
        base.OnCollisionEnter2D(coll);
    }

    protected override string CanAttack()
    {
        return "activateCharging";
    }

    protected override void CleanUp()
    {
        //Instantiate(gameObject, initialPosition, Quaternion.identity);
        base.CleanUp();
    }

    protected override string LeaveGround()
    {
        return null;
    }

}
