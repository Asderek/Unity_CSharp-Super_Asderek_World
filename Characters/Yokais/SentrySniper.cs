using UnityEngine;
using System.Collections;


public class SentrySniper : SentryScript
{
    public GameObject attack;

    public Vector2 atkVelocity = new Vector2(0, 3);
    public float atkLifeTime = 2;

    protected override string Attacking()    //done
    {
        return "activateRunning";
    }

    protected override void ChangeStateEvent(string currentState, string nextState) 
    {
        if (nextState.ToLower() == "sitting")
        {
            initialSitting = Time.time;
            rigidBody.velocity = Vector2.zero;
        }

        if ((nextState.ToLower() == "running") && (currentState.ToLower() == "sitting"))
        {
            transform.eulerAngles = new Vector2(0, 180 - transform.eulerAngles.y);
        }

        if (nextState.ToLower() == ("attacking.firing"))
        {
            lastAttack = Time.time;

            GameObject Copy = (GameObject)Instantiate(attack, transform.position, Quaternion.identity);
            //print((("Instanciate "+ Copy);
            Copy.AddComponent<Projectile>();
            Copy.GetComponent<Projectile>().owner = this.gameObject;
            //Copy.GetComponent<Projectile>().lifeTime = atkLifeTime;
            //Copy.GetComponent<Projectile>().velocity = atkVelocity;
        }

    }

    protected override string Running()
    {
        float direction;

        string ret = DecideAttack();
        if (ret != null)
        {
            rigidBody.velocity = Vector2.zero;
            return ret;
        }

        if (isOnRange(viewRange)) //se o player está na distancia de chasing
        {

            direction = player.transform.position.x - transform.position.x;
            if (Mathf.Abs(direction) < 0.5f)
            {
                return "activateRunning";
            }

            if (direction == 0)
                return "activateRunning";

            direction = direction / Mathf.Abs(direction);
            transform.eulerAngles = new Vector2(0, (direction - 1) * (-90));
            AchieveMaxSpeed(maxSpeed);

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

        AchieveMaxSpeed(maxSpeed);

        return nextState;


    }



    
}
