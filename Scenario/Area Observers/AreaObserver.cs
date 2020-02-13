using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]

public class AreaObserver : MonoBehaviour {
    
    protected List<MonoBehaviour> modifiedScripts;
    protected List<Collider2D> modifiedColliders;
    protected Collider2D area;

    public bool reactToEnemy;
    public bool reactToPlayer;
    protected bool ready;

	// Use this for initialization
    protected virtual void Awake()
    {

        if (GetComponentInParent<Spawner>() != null)
        {
            gameObject.SetActive(false);
            return;
        }


        ready = false;
        area = GetComponent<PolygonCollider2D>();
        area.isTrigger = false;

        modifiedScripts = new List<MonoBehaviour>();
        MonoBehaviour[] scripts = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour instance in scripts)
        {
            if ((instance != this) && (instance.enabled))
            {
                instance.enabled = false;
                modifiedScripts.Add(instance);
            }
        }

            modifiedColliders = new List<Collider2D>();

            Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D instance in colliders)
            {
                if ((instance.isTrigger) && instance.enabled)
                {
                    instance.enabled = false;
                    modifiedColliders.Add(instance);
                }
            }

            area.isTrigger = true;

            
	}

    protected virtual void LateUpdate()
    {

        if (ready)
        {
            foreach (MonoBehaviour instance in modifiedScripts)
            {
                instance.enabled = true;
            }

            foreach (Collider2D instance in modifiedColliders)
            {
                instance.enabled = true;
                
            }
            enabled = false;

        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        Transform parent = collider.gameObject.transform.parent;
        if (parent != null)
        {
            if (parent.gameObject.GetComponent<Asderek>() != null)
            {
                if (!reactToPlayer)
                {
                    return;
                }
            }
            else if (parent.gameObject.GetComponent<Enemy>() != null)
            {
                if (!reactToEnemy)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            area.enabled = false;
            ready = true;
           
        }
    }


	
}
