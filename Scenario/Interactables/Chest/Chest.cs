using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Chest : NotifyingInteractables {

    [Header("----------------------------------------------------------------------")]
    public Progress.GameItems type;

    protected Animator animator;
    protected GameObject obj;
    public Vector2 endPosition;
    [Range(0,1f)]
    public float velocity;
    [Range(0, 1f)]
    public float velocityFade;
    private float lerp;
    private bool alreadyDone = false;
    private bool alreadyReturn = false;
    

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();

    }

    protected virtual void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && animator.GetCurrentAnimatorStateInfo(0).IsName("opening"))
        {
            print("alreadyDone: " + alreadyDone);
            if (alreadyDone == false)
            {
                alreadyDone = true;

                print(type + " -> " + Progress.GetSprite(type).name);
                obj = Instantiate(Progress.GetSprite(type), transform.position + new Vector3(0, 0, 2), Quaternion.identity);
              
                foreach (Collider2D col in obj.GetComponentsInChildren<Collider2D>())
                {
                    col.enabled = false;
                }

                obj.SetActive(true);
                lerp = 0;
            }
        }

        if (obj != null)
        {
            if (lerp > 0.4f)
            {
                if (alreadyReturn == false)
                {
                    alreadyReturn = true;
                    player.GetComponent<Asderek>().ReceiveNotification(Asderek.Notification.Return);
                }
            }

            if (lerp < 1)
            {
                Vector3 v3 = Vector2.Lerp(transform.position, (Vector2) transform.position + endPosition, lerp);
                v3.z = transform.position.z + 2;
                obj.transform.position = v3;
                lerp += velocity;
            }
            else if (lerp < 2)
            {
                Color init = obj.GetComponent<SpriteRenderer>().color;
                init.a = 2 - lerp;
                obj.GetComponent<SpriteRenderer>().color = init;
                lerp += velocityFade;
            }
            else
            {
                Destroy(obj);
                manager.AddItem(type);
                Destroy(this);
            }

        }

    }

    public override void ActivateInteraction()
    {
        manager.StopDisplayOnScreen(gameObject);
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            if (col.isTrigger)
                col.enabled = false;
        }
        animator.SetTrigger("triggerSwitch");
        player.GetComponent<Asderek>().ReceiveNotification(notificationType);
        alreadyDone = false;
        alreadyReturn = false;
        
    }

}
