using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSwitcher : EventObserver {

    public GameObject enables;
    public GameObject disables;

    protected override void Start()
    {
        base.Start();
        if(disables!=null)
            disables.SetActive(false);

        if (enables != null)
            enables.SetActive(true);
    }

    public override void SendNotification(bool status, Switch source)
    {
            if (enables != null)
                enables.SetActive(!status);

            if (disables != null)
                disables.SetActive(status);
    }
}
