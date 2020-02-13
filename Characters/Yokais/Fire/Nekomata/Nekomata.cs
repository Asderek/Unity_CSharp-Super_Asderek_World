using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Nekomata : SentryScript {

    
    
    private bool alreadyJump = false;
    private bool instanced = false;
    private bool alreadySummoned;
    private bool alreadyShot;
    [Header("----------------------------------------------------------------------")]
    public Vector3 summonVect;
    public Vector3 shootVect;
    public float summonRange;
    public float fleeingRange;

    protected override void Start()
    {
        base.Start();
        distanceToKeep--;
    }

    protected override string CurrentUpdate()
    {
        //print("CUR_State = " + currentState);
        switch (currentState.ToLower())
        {

            case "rolling":
                return Rolling();

            case "summoning":
                return Summoning();

            case "shooting":
                return Shooting();

            default:
                break;

        }

        return base.CurrentUpdate();
    }

    protected override string Chilling()
    {
        string ret = DecideAttack();
        if (ret != null)
            return ret;


        if (isOnRange(fleeingRange))
        {
            FightOrFlight(false);
            return "activateRunning";
        }

        if (isOnRange(viewRange))
            FightOrFlight(true);

        if ((Time.time - attacks[0].lastAttack > attacks[0].CD) && (Time.time - lastAttack > globalCD))
            return "activateRunning";
        else
            return null;
        
        

    }

    protected override string Running()
    {

        string nextState;
        if (isOnLedge)
        {
            nextState = OnLedge();
            if (nextState != null)
            {
                return nextState;
            }
        }

        if (isOnRange(viewRange))
        {
            if (isOnRange(fleeingRange))
            {
                if ((Time.time - attacks[1].lastAttack > attacks[1].CD) && (Time.time - lastAttack > globalCD))
                {
                    attackType = 1;
                    if (attackTypeParameter != null)
                        animator.SetInteger(attackTypeParameter.name, attackType);
                    attacks[attackType].lastAttack = Time.time;
                    lastAttack = Time.time;
                    FightOrFlight(true);
                    return "activateAttacking";
                }
                else
                {
                    FightOrFlight(false);
                    AchieveMaxSpeed(maxSpeed);
                    return "activateRunning";
                }
            }
            else if ((Time.time - attacks[0].lastAttack > attacks[0].CD) && (Time.time - lastAttack > globalCD))
            {
                attackType = 0;
                if (attackTypeParameter != null)
                    animator.SetInteger(attackTypeParameter.name, attackType);
                attacks[attackType].lastAttack = Time.time;
                lastAttack = Time.time;
                FightOrFlight(true);
                return "activateAttacking";
            }
            else
            {
                FightOrFlight(true);
                return "activateChilling";
            }
        }

        return base.Running();
    }

    private string Summoning()
    {
        
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f && !alreadySummoned)
        {
            if (attacks[attackType].obj != null)
            {
                alreadySummoned = true;
                Vector3 location = transform.position + new Vector3(getDirection() * summonVect.x,summonVect.y,0);
                GameObject hitodama = (GameObject) Instantiate(attacks[attackType].obj, location, Quaternion.identity);
                float radius = Random.Range(3.0f, 5.0f);
                float angle = Random.Range(0f, 90f);


                hitodama.GetComponent<Hitodama>().destinyPosition = (Vector2)transform.position + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * radius * getDirection(), Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
            }
        }
        return null;
    }

    private string Shooting()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f && !alreadyShot)
        {
            if (attacks[attackType].obj != null)
            {
                alreadyShot = true;
                Vector3 location = transform.position + new Vector3(getDirection() * shootVect.x, shootVect.y, 0);
                ((GameObject)Instantiate(attacks[attackType].obj, location, transform.rotation)).
                    GetComponent<Fireball>().setParameter(
                                                                                                    attacks[attackType].attackElement,
                                                                                                    attacks[attackType].damage,
                                                                                                    attacks[attackType].damageForce,
                                                                                                    attacks[attackType].criticalChance,
                                                                                                    getDirection(),
                                                                                                    gameObject);
            }
        }
        return null;
    }

    private string Rolling()
    {
        if (isOnGround) {
            AchieveMaxSpeed(maxSpeed);
        }
        return null;
    }

    protected override bool GroundVerification()
    {
        bool ground = base.GroundVerification();
        if (ground == true && instanced == false)
        {
            //Instantiate(bolaVerde, transform.position, Quaternion.identity);
            instanced = true;
        }

        if (ground == false)
        {
            instanced = false;
        }
        return ground;
    }

    protected override void ChangeStateEvent(string currentState, string nextState)
    {
        alreadyShot = alreadySummoned = false;
        base.ChangeStateEvent(currentState, nextState);
    }

    protected override string BeginChase()
    {
        float direction;
        direction = player.transform.position.x - transform.position.x;

        //if (isOnRange(distanceToKeep))
        //{
        //    FightOrFlight(false);
        //    direction *= -1;
        //}
        //else {
        //    FightOrFlight(true);
        //    return "activateChilling";
        //}


        if(isOnRange(distanceToKeep) && !isOnRange(distanceToKeep-2))
        {
            FightOrFlight(true);
            //print("ret_activateChilling");
            return "activateChilling";
        }
        else if (isOnRange(distanceToKeep))
        {
            //print("elseIF");
            FightOrFlight(false);
            direction *= -1;
        }


        if (direction == 0)
        {
            //print("ret_direction==0");
            return "activateRunning";
        }

        direction = direction / Mathf.Abs(direction);
        transform.eulerAngles = new Vector2(0, (direction - 1) * (-90));
        AchieveMaxSpeed(maxSpeed);

        //print("ret_activateRunning");
        return "activateRunning";
    }

    protected override void ControlVelocity()
    {
        if (!isOnGround)
            return;
        base.ControlVelocity();
    }
}

