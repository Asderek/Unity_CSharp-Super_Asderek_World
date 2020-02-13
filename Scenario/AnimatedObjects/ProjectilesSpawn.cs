using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class ProjectilesSpawn : RepelProjectile{

    public GameObject target;
    public GameObject spawn;
    public float threshold = 7f;
    private float lastSpawn;
    public float SpawnCD = 4;
    private ArrayList spawned = new ArrayList();

    protected override void Start()
    {
        base.Start();
        lastSpawn = -SpawnCD;
        spawned.Add(spawn);
    }

    protected override void FixedUpdate()
    {
        
        base.FixedUpdate();
        if (transform.position.y >= threshold && (Time.time - lastSpawn)>SpawnCD)
        {
            lastSpawn = Time.time;
            Rigidbody2D rigidBody = transform.GetComponent<Rigidbody2D>();

            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0;
            rigidBody.isKinematic = true;
            //rigidBody.gravityScale = 0;

            GameObject aux = (GameObject)Instantiate(spawn, transform.position, transform.rotation);
            
            aux.SetActive(true);
            //transform.Translate(direction*20);

            spawned.Add(aux);
        }
    }

    void OnDestroy()
    {
        if (spawn != null)
        {
            spawn.SetActive(true);
            spawn.transform.position = transform.position;
        }

    }

    
    	
}
