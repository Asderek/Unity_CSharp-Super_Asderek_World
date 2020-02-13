using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System;

public class LavaSpew : MonoBehaviour {

    public float attackDamage;
    public float damageForce;

    private float lifeTime;
    public float lifeSpan;
    private float floorRotation;

    void Start()
    {

        //print("Jesus = " + transform.eulerAngles.z);
        lifeTime = Time.time;
    }

    void Update()
    {
        //print("localEulerAngles = " + transform.localEulerAngles.y);
        RotateTo(floorRotation);

        if (Time.time - lifeTime > lifeSpan)
        {
            Destroy(gameObject);
        }
    }

    private void RotateTo(float targetRotation)
    {
        int factor;

        if (transform.eulerAngles.y > 0)
        {
            targetRotation = 360 - targetRotation;
        }

        float angle1 = targetRotation;
        float angle2 = targetRotation + 180;
        while (angle2 > 360)
            angle2 -= 360;

        float diff1 = angle1 - transform.localEulerAngles.z;

        float diff2 = angle2 - transform.localEulerAngles.z;

        if (diff1 > 180)
            diff1 -= 360;
        else if (diff1 < -180)
            diff1 += 360;

        if (diff2 > 180)
            diff2 -= 360;
        else if (diff2 < -180)
            diff2 += 360;


        float dest;

        if (Mathf.Abs(diff1) > Mathf.Abs(diff2))
            dest = diff2;
        else
            dest = diff1;

        if (Mathf.Abs(dest) < 2)
            return;

        
        factor = (int) Mathf.Sign(dest);
        transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, 0, factor * 1.5f);
    }

    protected virtual void OnTriggerEnter2D(Collider2D colisor) //attack
    {

        if (colisor.GetComponent<Floor>() != null && colisor.gameObject != null)
        {
            ////print(("Floor");
            if (GetComponent<Rigidbody2D>() != null)
            {
                floorRotation = (int)colisor.GetComponent<Floor>().GetRotation(transform.position.x);
                
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().isKinematic = true;
            }
        }


        if (Utilities.HitAsderek(colisor))
        {

            Asderek enemy = colisor.gameObject.GetComponentInParent<Asderek>();

            float weakness = enemy.getDamageModifier(Commandments.Element.FIRE);

            enemy.ReceiveDamage(Utilities.standardVector(colisor.transform.position.x - transform.position.x), damageForce, attackDamage);
        }

    }

    protected virtual void OnTriggerStay2D(Collider2D colisor)
    {
        if (colisor.GetComponent<Floor>() != null && colisor.gameObject != null)
        {
            ////print(("Floor");
            if (GetComponent<Rigidbody2D>() != null)
            {
                floorRotation = (int)colisor.GetComponent<Floor>().GetRotation(transform.position.x);

            }
        }
    }
}
