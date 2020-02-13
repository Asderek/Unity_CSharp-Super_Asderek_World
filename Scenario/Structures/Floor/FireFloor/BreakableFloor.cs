using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableFloor : EventObserver {
   
    public GameObject disables;
    public GameObject enables;
    
    private bool canBreak;
    private List<GameObject> floors;
    public float throwTime = 1;
    private float lastThrow;

    public Vector2 dropForce;
    public float rotation;
    public float showContent = 1;



    protected override void Start()
    {
        base.Start();
        canBreak = false;
        disables.SetActive(false);

        floors = new List<GameObject>();

        foreach (Transform t in enables.GetComponentInChildren<Transform>())
        {
            if (t.parent = enables.transform)
                floors.Add(t.gameObject);
        }

        lastThrow = -throwTime;
    }

    protected virtual void Update()
    {
        if (canBreak)
        {
            if (Time.time - lastThrow > throwTime)
            {
                lastThrow = Time.time;
                
                if (floors.Count == 0)
                {                    
                    GameManager.GetInstance().getCurrentCamera().ClearChanges();
                    disables.SetActive(true);
                    enables.SetActive(false);
                    Destroy(this);
                    return;
                }

                int currentThrow = Random.Range(0,floors.Count-1);

                floors[currentThrow].AddComponent<Rigidbody2D>();

                floors[currentThrow].AddComponent<SimpleFaller>();
                floors[currentThrow].GetComponent<SimpleFaller>();

                floors[currentThrow].GetComponent<Rigidbody2D>().AddForce(Vector2.Scale(new Vector2(Random.Range(-1f, 1f), Random.Range(0, 1f)), dropForce));
                floors[currentThrow].GetComponent<Rigidbody2D>().AddTorque(Random.Range(0, rotation));

                floors.Remove(floors[currentThrow]);

                if (floors.Count == 0)
                {
                    lastThrow = Time.time + showContent - throwTime;
                }

            }
        }
    }

    public override void SendNotification(bool status, Switch source)
    {
        if (status == true)
        {
            canBreak = true;
            GameManager.GetInstance().getCurrentCamera().SetTempTarget(this);
        }
    }


}
