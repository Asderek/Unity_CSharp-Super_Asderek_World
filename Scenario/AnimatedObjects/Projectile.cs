using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Projectile : SimpleCollider 
{

    public Vector2 maxSpeed;
    public bool teamAttack = false;

    protected virtual void Start()
    {
        GetComponent<Rigidbody2D>().velocity = maxSpeed;
    }

    protected override void HitCharacterAttack()
    {
        if (gameObject)
        {
            Destroy(gameObject);
        }
    }

    protected override void HitCharacter(GameObject target)
    {
        bool crit = Random.Range(0, 100f) < critChance;


        if (target.GetComponent<Character>().ReceiveDamage(Utilities.standardVector(colisor.transform.position.x - transform.position.x), repelForce, damage, crit, myElement) != 0)
        {
            if (gameObject)
            {
                Destroy(gameObject);
            }
        }
    }


    protected override void HitSomething()
    {
        if (teamAttack)
        {
            if (gameObject)
            {
                Destroy(gameObject);
            }
        }
    }

    public void setParameter(Commandments.Element newElement, float damage, float repelForce, float critChance, int direction, GameObject owner)
    {
        this.myElement = newElement;
        this.damage = damage;
        this.repelForce = repelForce;
        this.critChance = critChance;
        maxSpeed.x *= direction;
        this.owner = owner;
    }
}
