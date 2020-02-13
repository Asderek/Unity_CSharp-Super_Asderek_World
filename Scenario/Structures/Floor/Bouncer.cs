using UnityEngine;
using System.Collections;

public class Bouncer : MonoBehaviour {

    public float intensity = 250;

    protected virtual void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.GetComponent<FreeCamera>() != null)
            return;

        GameObject parent = colisor.gameObject.transform.parent.gameObject;

        if (parent.GetComponent<Character>() == null)
            return;

        parent.GetComponent<Rigidbody2D>().velocity = new Vector2(parent.GetComponent<Rigidbody2D>().velocity.x, 0);
        parent.GetComponent<Rigidbody2D>().AddForce(Vector2.up*intensity);
    }

}
