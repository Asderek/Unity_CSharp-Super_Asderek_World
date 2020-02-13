using UnityEngine;
using System.Collections;

public class Item : Interactable
{
    [HideInInspector]
    public GameObject user=null;
    public Texture2D texture=null;
    public virtual bool useItem() { return false; }
    
    public Vector2 dropForce;
    public float rotation;

    protected virtual void Awake() {

        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();

        if (rigidBody == null)
            return;
        rigidBody.AddForce(dropForce);

        rigidBody.AddTorque(rotation);
    }

    protected virtual void EnemyGetItem(Enemy enemy) {
        if (enemy.GetItem(this))
        {
            Destroy(gameObject);
        }
    }

    protected override void OnTriggerStay2D(Collider2D colisor)
    {
        base.OnTriggerStay2D(colisor);
        if (colisor.gameObject.GetComponentInParent<Enemy>() != null)
        {
            EnemyGetItem(colisor.gameObject.GetComponentInParent<Enemy>());
        }
    }


    protected virtual void OnCollisionStay2D(Collision2D colidido)
    {
        if (gameObject.GetComponent<Rigidbody2D>() != null)
        if ((colidido.collider.GetComponent<Floor>() != null) && gameObject.GetComponent<Rigidbody2D>().velocity.x == 0 )
        {
            if (gameObject.GetComponentInChildren<BoxCollider2D>() != null) {
                gameObject.GetComponentInChildren<BoxCollider2D>().enabled = false;
            }
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        }

    }

    public override void ActivateInteraction()
    {
        if (manager.AddItem(this) == true)
        {
            manager.StopDisplayOnScreen(gameObject);
            Destroy(gameObject);
        }
    }

}
