using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimatingSpawn : Spawner {

    private Animator animator;
    public float percent;
    private bool alreadyDone;

    void Awake()
    {
        animator = GetComponent<Animator>();
        base.Awake();
    }


    protected override void Update()
    {
        base.Update();
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("drip") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > percent)
        {

            if (alreadyDone)
                return;


            spawnee = (Spawnee)Instantiate(model, model.gameObject.transform.position, model.gameObject.transform.rotation);
            spawnee.enabled = true;
            spawnee.gameObject.SetActive(true);
            spawnee.Tick += new Spawnee.TickHandler(RespawnAfter);
            alreadyDone = true;
        }

    }

    protected override void CreateInstance()
    {
        if (animator == null)
            return;

        animator.SetTrigger("triggerActivate");
        alreadyDone = false;
    }
	
}


#if UNITY_EDITOR
[CustomEditor(typeof(AnimatingSpawn))]
public class MyClassEditor : RandomScript_Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Additional code for the derived class...
    }
}
#endif
