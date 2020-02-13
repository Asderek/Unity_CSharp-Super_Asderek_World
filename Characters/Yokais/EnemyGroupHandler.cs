using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class EnemyGroupHandler : MonoBehaviour {

    private List<Sogenbi> myActiveMinions;
    private List<Sogenbi> myInactiveMinions;
    private GameObject player;
    public float viewDistance;
    public float activeMinions=0;
    public float maxActiveMinions=1;

    public RisingDoor door;

    public enum Notification
    { 
        SLEEP,
        DYING

    }

	void Start () {
        myActiveMinions = new List<Sogenbi>();
        myInactiveMinions = new List<Sogenbi>();
        myInactiveMinions.AddRange(GetComponentsInChildren<Sogenbi>());

        foreach (Sogenbi so in myInactiveMinions)
        {
            so.AddHandler(this);
        }

        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {

        if (isOnRange(viewDistance))
        {
            if (door != null)
            {
                //if (door.key != gameObject)
                //    door.changeKey(gameObject);
            }
            if (activeMinions < maxActiveMinions)
            {
                ActivateMinion();
            }
        }
    }

    public void ReceiveNotification(Notification note, Sogenbi source)
    {
        switch (note)
        { 
            case Notification.DYING:
               //print((("dead");
                maxActiveMinions++;
                myActiveMinions.Remove(source);
                myInactiveMinions.Remove(source);
                if(activeMinions > 0)
                    activeMinions--;


               //print((("activeMinions = " + myActiveMinions.Count);
               //print((("inactiveMinions = " + myInactiveMinions.Count);
                if (myActiveMinions.Count <= 0 && myInactiveMinions.Count <= 0)
                {
                    Destroy(gameObject);
                }

                break;
            case Notification.SLEEP:
                myInactiveMinions.Add(source);
                myActiveMinions.Remove(source);
                activeMinions--;
                break;
        }
    }

    protected bool isOnRange(float range) {
        return ( Mathf.Abs(player.transform.position.x - transform.position.x) < range);
    }

    private void ActivateMinion()
    {
        if(myInactiveMinions.Count == 0)
            return;

        int index = Random.Range(0,myInactiveMinions.Count);
        myActiveMinions.Add(myInactiveMinions[index]);
        myInactiveMinions[index].WakeUp();
        myInactiveMinions.RemoveAt(index);
        activeMinions++;
        
    }
}
