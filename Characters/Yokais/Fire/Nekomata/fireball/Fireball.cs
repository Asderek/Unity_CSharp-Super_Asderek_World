using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class Fireball : Projectile {

    private List<string> animatorStates;
    protected Animator animator;



	protected override void Start () {
        animator = GetComponentInChildren<Animator>();
	}

    protected virtual void Update()
    {
        if (GetComponent<Rigidbody2D>().velocity == Vector2.zero)
        {
            if (isStill())
                GetComponent<Rigidbody2D>().velocity = maxSpeed;
        }

        if (GameManager.GetInstance().getCurrentCamera().isOnCamera(transform) == false)
            Destroy(gameObject);

    }


    bool isStill()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("running"))
        {
            return true;
        }
        return false;
    }

 
}
