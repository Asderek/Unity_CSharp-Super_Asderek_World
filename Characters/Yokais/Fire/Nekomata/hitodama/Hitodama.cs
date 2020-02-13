using UnityEngine;
using System.Collections;

public class Hitodama : Enemy {

    public Vector2 destinyPosition;
    protected Vector2 nextPosition;
    public float changeDirection = 1;
    protected float lastChange;
    public float maxRange = 1;
    public float minimalDistance = 0.5f;
    public float maxSpeed = 2;
    protected float angle;
    public float stepAngle;
    public float maxVertSpeed;
    protected float initialAngle;

    public GameObject[] summons;
    private bool alreadySummoned = false;

    protected override void Start()
    {
        base.Start();
        sprite = GetComponentsInChildren<Transform>()[1].gameObject;
        lastChange = -changeDirection;
        initialAngle = sprite.transform.eulerAngles.z;
    }

    protected override string CurrentUpdate()
    {
        switch (currentState.ToLower())
        {
            case "running":
                return Running();

            case "disappearing":
                return Disappearing();
        }

        return base.CurrentUpdate();
    }

    protected virtual string Disappearing()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f && !alreadySummoned) {
            //print((("Hello");
            alreadySummoned = true;
            Instantiate(summons[Random.Range(0, summons.Length)], transform.position, Quaternion.identity);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
            Dying();
        }

        return null;
    }

    protected virtual string Running()
    {
        Vector2 aux = transform.position;

        if ((destinyPosition - aux).magnitude <= minimalDistance)
        {
            return "activateDisappearing";
        }

        if ((nextPosition - aux).magnitude <= minimalDistance)
        {
            lastChange = -changeDirection;
        }

        if (Time.time - lastChange > changeDirection) {
            //nextPosition = Vector3.Lerp(transform.position, destinyPosition, Random.Range(0.0f, 1.0f));
            nextPosition = destinyPosition;
            nextPosition = nextPosition + new Vector2(Random.Range(-maxRange, maxRange), Random.Range(-maxRange, maxRange));
            lastChange = Time.time;
        }

        AchieveMaxSpeed(maxSpeed);

        return null;    
    }

    protected override void AchieveMaxSpeed(float MaxSpeed)
    {
        Vector3 aux = nextPosition;
        aux.z = transform.position.z;
        Vector3 goalDirection = aux - transform.position;

        goalDirection.Normalize();

        rigidBody.velocity = maxSpeed * goalDirection + maxVertSpeed * (Quaternion.Euler(0, 0, 90) * goalDirection) * Mathf.Sin(angle);
        angle += stepAngle;

        float desiredAngle = Mathf.Rad2Deg* Mathf.Atan2(rigidBody.velocity.y, rigidBody.velocity.x);
        sprite.transform.Rotate(0, 0, -sprite.transform.eulerAngles.z + desiredAngle + initialAngle);
    }

    public override void Dying()
    {
        if (dead == false)
        {
            dead = true;
        }
    }
}
