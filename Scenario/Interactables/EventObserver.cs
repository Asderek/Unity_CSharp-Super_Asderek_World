using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObserver : MonoBehaviour {

    public List<Switch> observables;

	// Use this for initialization
	protected virtual void Start () {
        foreach (Switch ob in observables)
        {
            if ( ob != null)
                ob.RegisterObserver(this);
        }
	}
	

    public virtual void SendNotification(bool status, Switch source)
    {
        //print((("received notification with source");
    }
}
