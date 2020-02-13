using UnityEngine;
using System.Collections;

public class BackGround : MonoBehaviour {

    public GameObject player;


   void FixedUpdate() 
    {
    float newX, newY;
    newX = player.transform.position.x - transform.position.x;
    newY = player.transform.position.y - transform.position.y;

    transform.Translate(newX, newY,0);

     //Vector3 novaPosicao = new Vector3(newX, newY, transform.position.z);
     //transform.position = Vector3.Lerp(transform.position, novaPosicao, Time.time);
    }
}
