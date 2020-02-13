using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AreaEffector : MonoBehaviour {

    protected List<GameObject> visitors;
    public Transform camera;
   
    protected abstract void applyEffect(GameObject visitor);
    protected abstract void newVisitor(GameObject visitor);
    protected abstract void exitVisitor(GameObject visitor);
    protected abstract void onScreenEnter();
    protected abstract void onScreenExit();
    protected bool onScreen=false;

    protected virtual void LateUpdate()
    {
        foreach (GameObject visitante in visitors)
        {

            if (visitante != null)
            {
                applyEffect(visitante);
            }
            else
            {
                visitors.Remove(visitante);
                break;
            }
        }
    }
    
    protected virtual void Start()
    {
        visitors = new List<GameObject>();
        //camera = GameManager.GetInstance().getCurrentCamera().transform;

    }

    protected virtual void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.gameObject.GetComponent<Camera>() != null)
        {
            onScreenEnter();
            onScreen = true;
            return;
        }

        if (colisor.gameObject.transform.parent == null)
        {
            return;
        }

        GameObject parent = colisor.gameObject.transform.parent.gameObject;

        if (parent.GetComponent<Character>() == null)
            return;

        if (visitors.Contains(parent))
            return;

        visitors.Add(parent);
        newVisitor(parent);
    }



    protected virtual void OnTriggerExit2D(Collider2D colisor)
    {
        if (colisor.gameObject.GetComponent<Camera>() != null)
        {
            onScreenExit();
            onScreen = false;
            return;
        }

        if (colisor.gameObject.transform.parent == null)
        {
            return;
        }
        exitVisitor(colisor.gameObject.transform.parent.gameObject);
        visitors.Remove(colisor.gameObject.transform.parent.gameObject);
        
    }

    //protected void WarnUI(bool state)
    //{
    //    UIManager manager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    //    manager.ActiveWarning(state);
    //}
}
