using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System.Collections.Generic;

public class DamageFloor : MonoBehaviour {

    public float damage = 2;
    public float damageForce;
    public Commandments.Element myElement;

    public float maxSpeedRatio;


    protected List<GameObject> visitors;

	// Use this for initialization
    protected virtual void Start()
    {

        visitors = new List<GameObject>();
    }


    protected virtual void FixedUpdate()
    {

        foreach (GameObject visitante in visitors)
        {
            if (visitante != null)
            {
                if (visitante.GetComponent<Character>() != null)
                {

                    visitante.GetComponent<Character>().ReceiveDamage(CalculateDirection(visitante), damageForce, damage,false,myElement);
                    if (visitante.GetComponent<Character>().hp <= 0)
                    {
                        visitors.Remove(visitante);
                        break;
                    }
                }
            }
            else
            {
                visitors.Remove(visitante);
                break;
            }
        }
    }

    protected virtual Vector2 CalculateDirection(GameObject obj)
    {
        return Utilities.standardVector(obj.transform.position.x - transform.position.x, 30f);
    }

    void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.gameObject.transform.parent != null)
        {

            visitors.Add(colisor.gameObject.transform.parent.gameObject);
            if (colisor.gameObject.transform.parent.gameObject.GetComponent<Character>() != null)
                colisor.gameObject.transform.parent.gameObject.GetComponent<Character>().updateSpeed(1, maxSpeedRatio);
        }
        
    }

    void OnTriggerExit2D(Collider2D colisor)
    {


        if (colisor.gameObject.transform.parent != null)
        {

            visitors.Remove(colisor.gameObject.transform.parent.gameObject);
            if (colisor.gameObject.transform.parent.gameObject.GetComponent<Character>() != null)
                colisor.gameObject.transform.parent.gameObject.GetComponent<Character>().updateSpeed(1, 1.0f/maxSpeedRatio);
        }
    }


}
