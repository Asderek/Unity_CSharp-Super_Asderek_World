using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class ProjectilesDelay : RepelProjectile
{
    public GameObject target;
    private float threshold;
    private bool targetLocked = false;

    protected override void Start()
    {
        base.Start();
     
        threshold = Random.value*0.6f;
    }

    protected override void FixedUpdate()
    {

        base.FixedUpdate();

        float ratio = (Time.time - initialTime) / lifeTime;
        if (!repel)
        {
            if( (ratio > threshold) && (!targetLocked))
            {
                targetLocked = true;

                Vector2 direction = new Vector2((target.transform.position.x - transform.position.x), (target.transform.position.y - transform.position.y));
                direction.Normalize();

                
                Rigidbody2D rigidBody = transform.GetComponent<Rigidbody2D>();

                rigidBody.velocity = Vector2.zero;
                rigidBody.angularVelocity = 0;


                rigidBody.AddForce(direction * 200);
                rigidBody.angularVelocity = 400;
                rigidBody.gravityScale = 0;
                rigidBody.angularDrag = 0;


                //transform.Translate(direction*20);


            }
        }
    }


    

}
