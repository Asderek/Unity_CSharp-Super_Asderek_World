using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Spawner : MonoBehaviour
{
    public Spawnee model;
    protected Spawnee  spawnee;
    private float spawnTime = 5;

    [HideInInspector] 
    public bool isRandom;
    [HideInInspector]
    public float maxDelay;


    protected virtual void Awake() {
        model.enabled = false;
        model.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        ApplyOnce.apply("respawn", gameObject, () =>
        {
            CreateInstance();
            return true;
        });
    }

    protected virtual void OnDisable()
    {
        if(spawnee != null)
        {
            spawnee.Tick -= new Spawnee.TickHandler(RespawnAfter);
            Destroy(spawnee.gameObject);
        }
        ApplyOnce.remove("respawn", gameObject);
    }

    protected virtual void OnDestroy()
    {
    }

    protected virtual void RespawnAfter(float time)
    {
        spawnTime = time;

        if (isRandom)
            spawnTime += UnityEngine.Random.Range(0, maxDelay);

        if(isActiveAndEnabled)
            StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        if (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(spawnTime);
            CreateInstance();
        }
    }


    protected virtual void CreateInstance()
    {
        spawnee = (Spawnee)Instantiate(model, new Vector3(transform.position.x, transform.position.y, model.gameObject.transform.position.z), model.gameObject.transform.rotation);
        spawnee.enabled = true;
        spawnee.gameObject.SetActive(true);
        spawnee.Tick += new Spawnee.TickHandler(RespawnAfter);
    }


}

#if UNITY_EDITOR
[CustomEditor(typeof(Spawner),true)]
public class RandomScript_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields

        Spawner script = (Spawner)target;

        // draw checkbox for the bool
        script.isRandom = EditorGUILayout.Toggle("isRandom", script.isRandom);
        if (script.isRandom) // if bool is true, show other fields
        {
            script.maxDelay = EditorGUILayout.FloatField("maxDelay", script.maxDelay);
            //script.Template = EditorGUILayout.ObjectField("Template", script.Template, typeof(GameObject), true) as GameObject;
        }
    }
}
#endif
