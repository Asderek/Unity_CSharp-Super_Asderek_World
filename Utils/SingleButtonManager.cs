using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System;

public class SingleButtonManager {

    private static float deadZone = 0.4f;

    [System.Serializable]
    public enum Type {
        Digital,
        Trigger_Up,
        Trigger_Down
    };
    
    public ButtonManager.ButtonConfig config;
    public ButtonManager.ButtonID id;
    public System.Object requester = null;

    private float lastDown; 
    private float longClick = 30f;
    private State currentState;

    public enum State { 
        None,
        Pressed,
        Hold,
        Short,
        Long
    };

    public SingleButtonManager(ButtonManager.ButtonID id, ButtonManager.ButtonConfig config) {
        this.id = id;
        this.config = config;
        currentState = State.None;
        lastDown = longClick + 1;
    }
    

	public bool Update () {
        bool ret = false;
        if ((currentState == State.Pressed) && ( lastDown >= longClick/2))
        {
            currentState = State.Hold;
        }

        if ( Get() )
        {
            ApplyOnce.apply("pressed", this, () =>
            {
                lastDown = 0;
                currentState = State.Pressed;
                ret = true;
                return true;
              });
        }
        else
        {
            if (ApplyOnce.alreadyApplied("pressed", this))
            { 
                if ( lastDown >= longClick)
                {
                    currentState = State.Long;
                }
                else
                {
                    currentState = State.Short;
                }
                ApplyOnce.remove("pressed", this);
            } else
            {
                currentState = State.None;
                requester = null;
            }
        }

        lastDown++;
        return ret;
    }

    public bool GetUp(System.Object requester) {
        if (currentState == State.Long || currentState == State.Short)
            if ((this.requester == null) || (this.requester == requester))
            {
                this.requester = requester;
                return true;
            }
        return false;
    }

    public bool GetDown(System.Object requester)
    {
        if (lastDown == 1)
            if((this.requester == null) || (this.requester == requester))
            {
                this.requester = requester;
                return true;
            }
        return false;
    }

    public bool Get()
    {
        switch (config.type)
        {
            case Type.Digital:
                return Input.GetButton(config.inputName);

            case Type.Trigger_Down:
                return Input.GetAxisRaw(config.inputName) < -deadZone;

            case Type.Trigger_Up:
                return Input.GetAxisRaw(config.inputName) > deadZone;

        }
        return false;
    }

    public float GetValue()
    {
        switch (config.type)
        {
            case Type.Digital:
                return Input.GetButton(config.inputName) ? 1 : 0;

            case Type.Trigger_Down:
                float value = -Input.GetAxisRaw(config.inputName);
                if (value < 0)
                    value = 0;
                return value;

            case Type.Trigger_Up:
                value = Input.GetAxisRaw(config.inputName);
                if (value < 0)
                    value = 0;
                return value;
        }
        return 0;
    }

    public State GetState(System.Object requester)
    {
        if (currentState != State.None)
            if ((this.requester == null) || (this.requester == requester))
            {
                this.requester = requester;
                return currentState;
            }
        return State.None;
    }
}
