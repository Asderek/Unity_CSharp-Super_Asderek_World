using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foreground : MonoBehaviour {

    public float radius = 4.5f;
    public float growthVelocity = 1f;
    private float currentRadius = 0;
    bool asderek;

    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void OnTriggerStay2D(Collider2D colisor)
    {
        if (Assets.Scripts.Utilities.HitAsderek(colisor))
        {
            asderek = true;

            if (currentRadius <= radius)
            {
                currentRadius += growthVelocity;
                if (currentRadius > radius)
                {
                    currentRadius = radius;
                }

                mat.SetFloat("_Radius", currentRadius);
            }

            GameObject player = colisor.gameObject;
            Vector2 playerRelativePosition = player.transform.position - transform.position;

            playerRelativePosition.y += 1f;

            playerRelativePosition.x /= transform.lossyScale.x;
            playerRelativePosition.y /= transform.lossyScale.y;

            float newX = (0.5f + playerRelativePosition.x) * mat.mainTextureScale.x;
            float newY = (0.5f + playerRelativePosition.y) * mat.mainTextureScale.y;

            mat.SetVector("_Center", new Vector4(newX,newY,0,0));




        }
    }

    void OnTriggerExit2D(Collider2D colisor)
    {
        if (colisor.gameObject.tag == "PlayerSpriteTag")
        {
            if (!colisor.isTrigger)
            {
                asderek = false;
                mat.SetFloat("_Radius", 0);
            }
        }
    }

    void Update()
    {
        if (asderek == false && currentRadius > 0)
        {
                currentRadius -= growthVelocity;
                if (currentRadius < 0)
                {
                    currentRadius = 0;
                }

                mat.SetFloat("_Radius", currentRadius);
            }
        }

}
