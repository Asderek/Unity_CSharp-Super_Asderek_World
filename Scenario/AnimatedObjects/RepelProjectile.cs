using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class RepelProjectile : SimpleCollider {

    public float lifeTime = 4;

    protected float initialTime;
    protected bool repel = false;


    protected virtual void Start()
    {
        initialTime = Time.time;
    }

    protected virtual void FixedUpdate()
    {
        /*< troquei para ca tava no start com destroy ( gameObecjt, lifetime); >*/
        if (initialTime + lifeTime < Time.time) {
            Destroy(gameObject);
        }

        if (transform.position.y < 0)
        {
            //print((("Devia ter ajeitado essa porra!");
            //Destroy(gameObject);
        }
    }

    protected override void HitCharacterAttack()
    {
            transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            transform.GetComponent<Rigidbody2D>().angularVelocity = 0;

            Vector2 direction = new Vector2((owner.transform.position.x - transform.position.x),(owner.transform.position.y - transform.position.y) );
            direction.Normalize();

            transform.GetComponent<Rigidbody2D>().AddForce(direction*400);
            transform.GetComponent<Rigidbody2D>().AddTorque(30);

            repel = true;

    }

    protected override void HitOwner()
    {
        if (repel == true)
        {
            owner.GetComponent<Character>().ReceiveDamage(Utilities.standardVector(owner.transform.position.x - transform.position.x), repelForce, damage);
            Destroy(gameObject);
        }
    }

    protected override void HitCharacter(GameObject target)
    {
        if (!repel)
            base.HitCharacter(target);
    }

	// Update is called once per frame
	
}
