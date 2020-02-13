using UnityEngine;
using System.Collections.Generic;

public class Sogenbi : Enemy {

    private float verticalSpeed=0;
    public float velocityMargin;
    public float attackCD;
    public float maxSpeed;
    public float maxVertSpeed;
    private Vector3 initialPosition;
    public float initMargin;


    public Vector2 minAtkDistance;
    private float angle;
    public float stepAngle;
    private List<EnemyGroupHandler> handlers;

    protected override void Start()
    {
        base.Start();
        drawManager.drawList.Remove(displayHP);
        initialPosition = transform.position;
        if (handlers == null)
            handlers = new List<EnemyGroupHandler>();
    }

    protected override string CurrentUpdate()
    {
      
        verticalSpeed = rigidBody.velocity.y;
        if ((verticalSpeed < velocityMargin) && (verticalSpeed > -velocityMargin))
        {
            animator.SetInteger("verticalSpeed", 0);
        }
        else if (verticalSpeed > velocityMargin)
        {
            animator.SetInteger("verticalSpeed", 1);
        }
        else
        {
            animator.SetInteger("verticalSpeed", -1);
        }


        switch (currentState.ToLower())
        {

            case "sleeping":
                if (isOnRange(viewRange) && handlers.Count == 0) {
                    animator.SetTrigger("triggerAwaking");
                }
                return null;

            case "gliding":
                return Idle();

            case "rising":
            case "falling":
                return Chasing();

            case "attacking":
                return Attacking();

            case "takingdamage":
                return TakingDamage();

            case "preparingAttack":
                return null;

            default:
                break;

        }

        return base.CurrentUpdate();
    }
    
    protected virtual string Idle()
    {
        rigidBody.velocity = new Vector2(0, 1f);
        return null;
    }

    protected virtual string Chasing()
    {
        float direction;

        if ((isOnRange(attacks[attackType].range)) && (Time.time - lastAttack > attackCD) && (transform.position.y > (playerScript.getPlayerPosition().y+ attacks[attackType].range.y/2)))  //se o player esta na distancia de attacking
        {
            direction = playerScript.getPlayerPosition().x - transform.position.x;
            direction = direction / Mathf.Abs(direction);
            transform.eulerAngles = new Vector2(0, (direction - 1) * (-90));
            return "activateAttacking";
        }

        if (isOnRange(viewRange))
        {
            direction = player.transform.position.x - transform.position.x;

            if ((direction >= 0.5f) || (direction <= -0.5f))
            {
                direction = direction / Mathf.Abs(direction);
                transform.eulerAngles = new Vector2(0, (direction - 1) * (-90));
            }
            AchieveMaxSpeed(maxSpeed);

            return null;
        }
        else
        {
            AchieveMaxSpeed(maxSpeed, true);

            direction = initialPosition.x - transform.position.x;

            if ((direction >= 0.5f) || (direction <= -0.5f))
            {
                direction = direction / Mathf.Abs(direction);
                transform.eulerAngles = new Vector2(0, (direction - 1) * (-90));
            }

            if ((transform.position - initialPosition).magnitude < initMargin)
            {
                NotifyHandlers(EnemyGroupHandler.Notification.SLEEP);
                return "activateSleeping";
            }
            return null;
        }

    }

    protected virtual string Attacking()
    {
        rigidBody.velocity = Vector2.zero;
        return null;
    }

    protected virtual string TakingDamage()
    {
        Vector2 direction = (transform.position - player.transform.position);
        direction.y /= 2;
        direction.Normalize();
        rigidBody.velocity = (direction * 5f);
        return null;
    }
    
    protected override void ChangeStateEvent(string currentState, string nextState)
    {
        if (nextState.ToLower().Contains("attack"))
        {
            lastAttack = Time.time;
            rigidBody.velocity = Vector2.zero;
        }
        if (nextState.ToLower() == "shuttingdown")
        {
            rigidBody.velocity = Vector2.zero;
        }
        if (currentState.ToLower() == "shuttingdown")
        {
            drawManager.drawList.Remove(displayHP);
        }
        if (nextState.ToLower() == "gettingready")
        {
            drawManager.drawList.Add(displayHP);
        }

    }

    protected virtual void AchieveMaxSpeed(float MaxSpeed, bool home=false) {
        accelerating = true;
        Vector3 goalDirection;

        if (home == false)
        {
            goalDirection = playerScript.getPlayerPosition() + new Vector2(-getDirection() * minAtkDistance.x - transform.position.x, minAtkDistance.y - transform.position.y);

        }
        else
        {
            goalDirection =  initialPosition- transform.position;

            
        }

        if (goalDirection.magnitude <= initMargin)
        {
            rigidBody.velocity = Vector2.zero;
            return;
        }

        goalDirection.Normalize();
        //print("GoalDirection = " + goalDirection);
        //print("Result = " + (maxSpeed * goalDirection + maxVertSpeed * (Quaternion.Euler(0, 0, 90) * goalDirection) * Mathf.Sin(angle)));

        
        rigidBody.velocity = maxSpeed * goalDirection + maxVertSpeed * (Quaternion.Euler(0, 0, 90) * goalDirection) * Mathf.Sin(angle);
        //print("rigidBody.velocity =" + rigidBody.velocity);
        angle += stepAngle;

    }

    public void WakeUp()
    {
        animator.SetTrigger("triggerAwaking");
    }

    public void NotifyHandlers(EnemyGroupHandler.Notification note)
    {
        foreach (EnemyGroupHandler h in handlers)
        {
            h.ReceiveNotification(note,this);
        }
    }

    public void AddHandler(EnemyGroupHandler handler)
    {
        if (handlers == null)
            handlers = new List<EnemyGroupHandler>();
        handlers.Add(handler);
    }

    public override void Dying()
    {
        // NotifyHandlers(EnemyGroupHandler.Notification.DYING);
        rigidBody.velocity = Vector2.zero;
        rigidBody.isKinematic = false;
        myBounds.isTrigger = false;
        base.Dying();
    }

    protected override void CleanUp()
    {
        NotifyHandlers(EnemyGroupHandler.Notification.DYING);        
        base.CleanUp();
    }

    protected override string DecideAttack()
    {
        attackType = 0;
        return null;
    }

    protected override void ControlVelocity()
    {
        return;
    }
}
