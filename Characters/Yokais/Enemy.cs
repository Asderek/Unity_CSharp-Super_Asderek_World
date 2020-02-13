using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System.Collections.Generic;

public class Enemy : Character
{



    [Header("----------------------------------------------------------------------")]
    public GameObject player;
    public Asderek playerScript;
    public bool friendlyFire;
    protected bool dead= false;
    public bool isOnWall = false;

    private float ultIncrease = 0.2f;
    public Vector2 viewRange = new Vector2(3,3);
    public float repelForce;
    public float contactDamage; 
    
    public float globalCD;
    protected float lastAttack;

    protected int attackType = 0;
    protected Vector2 maxMeleeRange;
    public Attack[] attacks;
    public float sortCD = 5;
    protected float lastSort;

    public GameObject cameraLimits;

    [HideInInspector]
    public Vector3 editorStartPosition;

    [System.Serializable]
    public struct Attack
    {
        public float damage;
        public float damageForce;
        public Vector2 range;
        public float CD;
        public float probability;
        public float criticalChance;
        public bool ranged;
        public Commandments.Element attackElement;
        public GameObject obj;
        public float lastAttack;

    }

    public enum Identifiers
    {
        Invulnerability,
    }

    [System.Serializable]
    public struct Drop
    {
        public GameObject item;
        public float dropChance;
    }

    public Drop[] drops;
    public float scriptDropChance;
    public int yokaiRank = 0;
    public float distanceToKeep;


    protected override void Start()
    {
        base.Start();
        editorStartPosition = transform.position;
        attackType = 0;

        lastAttack = -globalCD;
        lastSort = -sortCD;
        for(int i=0;i<attacks.Length;i++)
        {
            
            attacks[i].lastAttack = -attacks[i].CD;

            if (attacks[i].ranged == false)
            {

                if (maxMeleeRange.x < attacks[i].range.x)
                    maxMeleeRange.x = attacks[i].range.x;

                if (maxMeleeRange.y < attacks[i].range.y)
                    maxMeleeRange.y = attacks[i].range.y;
            }
        }


        player = UIManager.GetInstance().GetPlayer().gameObject;
        playerScript = UIManager.GetInstance().GetPlayer();
        gameObject.tag = "Enemy";

        if (attacks.Length != 0)
        {
            distanceToKeep = attacks[0].range.x;
            foreach (Attack attack in attacks)
            {
                if (distanceToKeep > attack.range.x)
                    distanceToKeep = attack.range.x;
            }
        }

        rigidBody.gravityScale = 3;


    }
    
    protected override string CurrentUpdate()
    {
        switch (currentState.ToLower())
        {
            case "dying":
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.2f)
                {
                   CleanUp();
                }
                break;
        }

