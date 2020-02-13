using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MultiPath))]
public class MultiPathObserver : EventObserver {

    private MultiPath path;

    protected override void Start()
    {
        path = GetComponent<MultiPath>();
        base.Start();
    }

    public override void SendNotification(bool status, Switch source)
    {
        base.SendNotification(status, source);
        if(status)
            path.ChangePath(source);
    }

}
