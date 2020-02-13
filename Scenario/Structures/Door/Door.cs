using UnityEngine;
using System.Collections;

public abstract class Door : EventObserver {

    protected bool state=false ;

    public abstract void open();
    public abstract void close();

    protected virtual void Update() {
        if (state == true)
            open();
        else
            close();
        
    }

    public override void SendNotification(bool status, Switch source)
    {
        state = status;
    }

}