        return null;
    }

    protected virtual void CleanUp()
    {
        //Destroy(gameObject);
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D colisor) //attack
    {
        if (colisor.gameObject.tag == "PlayerSpriteTag" && colisor.gameObject != null)
        {
            Asderek enemy = colisor.gameObject.GetComponentInParent<Asderek>();
            foreach (Collider2D col in colliders)
            {
                if (col.isActiveAndEnabled)
                {
                    if (enemy.ColliderContains(col))
                    {
                        if(CanDealDamage())
                            OnCollisionWithPlayer();
                    }
                }
            }
        }
    }
    
    protected virtual void OnCollisionWithEnemy(GameObject obj)
    {
        if (friendlyFire == false)
        {
            if (obj.GetComponent<Enemy>().friendlyFire == false)
                Physics2D.IgnoreCollision(myBounds, obj.GetComponent<Enemy>().myBounds);
        }
    }
    
    protected virtual void OnCollisionWithPlayer() {
        
        float multiplier = 1;
        bool critical = false;

        float criticalValue = 100 * Random.value;

        if (criticalValue < (attacks[attackType].criticalChance))
        {
            critical = true;
            multiplier *= 2;
        }
        float damage = playerScript.ReceiveDamage(CalculateDirection(player), attacks[attackType].damageForce, multiplier * attacks[attackType].damage, critical, attacks[attackType].attackElement, this);
        if ( damage > 0)
        {
            DealDamageOnPlayer(damage);
        }
    }

    protected virtual void DealDamageOnPlayer(float damage)
    {
            
    }

    protected virtual Vector2 CalculateDirection (GameObject obj){
        
        float direction = obj.transform.position.x-transform.position.x;
        
        if (isOnGround) {
            return new Vector2(direction /Mathf.Abs(direction),1);
        }
        return Utilities.standardVector(obj.transform.position.x - transform.position.x);
    }

    protected override  void OnCollisionEnter2D(Collision2D coll)        //colliding
    {
        
        base.OnCollisionEnter2D(coll);
        if (coll.gameObject == player)
        {
            Vector2 direction = new Vector2(player.transform.position.x - transform.position.x, 0);
            direction.Normalize();

            if (CanDealDamage())
            {
                bool isAttacking = false;
                foreach (Collider2D col in colliders)
                    if (col.isActiveAndEnabled)
                    {
                        isAttacking = true;
                        break;
                    }
                if (isAttacking)
                    OnCollisionWithPlayer();
                else
                    player.GetComponent<Asderek>().ReceiveDamage(CalculateDirection(coll.gameObject), repelForce, contactDamage, false, Commandments.Element.NEUTRAL, this);
            }
        }

        if (coll.gameObject.tag == "Enemy")
        {
            OnCollisionWithEnemy(coll.gameObject);
        }

        if (coll.gameObject.tag == "Wall")
        {
            EdgeCollider2D collider = coll.otherCollider as EdgeCollider2D;
            if (collider != null)
            {
                float yTop = Mathf.Max(collider.points[0].y, collider.points[1].y);
                float yBottom = Mathf.Min(collider.points[0].y, collider.points[1].y);

                if (transform.position.y < yTop && transform.position.y > yBottom)
                {
                    isOnWall = true;
                }
            }

            
        }

        if (coll.gameObject.tag == "Floor" || (coll.gameObject.tag == "Ground"))
        {
            if (currentState == "dying")
            {
                foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
                {
                    col.enabled = false;
                }
                rigidBody.isKinematic = true;
            }
        }

    }

    protected virtual void OnCollisionStay2D(Collision2D coll)
    {
      
        if (coll.gameObject == player)
        {
            float posX, posY;
            posX = player.transform.position.x;
            posY = player.transform.position.y;

            if (Mathf.Abs(posX - transform.position.x) < 0.5)
            {
                if (posY > transform.position.y) {
                
                    float direction = (((int) player.transform.eulerAngles.y) / -90) + 1;

                    coll.rigidbody.AddForce(new Vector2(direction * attacks[0].damageForce, 0));
                    
                }
            }

            
        }

        if (coll.gameObject.tag == "Wall")
        {
            isOnWall = true;
        }
    }


    protected virtual void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Wall")
        {
            isOnWall = false;
        }
    }


    protected virtual bool isOnRange(float range) {
        return ( Mathf.Abs(player.transform.position.x - transform.position.x) < range);
    }

    protected virtual bool isOnRange(Vector2 range)
    {
        Vector2 distance = playerScript.getPlayerPosition() - (Vector2)transform.position;
        return (Mathf.Abs(distance.x) < range.x) && (Mathf.Abs(distance.y) < range.y);

    }

    protected void dropItems() {
        float dropValue = 100 * Random.value;
        ////print((("   DropValue: " + dropValue);
        //dropValue = rand.Next(100);

        float total = 0;
        foreach (Drop drop in drops)
        {
            total += drop.dropChance;
        }

        dropValue /= Commandments.Modifiers.IncreaseDropRate.GetValue();
        if ( dropValue > total)
            return;

        dropValue = total * (dropValue / total);


        float acc=0;

        foreach (Drop drop in drops)
        {
            acc += drop.dropChance;
            ////print((("Acc: " + acc );
            if (acc > dropValue)
            {
                
                Instantiate(drop.item, transform.position, Quaternion.identity);
                break;
            }

        }

    }

    public override void Dying()
    {
        if (dead == false)
        {
            dead = true;
            dropItems();
            if (dyingParameter != null)
            {
                animator.SetTrigger(dyingParameter.name);
            }
            drawManager.drawList.Clear();
            player.GetComponent<Asderek>().IncreaseUlt(ultIncrease);
        }

    }

    public override float ReceiveDamage(Vector2 direction, float force, float damage, bool critical = false, Commandments.Element attackerElement = Commandments.Element.NEUTRAL, Character source = null)
    {

        if (currentState == "dying")
            return 0;

        float receivedDamage = 0;
        bool ret = CoolDownManager.Apply(Identifiers.Invulnerability.ToString(),gameObject,invulnerabilityTime, () => 
        {


            damage *= damageModifier[attackerElement.toInt()];

            receivedDamage = base.ReceiveDamage(direction, force, damage, critical, attackerElement, source);

            if (hp <= 0)
            {
                if (takingDamageParameter != null)
                    animator.ResetTrigger(takingDamageParameter.name);
                if (critical) {
                    float criticalValue = 100 * Random.value;
                    


                    if (criticalValue > (100 - scriptDropChance*Commandments.Modifiers.IncreaseScriptDropRate.GetValue()))
                    {
                        GameManager.GetInstance().SpawnYokaiEssence(transform.position, yokaiRank, player);
                    }
                }
                Dying();
            }

            return true;
        });

        ////print("Received Damage = " + receivedDamage);
        return receivedDamage;
    }

    protected override void AchieveMaxSpeed(float MaxSpeed)
    {

        float direction = (((int)transform.eulerAngles.y) / -90) + 1;

        accelerating = true;
        float multiplier = 1;
        if (direction * rigidBody.velocity.x < 0)
            multiplier = 100;

        desiredVelocity = rigidBody.velocity.x + (direction * MaxSpeed * maxSpeedRatio - rigidBody.velocity.x) / (timeOfAccel * multiplier);

    }

    protected virtual bool CanDealDamage()
    {
        if (currentState == "dying")
            return false;
        return true;
    }

    protected virtual void FightOrFlight(bool fight = true)
    {
        float direction;
        direction = player.transform.position.x - transform.position.x;
        if (!fight)
        {
            direction *= -1;
        }

        if (direction == 0)
            direction = 1;

        direction = direction / Mathf.Abs(direction);
        transform.eulerAngles = new Vector2(0, (direction - 1) * (-90));
        
    }

    protected virtual int sortAttack()
    {
        if (Time.time - lastSort < sortCD)
        {
            return -1;
        }

        List<int> availableAttacks = new List<int>();
        float maxProb = 0;
        bool reach = isOnRange(maxMeleeRange);
        float randNumber;


        if (!reach)
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                if (attacks[i].ranged == true)
                {
                    if (Time.time - attacks[i].lastAttack > attacks[i].CD)
                    {
                        availableAttacks.Add(i);
                        maxProb += attacks[i].probability;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                if (Time.time - attacks[i].lastAttack > attacks[i].CD)
                {
                    availableAttacks.Add(i);
                    if (attacks[i].ranged == true)
                        maxProb += attacks[i].probability / 2.0f;
                    else
                        maxProb += attacks[i].probability;
                }
            }
        }

        randNumber = Random.Range(0, maxProb);
        foreach (int index in availableAttacks)
        {

            if (attacks[index].ranged == true && reach == true)
            {
                randNumber -= attacks[index].probability / 2;
            }
            else
                randNumber -= attacks[index].probability;

            if (randNumber < 0)
            {
                lastSort = Time.time;
                return index;
            }
        }



        return -1;
    }

    protected virtual string DecideAttack()
    {

        if (Time.time - lastAttack < globalCD)
        {
            //print("if (Time.time - lastAttack < globalCD)");
            return null;
        }

        int tempAttack = sortAttack();

        if (tempAttack >= 0)
            attackType = tempAttack;
        //print("attackType = " + attackType);


        if (Time.time - attacks[attackType].lastAttack < attacks[attackType].CD)
        {
            //print("if (Time.time - attacks[attackType].lastAttack < attacks[attackType].CD)");
            return null;
        }


        if (isOnRange(attacks[attackType].range))
        {
            if (attackTypeParameter != null)
                animator.SetInteger(attackTypeParameter.name, attackType);
            attacks[attackType].lastAttack = Time.time;
            lastAttack = Time.time;
            lastSort = -sortCD;

            FightOrFlight(true);

            //print("if (isOnRange(attacks[attackType].range))");
            return "activateAttacking";
        }
        //print("return null");
        return null;
    }


}