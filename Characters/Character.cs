using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class Character : MonoBehaviour
{
    public enum Direction : int
    {
        Left = -1,
        Right = 1

    }

    [Header("----------------------------------------------------------------------")]
    /****/
    public DrawableManager drawManager;
    public Drawable displayHP;
    /****/

    public float invulnerabilityTime = 1;
    public float maxHP = 100;
    public float timeOfAccel = 0.3f;

    protected GameObject sprite;
    protected Rigidbody2D rigidBody;
    protected Animator animator;
    protected Transform groundVerifier;
    protected Transform ledgeVerifier;
    //protected List<AnimatorState> animatorStates;
    protected List<string> animatorStates;
    protected List<AnimatorControllerParameter> animatorParameters;
    protected AnimatorControllerParameter groundParameter, takingDamageParameter, dyingParameter,
                                                                                verticalParameter, attackTypeParameter;
    //protected AnimatorState currentState;
    protected string currentState;
    public Commandments.Element myElement = Commandments.Element.NEUTRAL;

    public AnimationClip[] anims;

    public float hp;
    protected float currentFriction;
    protected float rotation;
    public bool isOnGround;
    public bool isOnLedge;
    protected float desiredVelocity = 0f;
    protected bool accelerating = false;

    public float maxSpeedRatio = 1f;
    public float criticalChance = 5f;
    public float damageForce = 300;
    public float attackDamage = 25;

    protected float[] damageModifier;

    protected List<Collider2D> colliders;
    
    public Collider2D myBounds;
    public Vector2 offset;
    
    public float maxSpeed = 20;

    protected virtual void Start()
    {

        colliders = new List<Collider2D>();
        Collider2D[] vect = GetComponentsInChildren<Collider2D>();
        if (vect.Length > 0)
        {
            myBounds = vect[0];
         }
        for (int i = 0; i < vect.Length; i++)
        {
            if (vect[i].isTrigger)
            {
                colliders.Add(vect[i]);
            }
        }
        hp = maxHP;


        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.freezeRotation = true;



        animator = GetComponentInChildren<Animator>();

        //UnityEditor.Animations.AnimatorController controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

        //animatorStates = new List<UnityEditor.Animations.AnimatorState>();
        animatorParameters = new List<AnimatorControllerParameter>();
        animatorStates = new List<string>();
        currentState = "";
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {

            if (parameter.name == "isOnGround")
                groundParameter = parameter;
            else if (parameter.name == "verticalSpeed")
                verticalParameter = parameter;
            else if (parameter.name == "triggerTakingDamage")
                takingDamageParameter = parameter;
            else if (parameter.name == "triggerDying")
                dyingParameter = parameter;
            else if (parameter.name == "attackType")
                attackTypeParameter = parameter;
            else if (parameter.name.Contains("activate"))
                animatorParameters.Add(parameter);

        }

        foreach (AnimationClip a in anims)
        {
            animatorStates.Add(a.name);
        }


        damageModifier = new float[System.Enum.GetValues(typeof(Commandments.Element)).Length];
        for (int cont = 0; cont < System.Enum.GetValues(typeof(Commandments.Element)).Length - 1; cont++)
        {
            if (myElement == Commandments.Element.NEUTRAL)
                damageModifier[cont] = 1.0f;
            else
            {
                damageModifier[cont] = Commandments.damageTable[cont,myElement.toInt()];
            }
        }
        damageModifier[damageModifier.Length - 1] = 1;



        groundVerifier = null;
        ledgeVerifier = null;
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.tag == "GroundVerifier")
            {
                groundVerifier = t;
            }
            if (t.tag == "LedgeVerifier")
                ledgeVerifier = t;
        }

        drawManager.drawList.Add(displayHP);

        //displayHP.behavior = Drawable.Behavior.PERCENTAGE;
        //displayHP.size.width = 0.04f;
        //displayHP.size.height = 0.0075f;
        //displayHP.percent = 1;

    }

    protected virtual void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        
        isOnGround = GroundVerification();
        
        isOnLedge = LedgeVerification();
        if (groundParameter != null)
            animator.SetBool(groundParameter.name, isOnGround);

        if(verticalParameter != null)
            SetVerticalSpeed();


        SynchronizeAnimation();

        SetNextStageParameters(CurrentUpdate());


        ControlVelocity();



    }

    protected virtual void SetVerticalSpeed()
    {
        float verticalSpeed;
        float velocityMargin = 0.05f;
        if (!isOnGround)
        {
            verticalSpeed = rigidBody.velocity.y;
            if ((verticalSpeed < velocityMargin) && (verticalSpeed > -velocityMargin))
            {
                animator.SetInteger("verticalSpeed", 0);
            }
            else if (verticalSpeed > velocityMargin)
            {
                animator.SetInteger("verticalSpeed", 1);
            }
            else
            {
                animator.SetInteger("verticalSpeed", -1);
            }
        }
        else
        {
            verticalSpeed = 0;
            animator.SetInteger("verticalSpeed", 0);
        }
    }


    public virtual void FixedUpdate()
    {
        drawManager.FixedUpdate();
        //accelerating = false;
        //isOnGround = GroundVerification();
        //if (groundParameter != null)
        //    animator.SetBool(groundParameter.name, isOnGround);

        //SynchronizeAnimation();

        //SetNextStageParameters(CurrentUpdate());

    }

    public virtual float ReceiveDamage(Vector2 direction, float force, float damage, bool critical = false, Commandments.Element enemyElement = Commandments.Element.NEUTRAL, Character source = null)
    {

        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);

        rigidBody.AddForce(direction * force);

        if (takingDamageParameter != null)
            animator.SetTrigger(takingDamageParameter.name);

        //damage *= Utilities.damageFromTo(element, myElement);

        //hp -= damage;
        ChangeHP(-damage);

        return damage;
    }

    public virtual void OnDestroy()
    {
        /*remove todas as referencias para esse objeto*/
    }

    protected virtual string CurrentUpdate()
    {
        ////print((("Nao devia etsar aqui");
        return null;
    }

    protected virtual bool GroundVerification()
    {
        if (groundVerifier == null)
            return true;

        return (Physics2D.Linecast(transform.position, groundVerifier.position, 1 << LayerMask.NameToLayer("Floor")) /*&& rigidBody.velocity.y == 0*/);
    }

    protected virtual bool LedgeVerification()
    {
        if (ledgeVerifier == null || !isOnGround)
            return false;

        return !(Physics2D.Linecast(transform.position, ledgeVerifier.position, 1 << LayerMask.NameToLayer("Floor")));
    }


    protected virtual void SynchronizeAnimation()
    {

        foreach (string testState in animatorStates)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(testState))
            {

                if ((currentState != testState) && (currentState != null))
                {
                    ChangeStateEvent(currentState, testState);
                }
                currentState = testState;
                return;
            }

        }
        ////print((("[" + this.GetType().Name + "] SynchronizeAnimation: State not found");
    }

    protected virtual void SetNextStageParameters(string nextState)
    {
        foreach (AnimatorControllerParameter parameter in animatorParameters)
        {

            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, string.Equals(parameter.name, nextState, System.StringComparison.OrdinalIgnoreCase));
            }

        }

    }

    protected virtual void AchieveMaxSpeed(float MaxSpeed)
    {
        float direction = (((int)transform.eulerAngles.y) / -90) + 1;

        accelerating = true;
        desiredVelocity = rigidBody.velocity.x + (direction * MaxSpeed * maxSpeedRatio - rigidBody.velocity.x) / timeOfAccel;
    }

    protected virtual void ChangeStateEvent(string currentState, string nextState) { }

    protected virtual void OnCollisionEnter2D(Collision2D colidido)
    {
        if (colidido.collider.GetComponent<BoxCollider2D>() != null)
        {
            if (colidido.collider.GetComponent<BoxCollider2D>().sharedMaterial != null)
            {
                currentFriction = colidido.collider.GetComponent<BoxCollider2D>().sharedMaterial.friction;
                isOnGround = GroundVerification();
                rotation = colidido.collider.transform.rotation.eulerAngles.z;
                ////print((("CurrentFriction = "+currentFriction);
            }


        }
        if (colidido.collider.GetComponent<Floor>() != null)
        {
            timeOfAccel *= colidido.collider.GetComponent<Floor>().accelTimeRatio;
            maxSpeedRatio *= colidido.collider.GetComponent<Floor>().maxSpeedRatio;
        }

    }

    protected virtual void OnCollisionExit2D(Collision2D colidido)
    {
        if (rigidBody == null)
            return;

        if (!rigidBody.IsTouchingLayers())
        {
            currentFriction = 0;
            rotation = 0;
        }
        if (colidido.collider.GetComponent<Floor>() != null)
        {
            timeOfAccel /= colidido.collider.GetComponent<Floor>().accelTimeRatio;
            maxSpeedRatio /= colidido.collider.GetComponent<Floor>().maxSpeedRatio;
        }
    }

    public virtual void Dying()
    {
        Destroy(gameObject);
    }

    protected virtual void ControlVelocity()
    {
        if (currentState.ToLower().Equals("takingdamage"))
            return;

        float desiredY = rigidBody.velocity.y;
        if (!accelerating && isOnGround)
        {
            desiredVelocity = rigidBody.velocity.x - rigidBody.velocity.x / (timeOfAccel * 5);
            desiredY = rigidBody.velocity.y - rigidBody.velocity.y / (timeOfAccel * 5);
        }

        if (desiredVelocity > 0)
            desiredVelocity = Mathf.Min(desiredVelocity, maxSpeed);
        else
            desiredVelocity = Mathf.Max(desiredVelocity, -1*maxSpeed);

        if (desiredY > 0)
            desiredY = Mathf.Min(desiredY, maxSpeed);
        else
            desiredY = Mathf.Max(desiredY, -1 * maxSpeed);
        
        rigidBody.velocity = new Vector2(desiredVelocity, desiredY);
        accelerating = false;
    }

    public virtual void ChangeHP(float value)
    {
        hp += value;
        if (hp > maxHP)
            hp = maxHP;
        if (hp < 0)
            hp = 0;

        //print("CHARACTER_hp/maxHP = " + (hp / maxHP));
        displayHP.percent = hp / maxHP;
    }

    public virtual bool GetItem(Item item)
    {
        item.user = gameObject;
        return item.useItem();
    }

    public virtual Commandments.Element getElement()
    {
        return myElement;
    }

    public void updateSpeed(float timeOfAccel, float maxSpeedRatio)
    {
        this.timeOfAccel *= timeOfAccel;
        this.maxSpeedRatio *= maxSpeedRatio;
    }

    public float getDamageModifier(Commandments.Element elemento)
    {
        return damageModifier[elemento.toInt()];
    }

    public virtual bool ColliderContains(Collider2D enemyCollider)
    {
  
        if(Utilities.Intersects(enemyCollider.bounds, myBounds.bounds))
        {
            return true;
        }  
        return false;
    }

    protected virtual int getDirection()
    {
        return (((int)transform.eulerAngles.y) / -90) + 1; /*+1 right -1 left*/
    }

    protected virtual void TurnCharacter(Direction dir)
    {
        transform.eulerAngles = new Vector2(0, -((int)dir - 1) * 90);
        //transform.eulerAngles = new Vector2(0, (float)dir);
    }

    protected virtual void TurnCharacter(Vector2 destiny)
    {
        if ((transform.position.x - destiny.x) > 0)
        {
            TurnCharacter(Direction.Left);
        }
        else
            TurnCharacter(Direction.Right);
    }

    public void SetDying()
    {
        animator.SetTrigger("triggerDying");
    }
    
    protected virtual bool isFacing(GameObject target)
    {
        float direction;
        direction = target.transform.position.x - transform.position.x;

        if ((direction * getDirection()) > 0)
            return true;
        else
            return false;

  

    }

    protected virtual void OnGUI()
    {
        if(Time.timeScale != 0)
            drawManager.OnGUI(transform.position);
    }

    protected virtual void LookTo(GameObject obj)
    {
        if (transform.position.x > obj.transform.position.x)
        {
            TurnCharacter(Direction.Left);
        }
        else
            TurnCharacter(Direction.Right);
    }

}
