using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class BreakableFaller: SimpleFaller{

    public float direction=-1;
    public float maxAngle, minAngle;
    public float maxForce, minForce;
    public float critChance;

    protected override void Start()
    {
        base.Start();
        
        //Atira bolas em direções x randomicas, com angulo entre minDegree e maxDegree
        if (direction == 0)
        {
            GetComponent<Rigidbody2D>().AddForce(-Utilities.standardVector((Mathf.CeilToInt(((Random.value) * 2)) * 2 - 3), Random.Range(minAngle, maxAngle)) * Random.Range(minForce, maxForce));
        }
        //Atira bolas na direção especificada, idem
        else
        {
            GetComponent<Rigidbody2D>().AddForce(-Utilities.standardVector(-direction, Random.Range(minAngle, maxAngle)) * Random.Range(minForce, maxForce));
        }
    }

    protected override void Update()
    {
        base.Update();

		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("dying")) 
		{
			
			GetComponent<Rigidbody2D> ().freezeRotation = true;
			transform.Rotate(0, 0, -transform.eulerAngles.z);
		}

    }

    protected override void OnTriggerEnter2D(Collider2D colisor)
    {
        //base.OnTriggerEnter2D(colisor);
    }

    protected virtual void OnCollisionEnter2D(Collision2D coll)        //colliding
    {
        base.OnTriggerEnter2D(coll.collider);
    }
}
