using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Snakes : Projectile {

    public float breakChance;

    public float lifeTime;
    public float bindTime;
    private float startTime;
    private float bindStartTime;

    private GameObject player;
    private Collider2D coll;
    private Vector3 groundVerifier = new Vector3(0,0.1f, 0);

    protected override void Start()
    {
        bindStartTime = 0;
        startTime = Time.time;
        coll = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        if (coll != null)
        {
            if (Physics2D.Linecast(transform.position, transform.position - groundVerifier, 1 << LayerMask.NameToLayer("Floor")))
            {
                GetComponent<Rigidbody2D>().isKinematic = true;
                GetComponent<Rigidbody2D>().velocity = maxSpeed;
                coll.enabled = false;
                coll = null;
            }
        }
        if (bindStartTime != 0)
        {
            if (Time.time - bindStartTime > 1.1f)
            {
                if (gameObject)
                {
                    if (ButtonManager.GetUp(ButtonManager.ButtonID.CIRCLE,this))
                    {
                        if (Random.Range(0, 100) < breakChance)
                        {
                            bindStartTime = -bindTime;
                        }
                    }

                    if (Time.time - bindStartTime > bindTime)
                        Destroy();
                }
            }

            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Vector3 aux = player.transform.position;
            aux.x = transform.position.x;
            player.transform.position = aux;
        }
        else if (Time.time - startTime > lifeTime)
        {
            Destroy();
        }

    }

    protected override void HitCharacter(GameObject target)
    {
       
        if (target.GetComponent<Asderek>() != null)
        {
            if (target.GetComponent<Asderek>().ReceiveModifier(Commandments.Modifiers.Paralysis))
            {
                bindStartTime = Time.time;
                player = target;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponentInChildren<Animator>().SetBool("activateClimbing", true);


                Vector3 aux = transform.position;
                aux.z = player.transform.position.z - 1 ;
                transform.position = aux;
            }
        }
    }

    void Destroy()
    {
        if(player != null)
            player.GetComponent<Asderek>().RemoveModifier(Commandments.Modifiers.Paralysis);
        Destroy(gameObject);
    }

}
