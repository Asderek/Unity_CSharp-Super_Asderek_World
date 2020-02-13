using UnityEngine;
using System.Collections;

public class GuardianPursuer : Guardian
{
    public float maxSpeed;
    public float OriginMargin;


    protected Vector3 initialPosition;
    protected Vector3 initialEuler;

    protected override void Start()
    {
        base.Start();
        initialEuler = transform.eulerAngles;
        initialPosition = transform.position;
    }

    protected override void ChangeStateEvent(string currentState, string nextState)
    {
        if (nextState.ToLower() == "sleeping")
        {
            transform.eulerAngles = initialEuler;
        }
        base.ChangeStateEvent(currentState, nextState);
    }
    protected override string CurrentUpdate()
    {
        switch (currentState.ToLower())
        {
            case "running":
                return Running();                

        }

        return base.CurrentUpdate();
    }


    protected virtual string Running()
    {
        string ret = DecideAttack();
        if (ret != null)
            return ret;

        if (isOnRange(distanceToKeep))
        {
            FightOrFlight(true);
            return "activateChilling";
        }

        if (isOnRange(viewRange))
        {
            FightOrFlight(true);
            AchieveMaxSpeed(maxSpeed);
        }
        else if (Sleep())
        {
            return "activateSleeping";
        }
        else
        {
            float direction;
            direction = initialPosition.x - transform.position.x;

            if (direction == 0)
                direction = 1;

            direction = direction / Mathf.Abs(direction);
            transform.eulerAngles = new Vector2(0, (direction - 1) * (-90));
            AchieveMaxSpeed(maxSpeed);
        }
        return null;
    }

    protected override string Chilling()
    {
        AchieveMaxSpeed(0);
        if (!isOnRange(distanceToKeep + 1) && isOnRange(viewRange))
        {
            return "activateRunning";
        }
        return base.Chilling();
    }

    protected override bool Sleep()
    {
        return Mathf.Abs(initialPosition.x - transform.position.x) < OriginMargin;
    }

    protected override string Sleeping()
    {
        AchieveMaxSpeed(0);
        return base.Sleeping();
    }

}


