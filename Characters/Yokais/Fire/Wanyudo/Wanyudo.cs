using UnityEngine;

public class Wanyudo : SentryScript
{

    public float chargeDistance;
    protected GameObject lavaInstance;
    public GameObject lavaSpew;

    public float lavaCD;
    private float lastLava;

    private bool giveDamage = false;
    private float chillingEnter;
    private float chillingTime = 2f;
    private float lastEnrage;
    private float coolingOffTime = 10;

    protected override void Start()
    {
        base.Start();

    }

    protected override void ChangeStateEvent(string currentState, string nextState)
    {
        animator.ResetTrigger(takingDamageParameter.name);

        if (currentState == "fleeing")
        {
            if (lavaInstance != null)
            {
                lavaInstance.transform.parent = null;
                lavaInstance = null;
            }
        }

        if (nextState == "charging")
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            maxSpeed *= 1.6f;
            attackDamage *= 1.2f;
            lastEnrage = Time.time;
            FightOrFlight(true);

        }

        if (currentState == "enrage")
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            maxSpeed /= 1.6f;
            attackDamage /= 1.2f;
        }

        

        if (nextState == "chilling")
        {
            chillingEnter = Time.time;
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }


        base.ChangeStateEvent(currentState, nextState);
    }

    protected override string CurrentUpdate()
    {

        string nextState;
        nextState = base.CurrentUpdate();
        if (nextState == null)
            switch (currentState)
            {
                case "enrage":
                    return Enrage();

                case "charging":
                    return Charging();

                case "fleeing":
                    return Fleeing();


                default:
                    break;
            }
        return nextState;
    }

    protected string Fleeing()
    {
        if (isOnLedge || isOnWall)
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);

            float asd = Random.Range(0, 2);
            //print("asd = " + asd);
            if (asd == 0)
                return "activateRunning";
            else
                return "activateCharging";
        }

        float CurrentDistance = (-player.transform.position.x + transform.position.x);
        if ((Mathf.Abs(CurrentDistance) > chargeDistance) && (lavaInstance == null))
        {
            return "activateCharging";
        }

        CurrentDistance /= Mathf.Abs(CurrentDistance);
        transform.eulerAngles = new Vector2(0, (CurrentDistance - 1) * (-90));

        AchieveMaxSpeed(maxSpeed);

        ////print(((rigidBody.velocity.y);
        if (Time.time - lastLava > lavaCD && isOnGround && rigidBody.velocity.y < 0.01f)
        {
            lavaInstance = (GameObject)Instantiate(lavaSpew, transform.position, Quaternion.identity);
            lastLava = Time.time;
            if (lavaInstance != null)
            {
                lavaInstance.transform.eulerAngles = transform.eulerAngles;
                lavaInstance.transform.parent = transform;

            }
        }
        else if (Time.time - lastLava > lavaCD * 0.8f)
        {
            if (lavaInstance != null)
            {
                lavaInstance.transform.parent = null;
                lavaInstance = null;
            }
        }

        return null;

    }

    protected string Charging()
    {
        return null;
    }

    protected override string OnLedge()
    {

        if (isOnRange(viewRange)) //se o player está na distancia de chasing
        {
            //print("OnRange<br>");
            return null;
        }
        //print("NotOnRange");
        ApplyOnce.apply(ao_turnSentry, gameObject, () =>
        {
            //print((("Apply fucking Once " + gameObject)));
            TurnCharacter((Direction)(-getDirection()));
            AchieveMaxSpeed(maxSpeed);
            return true;
        });
        return "activateRunning";
    }

    protected virtual string Enrage()
    {
        if (giveDamage || (Time.time - lastEnrage > coolingOffTime && !isOnRange(viewRange)))
        {
            lastEnrage = Mathf.Infinity;
            giveDamage = false;
            return "activateChilling";
        }
        base.Running();
        return null;
    }

    protected override string Running()
    {
        if (giveDamage)
        {
            giveDamage = false;
            return "activateChilling";
        }
        return base.Running();
    }

    protected override void AchieveMaxSpeed(float MaxSpeed)
    {
        base.AchieveMaxSpeed(MaxSpeed);
    }

    protected override string CanAttack()
    {
        return null;
    }

    protected override Vector2 CalculateDirection(GameObject obj)
    {
        giveDamage = true;
        return base.CalculateDirection(obj);
    }

    protected override string Chilling()
    {
        if (Time.time - chillingEnter > chillingTime)
            return "activateRunning";

        return null;
    }

    protected override string DecideAttack()
    {
        attackType = 0;
        return null;
    }

    protected override string LeaveGround()
    {
        //print("leaveGround");
        return null;
    }

    protected override bool isOnRange(Vector2 range)
    {
        if (currentState == "enrage")
        {
            return base.isOnRange(range);
        }
        if (isFacing(player))
        {
            return base.isOnRange(range);
        }
        return base.isOnRange(new Vector2(range.x*0.25f,range.y));
    }
}
