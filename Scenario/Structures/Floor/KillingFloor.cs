using UnityEngine;
using System.Collections;

public class KillingFloor : MonoBehaviour
{

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //print("Collision.gameObject = " + collision.gameObject);
        if (Assets.Scripts.Utilities.HitAsderek(collision))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Asderek>().SetDying();
        }
        else if (collision.gameObject.GetComponentInParent<Character>() != null)
        {
            Rigidbody2D characterRigidBody = collision.gameObject.GetComponentInParent<Rigidbody2D>();
            collision.gameObject.GetComponentInParent<Character>().SetDying();
            if (characterRigidBody != null)
            {
                characterRigidBody.isKinematic = true;
                characterRigidBody.velocity = Vector2.zero;
            }
        }

    }


    /*void OnTriggerEnter2D(Collider2D colisor)
    {
            if (Assets.Scripts.Utilities.HitAsderekAttack(colisor))
            {
                return;
            }
            else if (colisor.gameObject.GetComponentInParent<Character>() != null)
            {
                Rigidbody2D characterRigidBody = colisor.gameObject.GetComponentInParent<Rigidbody2D>();
                colisor.gameObject.GetComponentInParent<Character>().Dying();
                if (characterRigidBody != null)
                {
                    characterRigidBody.isKinematic = true;
                    characterRigidBody.velocity = Vector2.zero;
                }
            }

    }*/

}
