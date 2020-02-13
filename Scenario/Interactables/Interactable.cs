using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour {

    protected UIManager manager;
    protected Asderek player;
    public string displayMsg;
    public ButtonManager.ButtonID button = ButtonManager.ButtonID.CIRCLE;
    protected bool inContact = false;
    public bool oneWay;
    public bool amStay = false;
    
    
    protected virtual void Start()
    {
        manager = UIManager.GetInstance();
    }

      protected virtual void OnTriggerExit2D(Collider2D colisor)
    {
        if (colisor.gameObject.GetComponentInParent<Asderek>() != null)
        {
            inContact = false;
            manager.StopDisplayOnScreen(gameObject);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D colisor)
    {

        if (amStay)
        {
            CheckDisplayOnScreen(colisor);  
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D colisor)
    {
        if (!amStay)
        {
            CheckDisplayOnScreen(colisor);
        }
    }

    private void CheckDisplayOnScreen(Collider2D colisor)
    {
        if (Assets.Scripts.Utilities.HitAsderek(colisor))
        {
            if (enabled == true)
            {
                player = colisor.gameObject.GetComponentInParent<Asderek>();
                if (player.IsAvailable())
                {
                    if (IsEnable(player))
                    {
                        HandlePlayerInteraction(); 
                    }
                }
            }
        }
    }

    protected virtual void HandlePlayerInteraction()
    {

        inContact = true;
        manager.InitDisplayOnScreen(gameObject, displayMsg, button);
    }

    public virtual void ActivateInteraction()
    {
        manager.StopDisplayOnScreen(gameObject);
        if (oneWay)
        {
            Destroy(this);
        }
    }

    public virtual bool IsEnable(Asderek player)
    {
        return true;
    }
 
}
