using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class Geiser : EventObserver
{

    public GameObject[] route;
    protected List<Transform> steps;
    private int cont;
    public int waitTime;


    protected bool direction = true;
    protected int currentStep = 0;
    private float count = 0;
    protected Vector3 lastPosition;

    public float velocity = 0.1f;

    // Use this for initialization
    protected virtual void Start()
    {
        base.Start();
        cont = Random.Range(0, waitTime) * 30;
        //cont = waitTime * 30;

        currentRoute = 0;
        NewMove();

        transform.position = steps[0].position;
        lastPosition = transform.position;
    }

    protected virtual void NewMove()
    {
        steps = new List<Transform>(route[currentRoute].GetComponentsInChildren<Transform>());
        steps.Remove(route[currentRoute].transform);
        currentStep = 0;
        direction = true;
    }

    protected virtual void FixedUpdate()
    {
        cont++;
        if (cont < waitTime * 30)
            return;

        Vector3 newPos = transform.position + (steps[currentStep].position - lastPosition).normalized * velocity;
        newPos.z = transform.position.z;
        transform.position = newPos;

        if ((Mathf.Abs(steps[currentStep].position.x - transform.position.x) < velocity) && Mathf.Abs(steps[currentStep].position.y - transform.position.y) < velocity)
        {
            HandlePoint(steps[currentStep]);
            if (direction)
            {
                currentStep++;
                if (currentStep >= steps.Count)
                {
                    HandleEdge();
                }
            }
            else
            {
                currentStep--;
                if (currentStep < 0)
                {
                    HandleEdge();
                }
            }
            lastPosition = transform.position;

        }
    }

    protected virtual void HandlePoint(Transform t)
    {
    }

    protected virtual void HandleEdge()
    {
        cont = 0;
        direction = !direction;
        if (currentStep <= 0)
        {
            currentStep = 0;
        }
        else
        {
            currentStep = steps.Count - 1;
        }
    }

    void OnTriggerEnter2D(Collider2D colisor)
    {
        GameObject obj = colisor.gameObject;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
        }
        obj.transform.parent = transform;
    }

    void OnTriggerExit2D(Collider2D colisor)
    {

        GameObject obj = colisor.gameObject;
        while (obj.transform.parent != transform && (obj.transform.parent != null))
        {
            obj = obj.transform.parent.gameObject;
        }
        obj.transform.parent = null;

    }

    public override void SendNotification(bool status, Switch source)
    {
        base.SendNotification(status, source);
        currentRoute++;
        if (currentRoute >= route.Length)
            currentRoute = 0;

        NewMove();

    }


    public int currentRoute { get; set; }
}
