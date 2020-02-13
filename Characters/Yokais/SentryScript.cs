using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts;


public class SentryScript : Enemy
{

    
    protected float initialSitting;
    protected float initialPosition;

    [Header("----------------------------------------------------------------------")]
    public float timeSitting = 2;
    public float cooldown = 2;
    public float maxSpeed = 2;
    public float scoutRange = 3;
    public float jumpForce = 350;


    public float AUXRANGE = 1;

    protected string ao_turnSentry = "TurnSentry";


    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        initialPosition = transform.position.x;
    }

    protected override string CurrentUpdate()
    {

        switch (currentState.ToLower())
        {

            case "chilling":
                return Chilling();

            case "running":
                return Running();

            case "takingdamage.fallingdown":
            case "takingdamage.gettingup":
                return TakingDamage();

            case "sitting":
                return Sitting();

            case "attacking":
                return Attacking();
            case "jumping":
                return Jumping();
                /*return Ranged()*/

            default:
                break;

        }

        return base.CurrentUpdate();
    }

    protected virtual string Jumping()
    {
        return "activateChilling";
    }

    protected virtual string TakingDamage()
    {
        return "activateChilling";
    }

    protected virtual string Sitting()
    {
        if (isOnRange(viewRange))
        {
            return "activateRunning";
        }

        if (Time.time - initialSitting > timeSitting)
        {
            ////print((("Time to go" + Time.time + " - " + initialSitting);
            return "activateRunning";
        }
        return null;
    }

    protected virtual string Attacking()
    {
        if (!isOnGround)
        {
            return "activateJumping";
        }

        AchieveMaxSpeed(maxSpeed / 2);
        return "activateRunning";
    }

    protected virtual string  Running()
    {
        float direction;
        string nextState;
        if (isOnWall)
        {
            ApplyOnce.apply(ao_turnSentry, gameObject, () =>
            {
                TurnCharacter((Direction)(-getDirection()));
                    AchieveMaxSpeed(maxSpeed);
                    return true;
                });
            AchieveMaxSpeed(maxSpeed);
            return "activateRunning";
        }
        
        if (!isOnGround)
        {
            nextState = LeaveGround();
            if (nextState != null)
            {
                return nextState;
            }
        }
        if (isOnLedge)
        {
            nextState = OnLedge();
            if (nextState != null)
            {
                return nextState;
            }
        }
        ApplyOnce.remove(ao_turnSentry, gameObject);



        string ret = DecideAttack();
        if (ret != null)
        {
            return ret;
        }

        if (isOnRange(viewRange)) //se o player está na distancia de chasing
        {
            nextState = BeginChase();
            if (nextState != null)
            {
                return nextState;
            }
        }

        float CurrentDistance = transform.position.x - initialPosition;
        direction = (((int)transform.eulerAngles.y) / -90) + 1; /*+1 right -1 left*/

        nextState = "activateRunning";

        

        if (direction * CurrentDistance > (scoutRange+AUXRANGE))
        {
            transform.eulerAngles = new Vector2(0, (direction + 1) * 90);
        }
        else if (direction * CurrentDistance > (scoutRange))
        {
            transform.eulerAngles = new Vector2(0, (direction + 1) * 90);
            if (direction > 0)
            {
                nextState = "activateSitting";
            }
            else
                nextState = "activateChilling";
        }

        AchieveMaxSpeed(maxSpeed);
        
        return nextState;


    }

    protected virtual string BeginChase()
    {
        float direction;
        direction = player.transform.position.x - transform.position.x;

        if (direction == 0)
            return "activateRunning";

        direction = direction / Mathf.Abs(direction);
        transform.eulerAngles = new Vector2(0, (direction - 1) * (-90));
        AchieveMaxSpeed(maxSpeed);

        return "activateRunning";
    }

    protected virtual string CanAttack()
    {
        return "activateAttacking";
    }

    protected virtual string OnLedge()
    {
        return "activateJumping";
    }

    protected virtual string LeaveGround()
    {
        return "activateJumping";
    }

    protected virtual string Chilling()
    {
        string ret = DecideAttack();
        if (ret != null)
            return ret;

        return "activateRunning";
    }

    protected override void ChangeStateEvent(string currentState, string nextState) 
    {
        base.ChangeStateEvent(currentState,nextState);

        ////print((("ChangeStateEvent");

        if (nextState.ToLower() == "sitting"){
            ////print((("Time to sit" + Time.time + " - " + initialSitting);
            initialSitting = Time.time;
        }

        if ((nextState.ToLower() == "running") && (currentState.ToLower()== "sitting" ) )
        {
            transform.eulerAngles = new Vector2 (0, 180 - transform.eulerAngles.y);
        }

        if ((nextState.ToLower() == "jumping"))
        {
            float direction = (((int)transform.eulerAngles.y) / -90) + 1; /*+1 right -1 left*/
            rigidBody.AddForce(Utilities.standardVector(direction) * jumpForce);
        }

    }


}
