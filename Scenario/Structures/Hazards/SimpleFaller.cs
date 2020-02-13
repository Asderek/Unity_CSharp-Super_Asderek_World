using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleFaller : MonoBehaviour {

    [System.Serializable]
    public struct InteractionConditions
    {
        public bool enemy;
        public bool player;
        public bool floor;
        public bool others;
    }

    public InteractionConditions dying, damaging;

    public float maxSelfDestroyDistance = 10;
    private Vector3 startingPosition;

    protected Transform player;
    public float repelForce = 100;
    public float damage = 20;
    public Commandments.Element myElement;

    protected Animator animator;


    protected virtual void Start()
    {
        player = UIManager.GetInstance().GetPlayer().transform;
        startingPosition = transform.position;
        animator = GetComponent<Animator>();
        GetComponent<Rigidbody2D>().isKinematic = false;
    }
    
   
    // Update is called once per frame
    protected virtual void Update()
    {
        if ((transform.position - startingPosition).magnitude > maxSelfDestroyDistance)
        {
            Destroy(gameObject);
        }

        if (animator)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("dying") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                Destroy(gameObject);
            }
        }
	}

    protected virtual void OnTriggerEnter2D(Collider2D colisor)
    {
        if (!enabled)
            return;

        Transform parent = colisor.gameObject.transform.parent;
        if (parent != null)
        {
            if (Utilities.HitAsderek(colisor))
            {
                HitPlayer(parent.gameObject);
                return;
            } else if (parent.gameObject.GetComponent<Enemy>() != null)
            {
                HitEnemy(parent.gameObject);
                return;
            }
        }

        if ((colisor.gameObject.tag == "Ground") || (colisor.gameObject.tag == "Floor"))
        {
            HitFloor(colisor.gameObject);
            return;
        }

        //print(((colisor.gameObject.tag);
        if (colisor.gameObject.tag == "IgnoreContact")
        {
            return;
        }
        
        HitOther(colisor.gameObject);

    }


    protected virtual void HitPlayer(GameObject target)
    {
        if (damaging.player)
        {
            Vector2 direction = Utilities.standardVector(target.transform.position.x - transform.position.x);
            target.GetComponent<Character>().ReceiveDamage(direction, repelForce, damage, false, myElement);
        }

        if (dying.player)
        {
            if (animator)
            {
                animator.SetTrigger("triggerDying");
            }
        }
    }
    protected virtual void HitEnemy(GameObject target)
    {
        if (damaging.enemy)
        {
            Vector2 direction = Utilities.standardVector(target.transform.position.x - transform.position.x);
            target.GetComponent<Character>().ReceiveDamage(direction, repelForce, damage, false, myElement);
        }

        if (dying.enemy)
        {
            if (animator)
            {
                animator.SetTrigger("triggerDying");
            }
        }
    }
    protected virtual void HitFloor(GameObject floor) 
    {
        if (dying.floor)
        {
            if (GetComponent<Rigidbody2D>())
            {
                GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                GetComponent<Rigidbody2D>().isKinematic = true;
            }

            if (animator)
            {
                animator.SetTrigger("triggerDying");
            }
        }
    }
    protected virtual void HitOther(GameObject other) 
    {
        if (dying.others)
        {
            if (GetComponent<Rigidbody2D>())
            {
                GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                GetComponent<Rigidbody2D>().isKinematic = true;
            }

            if (animator)
            {
                animator.SetTrigger("triggerDying");
            }
        }
    }


    
}
