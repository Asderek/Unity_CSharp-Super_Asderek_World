using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Fire : MonoBehaviour {

    public float delay;
    public float damage;
    public Commandments.Element myElement;
    public float repelForce;
    public float critChance;
    private float startingTime;

    public float lifeTime;

	// Use this for initialization
	void Start () {
        startingTime = Time.time;
        delay = Random.Range(0, delay);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Random.Range(-0.01f, 0.01f));

	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - startingTime > delay && delay > 0)
        {
            delay = -1;
            
            GetComponentInChildren<Animator>().SetTrigger("triggerActivate");
        }

        if (Time.time - startingTime > lifeTime)
        {
            Destroy(gameObject);
        }

	}

    protected virtual void OnTriggerEnter2D(Collider2D colisor)
    {

        Transform parent = colisor.gameObject.transform.parent;
        if (parent != null)
        {
            if (parent.gameObject.GetComponent<Character>() != null)
            {
                HitCharacter(parent.gameObject);
            }
        }

        if ((colisor.gameObject.tag == "Ground") || (colisor.gameObject.tag == "Floor"))
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }

    }

    private void HitCharacter(GameObject target)
    {
             target.GetComponent<Character>().ReceiveDamage(
                                                                                               Utilities.standardVector(target.transform.position.x - transform.position.x),
                                                                                               repelForce,
                                                                                               damage, Random.Range(0, 100) < critChance,
                                                                                               myElement);

    }

    
}
