using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Interactable {

    [System.Serializable]
    public enum Status
    {
        On,
        Off
    };

    protected List<EventObserver> observers = new List<EventObserver>();
    public Status status;

   

    public override void ActivateInteraction()
    {
        bool ret;
        base.ActivateInteraction();

        if (GetComponent<Animator>() != null && !GetComponent<Animator>().GetBool("triggerReset"))
            GetComponent<Animator>().SetTrigger("triggerSwitch");

        if(status == Status.On)
        {
            status = Status.Off;
            ret = false;
        }
        else
        {
            status = Status.On;
            ret = true;
        }


        foreach (EventObserver observer in observers)
        {
            observer.SendNotification(ret, this);
        }

    }

    public virtual void RegisterObserver(EventObserver observer)
    {
        bool ret;
        observers.Add(observer);
        if (status == Status.On)
            ret = true;
        else
            ret = false;
        observer.SendNotification(ret, this);
    }

    public virtual void UnregisterObserver(EventObserver observer)
    {
        observers.Remove(observer);
    }

    public virtual void Reset()
    {

        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().ResetTrigger("triggerSwitch");
            GetComponent<Animator>().SetTrigger("triggerReset");
        }

    }

}
