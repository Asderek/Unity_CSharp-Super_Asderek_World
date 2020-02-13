using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class DeathSpecial : MonoBehaviour
{

    public float radius;

    public bool isBig;
    private bool exploded;
    public float multiplier = 1;

    public float maxRadius;
    private float initialRadius;

    private GameObject target;
    private CircleCollider2D myBounds;
    private Commandments.Element myElement = Commandments.Element.DEATH;
    protected Rigidbody2D rigidBody;

    public float criticalChance = 0.1f;
    public float damageForce = 300;
    public float attackDamage = 20;

    public float maxSpeed;
    public float maxVertSpeed;

    private float angle;
    public float stepAngle;

    private float initTime;
    public float lifeTime;
    public float triggerRange;


    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        myBounds = GetComponentInChildren<CircleCollider2D>();
        initialRadius = myBounds.radius;
        initTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time - initTime > lifeTime)
            Destroy(gameObject);
        

        if (target == null)
        {
            if (myBounds.radius >= maxRadius)
                return;

            myBounds.radius *= 1.5f;
            return;
        }

        if (exploded)
        {
            if (GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isBig = false;
                exploded = false;
                transform.localScale /= 3f;
                maxSpeed += 0.33f * maxSpeed;

                for (int i = 0; i < 5; i++)
                {

                    float dir = (((int)transform.eulerAngles.y) / 360.0f);
                    Vector3 rotation = Quaternion.Euler(0, 0,  360/5.0f*(i+dir))* new Vector3(1,0,0)*radius;


                    Instantiate(gameObject, transform.position+rotation, Quaternion.identity);
                }
                Destroy(gameObject);
                
            }
            return;
                
        }

        Vector2 goalDirection;

        goalDirection = target.transform.position - transform.position;
        float direction = goalDirection.x / Mathf.Abs(goalDirection.x);
        transform.eulerAngles = new Vector2(0, (direction - 1) * (-90));
        
        if (isBig)
        {
            if (goalDirection.magnitude < triggerRange)
            {
                GetComponentInChildren<Animator>().SetTrigger("triggerExplode");
                exploded = true;
                rigidBody.velocity = Vector2.zero;
                return;
            }
        }

        goalDirection.Normalize();

        
        rigidBody.velocity = maxSpeed * goalDirection +maxVertSpeed * new Vector2(-goalDirection.y,goalDirection.x) * Mathf.Sin(angle);
        angle += stepAngle;






    }

    void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.gameObject.transform.parent == null)
            return;


        if (colisor.gameObject.transform.parent.gameObject.tag == "Enemy")
        {
            if (target == null)
            {
                target = colisor.gameObject;
                myBounds.radius = initialRadius;
                return;
            }

            Enemy enemy = colisor.gameObject.GetComponentInParent<Enemy>();


            float criticalValue = 100 * UnityEngine.Random.value;

            float weakness = enemy.getDamageModifier(myElement);

            float addCritical = 0;
            if (weakness != 1)
            {
                addCritical = ((weakness - 1) / Mathf.Abs(weakness - 1)) * criticalChance * 0.5f;
            }

            bool critical = false;
            if (criticalValue < (criticalChance + addCritical))
            {
                critical = true;
                multiplier *= 2;
            }
            colisor.gameObject.GetComponentInParent<Enemy>().ReceiveDamage(Utilities.standardVector(colisor.transform.position.x - transform.position.x), damageForce, multiplier * attackDamage * weakness, critical);
            Destroy(gameObject);
        }
    }

}
