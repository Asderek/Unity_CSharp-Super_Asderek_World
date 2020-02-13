using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class BouncingRock : SimpleFaller {
    
    public int BouncingTimes;
    public float critChance;
    public float direction = -1;
    public float maxAngle, minAngle;
    public float maxForce, minForce, jumpForce;
    
    Rigidbody2D rigidBody;

    protected override void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        base.Start();
    }

    protected override void HitFloor(GameObject floor)
    {
            BouncingTimes--;

            if (BouncingTimes <= 0)
            { 
                Destroy(gameObject);       
            }
            else if (rigidBody.velocity.x != 0)
            {
                rigidBody.AddForce(Utilities.standardVector(rigidBody.velocity.x, 75) * (jumpForce * BouncingTimes*0.30f + jumpForce));
                rigidBody.velocity = Vector2.zero;
            }
            else
            {
                rigidBody.AddForce(Vector2.up * (jumpForce * BouncingTimes));
                rigidBody.velocity = Vector2.zero;
            }
    }

    protected override void HitPlayer(GameObject target)
    {
        target.GetComponent<Character>().ReceiveDamage(
                                                                                               Utilities.standardVector(target.transform.position.x - transform.position.x),
                                                                                               repelForce,
                                                                                               damage, Random.Range(0, 100) < critChance,
                                                                                               myElement);

        HitFloor(target);
    }

    protected override void HitEnemy(GameObject target)
    {
        target.GetComponent<Character>().ReceiveDamage(
                                                                                               Utilities.standardVector(target.transform.position.x - transform.position.x),
                                                                                               repelForce,
                                                                                               damage, Random.Range(0, 100) < critChance,
                                                                                               myElement);

        HitFloor(target);
    }

    protected virtual void Activate()
    {
        transform.GetComponent<Rigidbody2D>().isKinematic = false;
        if (direction == 0)
        {
            GetComponent<Rigidbody2D>().AddForce(-Utilities.standardVector((Mathf.CeilToInt(((Random.value) * 2)) * 2 - 3), Random.Range(minAngle, maxAngle)) * Random.Range(minForce, maxForce));
        }
        else
        {
            GetComponent<Rigidbody2D>().AddForce(-Utilities.standardVector(-direction, Random.Range(minAngle, maxAngle)) * Random.Range(minForce, maxForce));
        }
        //ActivateTick(waitTime);
    }
	
}
