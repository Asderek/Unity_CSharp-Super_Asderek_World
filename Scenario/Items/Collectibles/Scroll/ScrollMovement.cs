using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMovement : Hitodama {
    
    [HideInInspector]
    public int rank = -1;

    public GameObject target;
    public float lerp = 0;

    protected override string Disappearing()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
        {
            Dying();
        }
        lerp += 0.01f;
        transform.position = Vector3.Lerp(transform.position,target.transform.position + new Vector3(0, 1f, 0),lerp);

        return null;
    }

    protected override string Running()
    {
        destinyPosition = target.transform.position + new Vector3(0,1f,0);
        return base.Running();
    }

    protected override void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.gameObject.tag == "PlayerSpriteTag")
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, colisor.transform.position.z - 1);
            UIManager.GetInstance().ObtainNewYokai(rank);
        }
        //base.OnTriggerEnter2D(colisor);
    }
}
