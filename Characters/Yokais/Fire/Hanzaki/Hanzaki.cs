using UnityEngine;
using System.Collections;

public class Hanzaki : Guardian {

    public float maxSpeed;
    public float OriginMargin;
    private Vector3 destination;

    protected override void Start()
    {
        base.Start();
        sprite = GetComponentsInChildren<Transform>()[1].gameObject;
    }

    protected override string CurrentUpdate()
    {
        //print((("currentState = " + currentState.ToLower());
        switch (currentState.ToLower())
        {
            
            case "fleeing":
                return Fleeing();

            case "climbing":
                return Climbing();
                
            case "running":
                return Running();

            case "falling":
                if (sprite.transform.eulerAngles.z > 0.1)
                {
                    sprite.transform.Rotate(0, 0, -sprite.transform.eulerAngles.z / 40);
                }
                break;
        }

        return base.CurrentUpdate();
    }

    private string Running()
    {
        AchieveMaxSpeed(maxSpeed, false);

        if (Wake() && (transform.position.y > player.transform.position.y + 3))
        {
            animator.SetTrigger("triggerAwaking");
            rigidBody.velocity = Vector2.zero;

            return null;
        }


        if (Sleep())
        {
            return "activateSleeping";
        }

        return null;
    }

    private string Climbing()
    {
        return null;
    }

    private string Fleeing()
    {
        FightOrFlight(false);
        AchieveMaxSpeed(maxSpeed,true);
        if (!isOnRange(viewRange * 3f) || isOnLedge)
        {
            return "activateClimbing";
        }
        return null;
    }

    protected void AchieveMaxSpeed(float MaxSpeed, bool horizontal)
    {
        if (horizontal)
        {
            base.AchieveMaxSpeed(MaxSpeed);
        }
        else
        {
            Vector2 delta;
            delta = destination - transform.position;

            sprite.transform.Rotate(0, 0, Vector2.Angle(Vector2.right, delta) - sprite.transform.eulerAngles.z);

            delta.y *= 7f;
            delta.Normalize();
            rigidBody.velocity = delta * maxSpeed;

        }
    }

    protected override void ChangeStateEvent(string currentState, string nextState)
    {

        if ((nextState == "falling") || (currentState == "climbing"))
        {
            rigidBody.isKinematic = !rigidBody.isKinematic;
        }

        if (nextState == "running")
        {
            destination = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(5, 7), transform.position.z);

            if (transform.position.x > player.transform.position.x)
                FightOrFlight(false);
            else
                FightOrFlight();


            sprite.transform.Rotate(0, 0, 90);
        }

        if (nextState == "gettingup")
        {
            sprite.transform.Rotate(0, 0, -sprite.transform.eulerAngles.z);
        }

        

        base.ChangeStateEvent(currentState, nextState);
    }

    protected override bool Sleep()
    {
        return ((transform.position - destination).magnitude < OriginMargin);
    }

    protected override bool GroundVerification()
    {
        if (rigidBody.isKinematic == true)
            return true;
        return base.GroundVerification();
    }

    protected override bool CanDealDamage() {
        return true;
    }

    protected override bool isOnRange(Vector2 range)
    {
        if (currentState.ToLower() == "sleeping")
        {
            if (transform.position.y < player.transform.position.y)
                return false;
        }
            
        
        return base.isOnRange(range);
    }
}
