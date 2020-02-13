using UnityEngine;
using Assets.Scripts;

public class KiyoHime : Boss {

    private float initialPosition;
    private float scoutRange=10;
    private float AUXRANGE = 2;
    private float maxSpeed=3.8f;
    public float jumpForce;
    private bool alreadyDone=false;
    public float maxSpeedModifier;
    public float poisonTime = 5f;

    public Vector3 asd;
    public Vector3 biteMargin;
    
	protected override void Start () {
        base.Start();
        if(cameraLimits!=null)
            cameraLimits.SetActive(false);
        initialPosition = transform.position.x;
	}
	
    protected override string CurrentUpdate()
    {

        switch (currentState.ToLower())
        { 
            case "running":
                return Running();
            case "rising":
                return Rising();
            case "jumping":
                return Jumping();      
            case "chilling":
                return Chilling();
            case "poisoning":
                return Poisoning();
            case "summoning":
                return Summoning();
            case "bitting":
                return Bitting();
            case "throwing":
                return Throwing();
        }
            
        return base.CurrentUpdate();
	}

    private string Throwing()
    {
        float time =animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if ( time< 0.6f)
        {
            Vector3 aux = biteMargin;
            aux.x *= getDirection();
            player.transform.position = transform.position + aux;
        }
        else if (!alreadyDone)
        {
            alreadyDone = true;
           
            //print((("Force " + Utilities.standardVector(getDirection(), 30) * attacks[attackType].damageForce);
            
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Vector3 aux = biteMargin;
            aux.x *= getDirection();
            player.transform.position = transform.position + aux;
            //print(((player.GetComponent<Rigidbody2D>().velocity);
            player.GetComponent<Rigidbody2D>().AddForce(Utilities.standardVector(getDirection(), 30) * attacks[attackType].damageForce);
            //print(((player.GetComponent<Rigidbody2D>().velocity);
        }

        if (time > 0.8f)
        {
            player.GetComponent<Asderek>().ReceiveModifier(Commandments.Modifiers.Paralysis, poisonTime);
        }

        return null;
    }

    private string Bitting()
    {
        float time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if ( time > 0.5f)
        {
            Vector3 aux = biteMargin;
            aux.x *= getDirection();
            player.transform.position = transform.position + aux;

            if (!alreadyDone && time > 0.8f) {
                alreadyDone = true;
                OnCollisionWithPlayer();
            }

        }
        else {
            int n = 2; Vector3 aux = biteMargin;
            aux.x *= (n - (n - 1) * time / 0.5f) * getDirection();
            player.transform.position = Vector3.Lerp(player.transform.position, transform.position + aux, time * 2f);
            alreadyDone = false;
        }
        return null;
    }

    private string Summoning()
    {
        if (alreadyDone == false)
        {
            alreadyDone = true;
            Vector3 pos = transform.position;
            pos.x += asd.x * getDirection();
            pos.y += asd.y;
            pos.z += -1;
            ((GameObject)Instantiate(attacks[attackType].obj, pos, transform.rotation)).
                   GetComponent<Snakes>().setParameter(
                                                                                                   attacks[attackType].attackElement,
                                                                                                   attacks[attackType].damage,
                                                                                                   attacks[attackType].damageForce,
                                                                                                   attacks[attackType].criticalChance,
                                                                                                   getDirection(),
                                                                                                   gameObject);
        }
        return null;
    }

    private string Rising()
    {
        AchieveMaxSpeed(maxSpeed * maxSpeedModifier);
        return null;
    }

    private string Poisoning()
    {
        if (!alreadyDone)
        {
            alreadyDone = true;
            Vector3 spawnPosition = transform.position;
            if (getDirection() < 1)
                spawnPosition.x -= 1.4f;
            spawnPosition.y += myBounds.bounds.size.y * 0.9f;
            Instantiate(attacks[1].obj, spawnPosition, Quaternion.identity);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f) {
            attackType = 0;
            if (attackTypeParameter != null)
                animator.SetInteger(attackTypeParameter.name, attackType);
            attacks[attackType].lastAttack = Time.time;
            lastAttack = Time.time;
            lastSort = -sortCD;
            return "activateAttacking"; 
        }

        return null;
    }

    private string Chilling()
    {

        if (isOnRange(viewRange))
            return "activateRunning";

        return null;
    }

    private string Jumping()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f && alreadyDone==false)
        {
            alreadyDone = true;
            
            rigidBody.AddForce(Utilities.standardVector(getDirection(), 30f) * jumpForce);
        }
        AchieveMaxSpeed(maxSpeed * (1+ (maxSpeedModifier-1)/2));
        return null;
    }

    private string Running()
    {
        float direction;
        string nextState;

        float CurrentDistance = transform.position.x - initialPosition;
        direction = (((int)transform.eulerAngles.y) / -90) + 1; /*+1 right -1 left*/

        nextState = "activateRunning";


        string ret = DecideAttack();
        if (ret != null)
            return ret;


        if (direction * CurrentDistance > (scoutRange + AUXRANGE))
        {
            transform.eulerAngles = new Vector2(0, (direction + 1) * 90);
        }
        else if (direction * CurrentDistance > (scoutRange))
        {
            transform.eulerAngles = new Vector2(0, (direction + 1) * 90);
            if (direction > 0)
                nextState = "activateSitting";
            else
                nextState = "activateChilling";
        }

        AchieveMaxSpeed(maxSpeed);

        return nextState;


    }

    protected override void ChangeStateEvent(string currentState, string nextState)
    {
        alreadyDone = false;

        if (currentState == "chilling")
        {
            cameraLimits.SetActive(true);
        }

        if (nextState == "dying")
        {
            cameraLimits.SetActive(true);
        }

        base.ChangeStateEvent(currentState, nextState);
    }

    protected override string DecideAttack()
    {
        //testa o tail 
        bool isBehind = ((int)(player.transform.position.x - transform.position.x) / getDirection()) < 0;
        if (isOnRange(attacks[3].range) && (Time.time - attacks[3].lastAttack > attacks[3].CD) && (Time.time - lastAttack < globalCD) && isBehind)
        {
            attackType = 3;
            if (attackTypeParameter != null)
                animator.SetInteger(attackTypeParameter.name, attackType);
            attacks[attackType].lastAttack = Time.time;
            lastAttack = Time.time;
            lastSort = -sortCD;
            return "activateAttacking"; 
        }
        else
            return base.DecideAttack();
    }

    protected override void OnCollisionWithPlayer()
    {
        if (currentState == "rising" || currentState == "jumping")
        {
            if (!player.GetComponent<Asderek>().isInvulnerable())
            {
                player.GetComponent<Asderek>().ReceiveModifier(Commandments.Modifiers.Paralysis);
                animator.SetTrigger("triggerBitting");
            }
        } else 
            base.OnCollisionWithPlayer();
    }
    
    protected override void DealDamageOnPlayer(float damage)
    {
        base.DealDamageOnPlayer(damage);
    }

}

