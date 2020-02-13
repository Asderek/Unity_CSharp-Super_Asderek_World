    using UnityEngine;
using System.Collections;
using Assets.Scripts;


using System.Collections.Generic;
using System;

public class Asderek : Character
{
    [Header("----------------------------------------------------------------------")]
    public float forca = 10;
    public float frames = 10;
    public bool cushined = true;
    private bool goingToLerpLocation = false; 

    #region Player Types

    enum AttackType
    {
        QuickAttack,
        HeavyAttack,
        HoldingAttack,
        QuickSpecial,
        HeavySpecial,
        HoldingSpecial,
        Rolling,
        Air,
        Nothing = 666
    }

    enum PlayerInputAnimation
    {
        Nothing,
        JumpShort,
        JumpLong,
        DownJump,
        QuickAttack,
        QuickSpecial,
        HeavyAttack,
        HeavySpecial,
        HoldingAttack,
        HoldingSpecial,
        Skill,
        Rolling,
        Ultimate,
        Ability_Triangle,
        Ability_Circle,
        Ability_X,
        Ability_Square
    };

    enum PlayerInputMovement
    {
        Left,
        Right,
        Up,
        Down
    };


    [System.Serializable]
    public enum Notification
    {
        Return,
        Sit,
        Water,
        Kneel
    };


    #endregion

    #region Variables

    public int attackType;

    public float speedRunning = 1.7f;
    public float speedRolling = 1.7f;
    public float cooldown = 10;
    public float jumpForce = 3;
    public float jumpAngle = 45;
    public float fallDamage = 25;

    public float mp;
    public float maxMP = 100;
    private float mpCost = 25;

    private float lastAttack;
    private float attackCD = 0.4f;

    private float lastInput;
    public float sittingTimer;

    private float lastSpecial;
    public float specialCD = 2f;

    private float lastVulnerability;
    private float verticalSpeed;
    private bool isOnWall = false;
    private float maxGravity;
    private float acceleration;
    private float dashForce = 300;
    private float value = 15f;
    private float factorR = 1.5f;
    private float factorW = 2f;

    public bool[] status = new bool[3];

    public float ultimate=0f;
    
    private int wallDirection;

    private UIManager uiManager;
    private GameManager gameManager;

    private Blink blinker;

    private List<PlayerInputMovement> commands;
    private PlayerInputAnimation nextAnimationInput;

    public GameObject currentFloor;
    private GameObject currentLadder;
    private GameObject currentWall;

    private bool newCommandExist;
    public float doubleJumpForce;
    public float velocityMargin = 0.05f;

    private Vector2 destiny;
    private Vector2 destinyDirection;
    public float grabLimit = 0.05f;
    public float ledgeVerificationRange = 0.5f;
    public bool toClimb = false;

    private ButtonManager buttons;
    public Vector2 mySize;

    private List<Collider2D> ignoredCollisions;

    public float mpChannelCost;
    private float facingDirection;

    [HideInInspector]
    public List<Commandments.Modifiers> modifiers = new List<Commandments.Modifiers>();
    private bool groundContact = false;

    
    protected GameObject tempParent;

    private Vector3 startPosition;
    [HideInInspector]
    public bool isDownJumping = false;


    #endregion

    #region Control Function

    protected override void Start()
    {
        base.Start();
        gameObject.tag = "Player";
        uiManager = UIManager.GetInstance();
        gameManager = GameManager.GetInstance();
        gameManager.RegisterPlayer(this);
        blinker = gameObject.AddComponent<Blink>();

        ignoredCollisions = new List<Collider2D>();

        //transform.eulerAngles = new Vector2(0, 180);
        lastAttack = -cooldown;
        lastSpecial = -specialCD;
        maxGravity = rigidBody.gravityScale;
        

        nextAnimationInput = PlayerInputAnimation.Nothing;
        commands = new List<PlayerInputMovement>();

        mp = maxMP;
        currentFloor = null;

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.gameObject.tag == "PlayerSpriteTag")
            {
                sprite = obj.gameObject;
                break;
            }
        }

        buttons = ButtonManager.GetInstance();

        startPosition = transform.position;

        mySize = GetComponentInChildren<BoxCollider2D>().size * sprite.transform.localScale.y;
        displayHP.offset = getPlayerPosition() - (Vector2)transform.position;

    }    

    protected override void Update()
    {
        base.Update();
        animator.SetInteger("weapon", (int)uiManager.getSelectedWeapon());
        
        if ( Time.timeScale == 0)
        {
            return;
        }
        

        desiredVelocity = 0;

        //animator.SetInteger("attackType", attackType);
        nextAnimationInput = CheckPlayerInputAnimation(nextAnimationInput);
        CheckPlayerInputMovement(commands);
        newCommandExist = true;
        //print((("Velocity:" + rigidBody.velocity.y);


        if (!isInvulnerable())
        {
            blinker.Stop();
            ApplyOnce.apply(Commandments.Modifiers.RemoveIgnoreColisions.ToString(), gameObject, () =>
                {
                    if (ignoredCollisions.Count > 0)
                    {

                        foreach (Collider2D collider in ignoredCollisions)
                        {
                            Physics2D.IgnoreCollision(myBounds, collider, false);
                        }
                        ignoredCollisions.Clear();
                    }

                    return true;
                });
        }

    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        ApplyModifiers();
    }
    protected override string CurrentUpdate()
    {
        if ((Time.timeScale == 0) || (ApplyOnce.alreadyApplied("Notification",gameObject)))
        {
            return null;
        }

        if (goingToLerpLocation)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(destiny.x,destiny.y,transform.position.z), 5*Time.deltaTime);
            if (Mathf.Abs(transform.position.x - destiny.x) < 0.05f)
            {
                goingToLerpLocation = false;
            }
        }

        if ((!newCommandExist) && (!currentState.ToLower().Contains("takingdamage")))
        {
            nextAnimationInput = CheckPlayerInputAnimation(nextAnimationInput);
            CheckPlayerInputMovement(commands);
        }

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
                //if (verticalSpeed < -20)
                //    animator.SetInteger("landingLevel", 2);
                //else if (verticalSpeed < -15)
                //{
                //    animator.SetInteger("landingLevel", 1);
                //}
                //else
                //    animator.SetInteger("landingLevel", 0);
            }
        }
        else
        {
            verticalSpeed = 0;
            animator.SetInteger("verticalSpeed", 0);
            if (rigidBody.velocity.y == 0)
            {
                ApplyOnce.remove("Jump", gameObject);
                ApplyOnce.remove("DoubleJumpActivated", gameObject);
            }
        }

        float horizontalSpeed = rigidBody.velocity.x;
        if ((horizontalSpeed < velocityMargin) && (horizontalSpeed > -velocityMargin))
            animator.SetInteger("horizontalSpeed", 0);
        else if (horizontalSpeed > velocityMargin)
            animator.SetInteger("horizontalSpeed", 1);
        else 
            animator.SetInteger("horizontalSpeed", -1);

        facingDirection = (((int)transform.eulerAngles.y) / -90) + 1;

        string activateState = null;
        //print("Current State: " + currentState);

        switch (currentState.ToLower().Replace("_","."))
        {

            case "sitting.death":
            case "sitting.fire":
            case "sitting.earth":
            case "sitting.ice":
            case "sitting.steel":  
            case "sitting.water":
                if (nextAnimationInput != PlayerInputAnimation.Nothing || commands.Count != 0)
                {
                   //print((("exit");
                    return "activateChilling";
                }
                break;

            case "climbing.right":
            case "climbing.left":
                activateState = Climbing(nextAnimationInput, commands);
                break;

            case "climbing.pullingup":
                activateState = PullingUp(nextAnimationInput, commands);
                break;

            case "climbing.preparing":
                activateState = ClimbingPreparing(nextAnimationInput, commands);
                break;

            case "climbing.downprep":
                activateState = ClimbingPreparingDown(nextAnimationInput, commands);
                break;

            case "chilling":
                activateState = Chilling(nextAnimationInput, commands);
                break;

            case "running":
                activateState = Running(nextAnimationInput, commands);
                break;

            case "jumping.rising":
            case "jumping.falling":
                activateState = Jumping(nextAnimationInput, commands);
                break;

            case "jumping.double":
                activateState = JumpingDouble(nextAnimationInput, commands);
                break;

            case "jumping.preparing":
                activateState = JumpingPreparing(nextAnimationInput, commands);
                break;
                
            case "jumping.landingsoft":
                activateState = JumpingLandingSoft(nextAnimationInput, commands);
                break;

            case "jumping.landingheavy":
                activateState = JumpingLandingHeavy(nextAnimationInput, commands);
                break;

            case "jumping.sliding":
                activateState = JumpingSliding(nextAnimationInput, commands);
                break;

            case "rolling":
                activateState = Rolling(nextAnimationInput, commands);
                break;

            /*Criado*/
            case "pullingup":
                activateState = PullingUp(nextAnimationInput, commands);
                break;
            case "attacking.bow.holding":
            case "attacking.bow.walking":
                activateState = AttackingBow(nextAnimationInput, commands, 0);
                break;
            case "attacking.bow.release":
                activateState = "activateChilling";
                break;

            case "attacking.death.normal.quickp1":
            case "attacking.death.normal.quickp2":
            case "attacking.death.normal.quickp3":
            case "attacking.death.normal.heavyp1":
            case "attacking.death.normal.heavyp2":
                activateState = Attacking(nextAnimationInput, commands);
                break;

            case "attacking.death.normal.air":
            case "attacking.death.normal.airwait":
            case "attacking.water.normal.air":
            case "attacking.water.normal.airwait":
                activateState = AttackingAir(nextAnimationInput, commands);
                break;

            case "attacking.water.normal.quickp1":
            case "attacking.water.normal.quickp2":
            case "attacking.water.normal.quickp3":
            case "attacking.water.normal.heavyp1":
                activateState = Attacking(nextAnimationInput, commands,true);
                break;

            case "attacking.death.special.quick":
            case "attacking.death.special.heavy": 
                activateState = AttackingDeathSpecial(nextAnimationInput, commands);
                break;

            case "attacking.water.special.heavy":
                ApplyOnce.apply("OnceInState", gameObject, () => {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f) 
                    {
                        ChangeMP(-2 * mpCost);
                        return true;
                    }
                    return false;
                });
                activateState = Attacking(nextAnimationInput, commands, true);
                break;

            case "attacking.water.special.quick":
                ApplyOnce.apply("OnceInState", gameObject, () =>
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
                    {
                        ChangeMP(-mpCost);
                        return true;
                    }
                    return false;
                });
                activateState = Attacking(nextAnimationInput, commands,true);
                break;

            case "attacking.death.normal.hold":
            case "attacking.death.special.hold":
                animator.SetTrigger("triggerFinishAttack");
                switch (nextAnimationInput)
                {
                    case PlayerInputAnimation.QuickSpecial:
                    case PlayerInputAnimation.HeavySpecial:
                    case PlayerInputAnimation.QuickAttack:
                    case PlayerInputAnimation.HeavyAttack:
                        //Utilities.//print((("PrepAttack");
                        animator.ResetTrigger("triggerFinishAttack");
                        return PrepareAttack(convertToAttack(nextAnimationInput));
                }
                activateState = "activateChilling";
                break;

            case "attacking.water.normal.hold":
                switch (nextAnimationInput)
                {
                    case PlayerInputAnimation.QuickAttack:
                    case PlayerInputAnimation.HeavyAttack:
                        animator.ResetTrigger("triggerFinishAttack");
                        return PrepareAttack(convertToAttack(nextAnimationInput));
                }
                activateState = "activateChilling";
                break;

            case "attacking.water.normal.heavyp2":
            case "attacking.water.special.hold":
                return AttackingWaterSpecial(nextAnimationInput, commands);
                
            case "attacking.water.special.preparehold":
                switch (nextAnimationInput)
                {
                    case PlayerInputAnimation.QuickSpecial:
                    case PlayerInputAnimation.HeavySpecial:
                        return PrepareAttack(convertToAttack(nextAnimationInput));
                }
                break;

            case "gettingup.faster":
                activateState = "activateRunning";
                break;

            case "gettingup.slower":
                activateState = "activateChilling";
                break;

            case "takingdamage":
                activateState = TakingDamage(nextAnimationInput, commands);
                break;

            case "notifications.water.enter":
            case "notifications.water.stay":
            case "notifications.water.exit":
                break;

            case "kneeling.goingdown":
            case "kneeling.gettingup":
                break;


            case "skill.death.fase.1":
            case "skill.death.fase.2":
            case "skill.death.glide":
            case "skill.death.return":
                activateState = Gliding(nextAnimationInput, commands);
                break;

            case "dying":
                Dying();
                break;

            default:
                break;

        }

        nextAnimationInput = PlayerInputAnimation.Nothing;
        commands.Clear();
        newCommandExist = false;


        return activateState;
    }

    protected virtual void LateUpdate()
    {
        if (rigidBody.velocity.y > maxSpeed)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxSpeed);
        }
        else if (rigidBody.velocity.y < -maxSpeed)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, -maxSpeed);
        }

        if (rigidBody.velocity.x > maxSpeed)
        {
            rigidBody.velocity = new Vector2(maxSpeed, rigidBody.velocity.y);
        }
        else if (rigidBody.velocity.x < -maxSpeed)
        {
            rigidBody.velocity = new Vector2(-maxSpeed, rigidBody.velocity.y);
        }

        //print(((rigidBody.velocity);
    }

    private string Gliding(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {
        string activateState = null;

        foreach (PlayerInputMovement co in commands)
        {
            int reflectAngle = 180;
            switch (co)
            {
                case PlayerInputMovement.Right:
                    reflectAngle = 0;
                    goto case PlayerInputMovement.Left;

                case PlayerInputMovement.Left:
                    transform.eulerAngles = new Vector2(0, reflectAngle);
                    AchieveMaxSpeed(speedRunning*0.8f);
                    break;

            }
        }

        if (ButtonManager.GetUp(ButtonManager.ButtonID.R1,this))
        {
            activateState = "activateChilling";
        }

        float vX = Mathf.Abs(rigidBody.velocity.x)*0.6f;

        if (rigidBody.velocity.y > vX)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, vX);

        if (rigidBody.velocity.y < -vX)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, -vX);


        if (!CheckMPAvailable(mpChannelCost * 1.1f) || uiManager.getSelectedWeapon().toElement() != Commandments.Element.DEATH)
        {
            return "activateChilling";
        }

        ChangeMP(-mpChannelCost * 1.1f);
        //ChangeMP(specials[DEATH].channelCost);

        return activateState;
    }
    
    protected override void ChangeStateEvent(string currentState, string nextState)
    {
        ApplyOnce.remove("OnceInState", gameObject);
        ApplyOnce.remove("Notification", gameObject);

        isDownJumping = false;

        if (nextState.ToLower().Contains("attacking"))
        {
            animator.speed *= Commandments.Modifiers.IncreaseAttackSpeed.GetValue();
        }
        if (currentState.ToLower().Contains("attacking"))
        {
            animator.speed /= Commandments.Modifiers.IncreaseAttackSpeed.GetValue();
        }

        if (nextState == "attacking.water.special.hold") {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Fire"));
        }
        if (currentState == "attacking.water.special.hold")
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Fire"),false);
        }
        if (nextState == "chilling")
        {
            lastInput = Time.time;
        }

        if (currentState == "chilling")
        {
            //print((("Change State");
        }


        if (nextState == "attacking.special")
        {
            //Instantiate(SpecialAttackObject, transform.position, transform.rotation);
        }

        if (nextState == "jumping.sliding")
        {
            //if (CheckActiveAbility(Abilities.SelectableAbility.WALL_GRAB))
            //{
            //    rigidBody.gravityScale = 0;
            //    rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            //}
            //else
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y*0.2f);
            wallDirection = (((int)transform.eulerAngles.y) / -90) + 1;
        }

        if (currentState == "jumping.sliding")
        {
            if (ApplyOnce.alreadyApplied("WallJump",gameObject))
            {
                //rigidBody.velocity = Vector2.zero;
                //rigidBody.gravityScale = maxGravity;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180 - transform.eulerAngles.y, transform.eulerAngles.z);
                ApplyOnce.remove("WallJump", gameObject);

            }
        }

            if (nextState == "jumping.rising")
        {
            ApplyOnce.remove("Jump", gameObject);
        }

        if (nextState == "jumping.double")
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        }

        if (nextState == "pullingup")
        {
            
            rigidBody.velocity = Vector2.zero;
            rigidBody.gravityScale = 0;
        }

        if (currentState.Contains("pullingup"))
        {
            //print((("exit");
            rigidBody.gravityScale = maxGravity;
            ApplyOnce.remove("changeDestiny", gameObject);
        }

        if (nextState == "jumping.landingheavy")
        {
            hp -= fallDamage;
            uiManager.ChangeHP(hp/maxHP);
        }

        if (currentState.Contains("climbing")) {
            toClimb = false;
        }

        if (nextState.Contains("climbing"))
        {
            //print((("enter");
            rigidBody.gravityScale = 0;
            rigidBody.velocity = new Vector2(0,0);

            if (nextState == "climbing.preparing") {
                if (currentLadder != null)
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, (-90 * currentLadder.transform.localScale.x) + 90, transform.eulerAngles.z);
            }

            if (nextState == "climbing.downprep")
            {
                if (currentLadder != null)
                {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, (-90 * currentLadder.transform.localScale.x) + 90, transform.eulerAngles.z);
                    Vector2 mySize = GetComponentInChildren<BoxCollider2D>().size * sprite.transform.localScale.y;
                    float direction = (transform.eulerAngles.y / -90) + 1;
                    destiny = new Vector2(currentLadder.transform.position.x - direction * mySize.x * 0.6f, transform.position.y - mySize.y);
                }
            }

        }
        else if (currentState.Contains("climbing"))
        {
            rigidBody.gravityScale = maxGravity;
            currentLadder = null;
        }

        if (!nextState.Contains("attacking") && currentState.Contains("attacking"))
        {
            animator.ResetTrigger("triggerFinishAttack");
        }

        if (currentState.Contains("attacking"))
        {
            //Utilities.//print((("Set CD");
            lastAttack = Time.time;
        }

        if (currentState.Contains("special"))
        {
            //Utilities.//print((("Set CD");
            lastSpecial = Time.time;
        }

        //if (nextState.Contains("attacking") && !isOnGround)
        //{
        //    rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        //}

        if (currentState.ToLower().Contains("gettingup")) {
            animator.SetInteger("gettingUpType", 0);
        }

        if (nextState.ToLower() == "rolling")
        {
            //print((("next = rolling");
            if (currentState.ToLower().Contains("falling"))
            {
                //print((("Contains");
                ApplyOnce.apply("LandingSoft", gameObject, () => { return true; });
            }
            CoolDownManager.Apply(Commandments.Modifiers.Invulnerability.ToString(), gameObject, invulnerabilityTime, () => { return true; });    
        }

        if (currentState.ToLower() == "rolling")
        {
            CoolDownManager.Remove(Commandments.Modifiers.Invulnerability.ToString(), gameObject);

            foreach (Collider2D collider in ignoredCollisions)
            {
                Physics2D.IgnoreCollision(myBounds, collider, false);
            }
            ignoredCollisions.Clear();

            //print((("remove");
            ApplyOnce.remove("LandingSoft", gameObject);


        }

        if( (currentState.ToLower() == "takingdamage") || (currentState.ToLower().Contains("landingheavy"))) {
            ApplyOnce.remove("ReceiveDamage", gameObject);
        }

        if (nextState.ToLower() == "skill_death_fase_1")
        {
            rigidBody.gravityScale *= 0.1f;
            rigidBody.velocity = rigidBody.velocity * 0.3f;
            speedRunning *= 0.7f;
        }

        if (currentState.ToLower() == "skill_death_return")
        {
            rigidBody.gravityScale *= 10;
            speedRunning /= 0.7f;
        }

        if (nextState.ToLower() == "dying")
        {
            rigidBody.isKinematic = true;
        }

        if (currentState.ToLower() == "dying")
        {
            rigidBody.isKinematic = false;
        }

        if (nextState.ToLower().Contains("landingheavy"))
        {
            ApplyOnce.apply("ReceiveDamage", gameObject, () => { return true;});
            ReceiveDamage(Vector2.zero,0, 10f*Commandments.Modifiers.ReduceFallingDamage.GetValue());
        }

    }

    #endregion

    #region Analisys Buttons

    private PlayerInputAnimation CheckPlayerInputAnimation( PlayerInputAnimation lastInput = PlayerInputAnimation.Nothing)
    {

        if (ButtonManager.GetDown(ButtonManager.ButtonID.R1,this))
        {
            if (CheckSkillAvailable())
                return PlayerInputAnimation.Skill;
        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.R2, this))
        {
            return (PlayerInputAnimation.Rolling);
        }

        if (ButtonManager.Get(ButtonManager.ButtonID.L2))
        {
            if (ButtonManager.GetDown(ButtonManager.ButtonID.X, this))
            {
                return PlayerInputAnimation.Ability_X;
            }
            if (ButtonManager.GetDown(ButtonManager.ButtonID.CIRCLE, this))
            {
                return PlayerInputAnimation.Ability_Circle;
            }
            if (ButtonManager.GetDown(ButtonManager.ButtonID.TRIANGLE, this))
            {
                return PlayerInputAnimation.Ability_Triangle;
            }
            if (ButtonManager.GetDown(ButtonManager.ButtonID.SQUARE, this))
            {
                return PlayerInputAnimation.Ability_Square;
            }
        }


        if (ButtonManager.GetDown(ButtonManager.ButtonID.X, this))
        {

            if (currentFloor != null)
            {
                if (ButtonManager.GetDown(ButtonManager.ButtonID.L_DOWN,this) && currentFloor.GetComponent<Floor>().isPassable)
                {
                    return (PlayerInputAnimation.DownJump);
                }
            }
            if (isDownJumping == false)
                return (PlayerInputAnimation.JumpShort);
        }
        

        switch (ButtonManager.GetState(ButtonManager.ButtonID.SQUARE,this))
        {

            case SingleButtonManager.State.Short:
                {
                    //print("Squa short");
                    return (PlayerInputAnimation.QuickAttack);
                }

            case SingleButtonManager.State.Long:
                {
                    //print("Squa long");
                    return (PlayerInputAnimation.HeavyAttack);
                }
            case SingleButtonManager.State.Hold:
                {
                    //print("Squa hold");
                    return (PlayerInputAnimation.HoldingAttack);
                }
        }

        switch (ButtonManager.GetState(ButtonManager.ButtonID.TRIANGLE, this))
        {

            case SingleButtonManager.State.Short:
                //print((("Tri Short");
                return (PlayerInputAnimation.QuickSpecial);

            case SingleButtonManager.State.Long:
                //print((("Tri long");
                return (PlayerInputAnimation.HeavySpecial);

            case SingleButtonManager.State.Hold:
                //print((("Tri hold");
                return (PlayerInputAnimation.HoldingSpecial);
        }
        
        if (ButtonManager.GetDown(ButtonManager.ButtonID.SELECT, this))
        {
            //LoadManager.getInstance().Reload();
            ReceiveModifier(Commandments.Modifiers.Poison, 8);
        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.R3, this))
        {
            //print((("2Buttons 1Press");
            //return PlayerInputAnimation.Ultimate
        }

        return lastInput;
    }

    private void CheckPlayerInputMovement(List<PlayerInputMovement> buttons)
    {

        if (ButtonManager.Get(ButtonManager.ButtonID.L_UP))
        {
            buttons.Add(PlayerInputMovement.Up);
        }
        else if (ButtonManager.Get(ButtonManager.ButtonID.L_DOWN))
        {
            buttons.Add(PlayerInputMovement.Down);
        }
        if (ButtonManager.Get(ButtonManager.ButtonID.L_RIGHT))
        {
            buttons.Add(PlayerInputMovement.Right);
        }
        else if (ButtonManager.Get(ButtonManager.ButtonID.L_LEFT))
        {
            buttons.Add(PlayerInputMovement.Left);
        }

        
    }

    #endregion

    #region Behavior Functions

    private string Chilling(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {
        if (currentLadder != null)
            
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, (-90 * currentLadder.transform.localScale.x) + 90, transform.eulerAngles.z);

        if (Time.time - lastInput > sittingTimer)
        {
            animator.SetInteger("weapon", (int)uiManager.getSelectedWeapon());
            return "activateSitting";
        }


        switch (nextAnimationInput)
        {
            /*rolling*/

            case PlayerInputAnimation.HeavySpecial:
                if (!CheckMPAvailable(2 * mpCost))
                    return null;
                goto case PlayerInputAnimation.QuickSpecial;
            case PlayerInputAnimation.QuickSpecial:
            case PlayerInputAnimation.HoldingSpecial:
                if (CheckMPAvailable(mpCost))
                {
                    if (Time.time - lastSpecial > specialCD)
                        return PrepareAttack(convertToAttack(nextAnimationInput));
                }
                break;
            case PlayerInputAnimation.QuickAttack:
            case PlayerInputAnimation.HeavyAttack:
            case PlayerInputAnimation.HoldingAttack:
                if(Time.time-lastAttack > attackCD)
                    return PrepareAttack(convertToAttack( nextAnimationInput));
                break;


            case PlayerInputAnimation.Rolling:
                animator.SetTrigger("triggerRolling");
                break;

            case PlayerInputAnimation.DownJump:
                if (CheckDownJump())
                {
                    return "activateChilling";
                }
                break;

            case PlayerInputAnimation.JumpShort:
                ApplyOnce.apply("Jump", gameObject, () =>
                {
                    animator.SetTrigger("triggerJumping");
                    return true;
                });
                break;

            case PlayerInputAnimation.Ability_X:
            case PlayerInputAnimation.Ability_Circle:
            case PlayerInputAnimation.Ability_Triangle:
            case PlayerInputAnimation.Ability_Square:
                {
                    ActivateAbility(nextAnimationInput);
                }
                break;
            default:

                foreach (PlayerInputMovement co in commands)
                {
                    switch (co)
                    {
                        case PlayerInputMovement.Left:
                        case PlayerInputMovement.Right:
                            return "activateRunning";

                        case PlayerInputMovement.Down:
                            if (currentLadder != null)
                            {
                                if (currentLadder.transform.position.y < transform.position.y)
                                    return "activateGoingDown";
                            }
                            break;
                        case PlayerInputMovement.Up:
                            if (CheckLadder())
                            {
                                    return "activateClimbing";   
                            }
                            if (CheckPullUp())
                            {
                                return "activatePullingUp";
                            }
                            break;

                    }

                }
                break;
        }

        return "activateChilling";
    }

    private bool CheckLadder()
    {

        if (currentLadder == null)
            return false;



        Vector2 ladderSize = currentLadder.GetComponent<BoxCollider2D>().size;
        Vector2 scale = currentLadder.transform.lossyScale;
        if (currentLadder.transform.position.y + ladderSize.y * 0.5f * scale.y * 0.9f > transform.position.y)
        {
            rigidBody.velocity = Vector2.zero;
            return true;
        }

        return false;
    }

    private string Running(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {
        string nextState = "activateChilling";
        foreach (PlayerInputMovement co in commands)
        {
            int reflectAngle = 180;
            switch (co)
            {
                case PlayerInputMovement.Right:
                    reflectAngle = 0;
                    goto case PlayerInputMovement.Left;

                case PlayerInputMovement.Left:
                    transform.eulerAngles = new Vector2(0, reflectAngle);
                    AchieveMaxSpeed(speedRunning);
                    if(nextState == "activateChilling")
                        nextState = "activateRunning";
                    break;

                case PlayerInputMovement.Down:
                    if (currentLadder != null)
                    {
                        if (currentLadder.transform.position.y < transform.position.y)
                            return "activateGoingDown";
                    }
                    break;

                case PlayerInputMovement.Up:
                    if (CheckLadder())
                    {
                            return "activateClimbing";
                    }
                    break;

            }

        }


        switch (nextAnimationInput)
        {
            case PlayerInputAnimation.QuickAttack:
            case PlayerInputAnimation.HeavyAttack:
            case PlayerInputAnimation.HoldingAttack:
                if (Time.time - lastAttack > attackCD)
                    return PrepareAttack(convertToAttack( nextAnimationInput));
                break;

            /*rolling*/
            case PlayerInputAnimation.Rolling:
                animator.SetTrigger("triggerRolling");
                break;

            case PlayerInputAnimation.DownJump:
                if (CheckDownJump())
                {
                    return "activateChilling";
                }
                break;

            case PlayerInputAnimation.JumpShort:
                ApplyOnce.apply("Jump", gameObject, () =>
                {
                    animator.SetTrigger("triggerJumping");
                    return true;
                });
                break;

            case PlayerInputAnimation.Ability_X:
            case PlayerInputAnimation.Ability_Circle:
            case PlayerInputAnimation.Ability_Triangle:
            case PlayerInputAnimation.Ability_Square:
                ActivateAbility(nextAnimationInput);
                break;
            case PlayerInputAnimation.QuickSpecial:
                break;

        }

        return nextState;
    }

    private string Jumping(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {
        string nextState = "activateChilling";
        foreach (PlayerInputMovement co in commands)
        {
            int reflectAngle = 180;
            switch (co)
            {
                case PlayerInputMovement.Right:
                    reflectAngle = 0;
                    goto case PlayerInputMovement.Left;

                case PlayerInputMovement.Left:
                    transform.eulerAngles = new Vector2(0, reflectAngle);
                    AchieveMaxSpeed(speedRunning);
                    nextState = "activateRunning";
                    break;

                case PlayerInputMovement.Down:
                    if (currentLadder != null)
                    {
                        if (currentLadder.transform.position.y < transform.position.y)
                            return "activateGoingDown";
                    }
                    break;

                case PlayerInputMovement.Up:
                    if (CheckLadder())
                    {
                            return "activateClimbing";
                    }
                    
                    if (currentFloor == null)
                        break;
 

                    if (CheckPullUp())
                    {
                        return "activatePullingUp";
                    }

                    
                    break;

            }

        }

        if (verticalSpeed <= 0 && isOnWall && !isOnGround)
        {
            bool iAmOnTheRight = transform.position.x > currentWall.transform.position.x;
            if (commands.Contains(PlayerInputMovement.Right) && !iAmOnTheRight)
            {
                return "activateSliding";
            }
            else if (commands.Contains(PlayerInputMovement.Left) && iAmOnTheRight)
            {
                return "activateSliding";
            }
        }

        switch (nextAnimationInput)
        {
            case PlayerInputAnimation.QuickAttack:
            case PlayerInputAnimation.HeavyAttack:
            case PlayerInputAnimation.HoldingAttack:
                if (Time.time - lastAttack > 2 * attackCD)
                {
                    animator.SetInteger("gettingUpType", 1);
                    return PrepareAttack(AttackType.Air);
                }
                break;

            case PlayerInputAnimation.JumpShort:               
                break;

            case PlayerInputAnimation.QuickSpecial:
                break;
            case PlayerInputAnimation.Ability_X:
            case PlayerInputAnimation.Ability_Circle:
            case PlayerInputAnimation.Ability_Triangle:
            case PlayerInputAnimation.Ability_Square:
                ActivateAbility(nextAnimationInput);
                break;
            case PlayerInputAnimation.Skill:
                if (uiManager.getSelectedWeapon().toElement() == Commandments.Element.FIRE)
                {
                    ApplyOnce.apply("DoubleJumpActivated", gameObject, () =>
                    {
                        if (CheckSkillAvailable())
                        {
                            if (!isOnGround)
                            {
                                animator.SetTrigger("triggerDoubleJump");
                                return true;
                            }
                        }
                        return false;
                    });
                }
                else
                    return "activateSkill";
                break;
               
        }

        return nextState;
    }
    
    private bool CheckSkillAvailable()
    {
        switch (uiManager.getSelectedWeapon().toElement())
        {
            case Commandments.Element.DEATH:
                return CheckDeathAvailable();
            case Commandments.Element.WATER:
                return CheckWaterAvailable();
            case Commandments.Element.FIRE:
                return CheckFireAvailable();
        }

        return false;
    }

    private bool CheckDeathAvailable()
    {
        //print("CheckDeath - currentState = "+ currentState);
        switch (currentState.ToLower().Replace("_", "."))
        {

            /*case "chilling":
                activateState = Chilling(nextAnimationInput, commands);
                break;

            case "running":
                activateState = Running(nextAnimationInput, commands);
                break;*/

            /*case "takingdamage":
                activateState = TakingDamage(nextAnimationInput, commands);
                break;*/

            case "jumping.rising":
            case "jumping.falling":
            case "jumping.double":
            case "jumping.sliding":
                if (!CheckMPAvailable(mpChannelCost * 1.1f))
                {
                    return false;
                }
                if (!uiManager.progress.Active(Progress.GameItems.DEATH_ITEM))
                {
                    return false;
                }

                return true;
            default:
                break;

        }

        return false;

    }

    private bool CheckWaterAvailable()
    {
        if (!uiManager.progress.Active(Progress.GameItems.WATER_ITEM))
        {
            return false;
        }
        return false;
    }

    private bool CheckFireAvailable()
    {
        if (!uiManager.progress.Active(Progress.GameItems.FIRE_ITEM))
        {
            return false;
        }
        switch (currentState.ToLower().Replace("_", "."))
        {
            case "jumping.rising":
            case "jumping.falling":
                return true;
        }
        return false;
    }


    private bool CheckPullUp()
    {

        if (currentFloor == null)
            return false;

        Vector2 leftPoint, rightPoint;
        leftPoint = rightPoint = Vector2.zero;


        mySize = GetComponentInChildren<BoxCollider2D>().size * sprite.transform.localScale.y;

        if (currentFloor.GetComponent<BoxCollider2D>()!= null && currentFloor.GetComponent<BoxCollider2D>().isTrigger == false)
        {
            Vector2 floorSize = currentFloor.GetComponent<BoxCollider2D>().size;

            float rotation = currentFloor.transform.rotation.eulerAngles.z;

            Vector2 floorScale = new Vector2(currentFloor.transform.localScale.x * currentFloor.transform.parent.transform.localScale.x, currentFloor.transform.localScale.y * currentFloor.transform.parent.transform.localScale.y);


            leftPoint.y = currentFloor.transform.position.y + Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * floorScale.y - Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * floorScale.x;
            leftPoint.x = currentFloor.transform.position.x - Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * floorScale.y - Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * floorScale.x;

            rightPoint.y = currentFloor.transform.position.y + Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * floorScale.y + Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * floorScale.x;
            rightPoint.x = currentFloor.transform.position.x - Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * floorScale.y + Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * floorScale.x;
        }
        else if (currentFloor.GetComponent<EdgeCollider2D>() != null && currentFloor.GetComponent<EdgeCollider2D>().isTrigger == false)
        {
            Vector2[] points = currentFloor.GetComponent<EdgeCollider2D>().points;
            Vector2 offSet = currentFloor.GetComponent<EdgeCollider2D>().offset;

            leftPoint = Vector2.Scale((points[0] + offSet), currentFloor.transform.lossyScale) + (Vector2)currentFloor.transform.position;
            rightPoint = Vector2.Scale((points[1] + offSet), currentFloor.transform.lossyScale) + (Vector2)currentFloor.transform.position;


            if (leftPoint.x > rightPoint.x)
            {
                Vector2 aux = leftPoint;
                leftPoint = rightPoint;
                rightPoint = aux;
            }

            int i = 1;
            while (transform.position.x > rightPoint.x)
            {
                i++;
                if (i >= points.Length)
                {
                    return false;
                }

                Vector2 auxPoint = Vector2.Scale((points[i] + offSet), currentFloor.transform.lossyScale) + (Vector2)currentFloor.transform.position;

                if (auxPoint.x > rightPoint.x)
                {
                    leftPoint = rightPoint;
                    rightPoint = auxPoint;
                }
                else
                {
                    rightPoint = leftPoint;
                    leftPoint = auxPoint;
                }

            }
            
        }
        else
            return false;


        destinyDirection = rightPoint - leftPoint;
        destinyDirection.Normalize();


        if (transform.position.x + mySize.x / 2 - grabLimit < leftPoint.x)
        {
            if (transform.position.y + mySize.y * 0.90f < leftPoint.y)
            {
                rigidBody.velocity = Vector2.zero;
                destiny = new Vector2(leftPoint.x + 0.5f, leftPoint.y + 0.005f);

                if (destinyDirection.x + destiny.x > rightPoint.x)
                    destinyDirection = Vector2.zero;

                TurnCharacter(Direction.Right);

                if (transform.parent != null)
                {
                    tempParent = transform.parent.gameObject;
                    destiny -= (Vector2)transform.parent.transform.position;
                }
                else
                    tempParent = null;

                return true;
            }
        }
        else if (transform.position.x - mySize.x / 2 + grabLimit > rightPoint.x)
        {
            if (transform.position.y + mySize.y * 0.90f < rightPoint.y)
            {
                rigidBody.velocity = Vector2.zero;
                destiny = new Vector2(rightPoint.x - 0.5f, rightPoint.y + 0.005f);

                if (destiny.x - destinyDirection.x < leftPoint.x)
                    destinyDirection = Vector2.zero;

                TurnCharacter(Direction.Left);

                if (transform.parent != null)
                {
                    tempParent = transform.parent.gameObject;
                    destiny -= (Vector2)transform.parent.transform.position;
                }
                else
                    tempParent = null;

                return true;
            }

        }
        else
        { 
            //To na meiuca
            if (currentFloor.GetComponent<Floor>().isPassable)
            {

                if (transform.position.y + mySize.y * 0.90f < leftPoint.y + (transform.position.x - leftPoint.x) * destinyDirection.y / destinyDirection.x)
                {

                    destiny.y = leftPoint.y + (transform.position.x - leftPoint.x) * destinyDirection.y / destinyDirection.x;
                    destiny.y += 0.005f;

                    destiny.x = transform.position.x;


                    if ((destiny + getDirection() * destinyDirection * 1.5f).x < leftPoint.x)
                        TurnCharacter(Direction.Right);
                    else if ((destiny + getDirection() * destinyDirection * 1.5f).x > rightPoint.x)
                        TurnCharacter(Direction.Left);

                    destiny += getDirection() * destinyDirection * 0.5f;

                    if ((destiny.x - destinyDirection.x < leftPoint.x) || (destinyDirection.x + destiny.x > rightPoint.x))
                        destinyDirection = Vector2.zero;

                    rigidBody.velocity = Vector2.zero;

                    if (transform.parent != null)
                    {
                        tempParent = transform.parent.gameObject;
                        destiny -= (Vector2)transform.parent.transform.position;
                    }
                    else
                        tempParent = null;

                    return true;
                }
            }

        }

        return false;
    }

    private string JumpingSliding(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {

        //if (!CheckActiveAbility(Abilities.SelectableAbility.WALL_GRAB))
        //{
        //    if (Mathf.Abs(rigidBody.velocity.y) >= speedRunning)
        //    {
        //        rigidBody.velocity = new Vector2(rigidBody.velocity.x, -speedRunning);
        //    }
        //}
        //else
        {
            if (rigidBody.velocity.y>0 && !ApplyOnce.alreadyApplied("Jump", gameObject))
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            }
        }


        foreach (PlayerInputMovement co in commands)
        {
            int direction = 1;
            switch (co)
            {
                case PlayerInputMovement.Left:
                    direction *= -1;
                    goto case PlayerInputMovement.Right;

                case PlayerInputMovement.Right:
                    if (direction != wallDirection)
                    {
                        {

                            float power;
                            Vector2 vector;

                            power = jumpForce / 10;
                            vector = new Vector2(direction / 9.8f, 0);
                            //print((("5");
                            rigidBody.AddForce(vector * power);
                        }
                    }
                    else 
                    {
                        //if (!CheckActiveAbility(Abilities.SelectableAbility.WALL_GRAB))
                        {
                            //print((("!");
                            rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y / 2);
                        }
                        //rigidBody.AddForce(Vector2.right * wallDirection * 10);
                    }
                    break;
                //case PlayerInputMovement.Down:
                //    if (CheckActiveAbility(Abilities.SelectableAbility.WALL_GRAB))
                //    {
                //        transform.position = new Vector3(transform.position.x, transform.position.y - 0.075f, transform.position.z);
                //    }
                //    break;
                case PlayerInputMovement.Up:
                    if (currentFloor != null)
                    {
                        if (CheckPullUp())
                        {
                            return "activatePullingUp";
                        }
                    }
                    break;


            }

        }
        switch (nextAnimationInput)
        {

            case PlayerInputAnimation.JumpShort:
                ApplyOnce.apply("WallJump", gameObject, () =>
                {
                    if(CheckModifier(Commandments.Modifiers.WallJump))
                    {
                        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

                        rigidBody.AddForce(new Vector2(-wallDirection, 0.75f) * jumpForce * Commandments.Modifiers.IncreaseJumpHeight.GetValue());
                        return true;
                    }
                    return false;
                });
                if (ApplyOnce.alreadyApplied("WallJump", gameObject))
                {
                    return "activateRising";
                }
                break;
        }


        if (isOnWall)
            return "activateSliding";
        else
            return null;
    }

    private string JumpingPreparing(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            //rigidBody.velocity = new Vector2(0, 0);
            ApplyOnce.remove("DoubleJumpForce", gameObject);
        }
        else
        {
            ApplyOnce.apply("OnceInState", gameObject, () => { 
                rigidBody.AddForce(Utilities.standardVector(getDirection(), jumpAngle) * jumpForce * Commandments.Modifiers.IncreaseJumpHeight.GetValue());
                return true; 
            });

            foreach (PlayerInputMovement co in commands)
            {
                int reflectAngle = 180;
                switch (co)
                {
                    case PlayerInputMovement.Right:
                        reflectAngle = 0;
                        goto case PlayerInputMovement.Left;

                    case PlayerInputMovement.Left:
                        transform.eulerAngles = new Vector2(0, reflectAngle);
                        AchieveMaxSpeed(speedRunning);
                        break;

                }

            }
        }

        return "activateChilling";

    }

    private string JumpingDouble(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.2f) {
            rigidBody.velocity = new Vector2(0, 0);
            ApplyOnce.remove("DoubleJumpForce", gameObject);
        }
        else
        {
            ApplyOnce.apply("DoubleJumpForce", gameObject, () =>
            {
                rigidBody.AddForce(new Vector2(0, 1) * doubleJumpForce);
                return true;
            });

            foreach (PlayerInputMovement co in commands)
            {
                int reflectAngle = 180;
                switch (co)
                {
                    case PlayerInputMovement.Right:
                        reflectAngle = 0;
                        goto case PlayerInputMovement.Left;

                    case PlayerInputMovement.Left:
                        transform.eulerAngles = new Vector2(0, reflectAngle);
                        AchieveMaxSpeed(speedRunning);
                        break;

                }

            }
        }

        return "activateChilling";
    }

    private string JumpingLandingHeavy(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {
        string nextState = "activateChilling";
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
        {
            rigidBody.velocity = new Vector2(0, 0);
        }
        else
        {
            foreach (PlayerInputMovement co in commands)
            {
                int reflectAngle = 180;
                switch (co)
                {
                    case PlayerInputMovement.Right:
                        reflectAngle = 0;
                        goto case PlayerInputMovement.Left;

                    case PlayerInputMovement.Left:
                        transform.eulerAngles = new Vector2(0, reflectAngle);
                        AchieveMaxSpeed(speedRunning);
                        nextState = "activateRunning";
                        break;

                }

            }
        }
        return nextState;
    }

    private string JumpingLandingSoft(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {

        string nextState = "activateChilling";
        foreach (PlayerInputMovement co in commands)
        {
            int reflectAngle = 180;
            switch (co)
            {
                case PlayerInputMovement.Right:
                    reflectAngle = 0;
                    goto case PlayerInputMovement.Left;

                case PlayerInputMovement.Left:
                    transform.eulerAngles = new Vector2(0, reflectAngle);
                    AchieveMaxSpeed(speedRunning);
                    if (nextState == "activateChilling")
                        nextState = "activateRunning";
                    break;

            }

        }

        switch (nextAnimationInput)
        {
            case PlayerInputAnimation.Rolling:
                animator.SetTrigger("triggerRolling");
                break;
        }
        return "activateChilling";
        
    }

    private string Climbing(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {
         string nextState = null;

         rigidBody.velocity = Vector2.zero;

         if (toClimb)
             nextState = "activateMoving";

        if (currentLadder)
            LookTo(currentLadder);
        else
        {
            return "activateChilling";
        }

        foreach (PlayerInputMovement co in commands)
        {
            int reflectAngle = 180;
            switch (co)
            {
                case PlayerInputMovement.Right:
                    reflectAngle = 0;
                    goto case PlayerInputMovement.Left;

                case PlayerInputMovement.Left:
                    
                    if ((transform.eulerAngles.y != reflectAngle) && (Mathf.Abs(ButtonManager.GetValue(ButtonManager.ButtonID.L_LEFT,ButtonManager.ButtonID.L_RIGHT)) > 0.9f))
                    {
                        /*drop*/

                        rigidBody.AddForce(new Vector2((transform.eulerAngles.y / 180) - 1, 0) * jumpForce/10 );
                        return "activateChilling";
                    }
                    break;

                case PlayerInputMovement.Up:
                    if (currentLadder != null)
                    {
                        Vector2 mySize = GetComponentInChildren<BoxCollider2D>().size * sprite.transform.lossyScale.y;
                        Vector2 ladderSize = currentLadder.GetComponent<BoxCollider2D>().size;
                        Vector2 scale = new Vector2(currentLadder.transform.lossyScale.x, currentLadder.transform.lossyScale.y);
                        float topPoint;

                        topPoint = currentLadder.transform.position.y;

                        //UIManager.GlobalGreenBall.transform.position = new Vector2(currentLadder.transform.position.x,topPoint);

                        if (transform.position.y + mySize.y*0.9f > topPoint)
                        {
                            destiny = new Vector2(transform.position.x + getDirection(), topPoint + 0.025f);
                            destinyDirection = Vector2.zero;

                            nextState =  "activatePullingUp";
                        }
                        else
                            nextState = "activateMoving";

                        toClimb = true;
                        //if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime<1)
                        transform.position = new Vector3(transform.position.x, transform.position.y + 0.025f, transform.position.z);

                    }
                    break;

                case PlayerInputMovement.Down:
                     if (currentLadder != null)
                    {
                        nextState = "activateMoving";
                        transform.position = new Vector3(transform.position.x, transform.position.y - 0.025f, transform.position.z);
                        if (isOnGround)
                            nextState = "activateChilling";
                    }
                    break;
            }

        }

        switch (nextAnimationInput)
        {
            case PlayerInputAnimation.QuickAttack:
            case PlayerInputAnimation.HeavyAttack:
            case PlayerInputAnimation.HoldingAttack:
                if (Time.time - lastAttack > attackCD)
                    return PrepareAttack(convertToAttack( nextAnimationInput));
                break;

            case PlayerInputAnimation.JumpShort:

                ApplyOnce.apply("Jump", gameObject, () =>
                {
                    animator.SetTrigger("triggerJumping");
                    return true;
                });
                break;
        }
        return nextState;
    }

    private string ClimbingPreparing(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {
        Vector3 currentPosition = transform.position;
        Vector2 mySize = GetComponentInChildren<BoxCollider2D>().size * sprite.transform.localScale.y;
        float direction = (transform.eulerAngles.y / -90) +1;
        float time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        rigidBody.velocity = Vector2.zero;


        if(currentLadder != null)
            currentPosition = Vector3.Lerp(transform.position, new Vector3(currentLadder.transform.position.x-direction*mySize.x*1.5f, transform.position.y, transform.position.z), time);
        
        transform.position = currentPosition;
        return "activateMoving";
    }

    private string ClimbingPreparingDown(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {
        Vector3 currentPosition = transform.position;

        float time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (time > 5.0f/7.0f)
        {
            currentPosition = Vector3.Lerp(transform.position, new Vector3(destiny.x, destiny.y, transform.position.z), (time - 5.0f / 7.0f) * 7.0f / 2.0f);
        }
        else if (time > 4.0f / 7.0f)
        {
            currentPosition = Vector3.Lerp(transform.position, new Vector3(destiny.x, transform.position.y + (destiny.y - transform.position.y) * 0.2f, transform.position.z), (time - 4.0f / 7.0f) * 7.0f ); ;
        }
        else if (time > 3.0f / 7.0f)
        {
            currentPosition = Vector3.Lerp(transform.position, new Vector3(destiny.x, transform.position.y, transform.position.z), (time - 3.0f / 7.0f) * 7.0f); ;
        }



        transform.position = currentPosition;
        return "activateMoving";

    }

    private string PullingUp(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {
        rigidBody.velocity = Vector2.zero;

        Vector2 auxDest = destiny;
        if (tempParent != null)
            auxDest += (Vector2)tempParent.transform.position;


        Vector2 currentPosition = transform.position;
        float time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (time < 0.33f)
        {
            currentPosition = Vector2.Lerp(transform.position, new Vector2(transform.position.x+(auxDest.x-transform.position.x)/11, auxDest.y), time * 3);
        }
        else 
        {
            ApplyOnce.apply("changeDestiny", gameObject, () => {

                
                destiny += getDirection() * destinyDirection;

                return true;
            });


            currentPosition = Vector2.Lerp(transform.position, auxDest, (time-0.33f) * 2/3);
        }
        transform.position = new Vector3( currentPosition.x, currentPosition.y, transform.position.z);
        return "activateRunning";
    }

    private string Rolling(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {

        int factor = 1; ;

        if (ApplyOnce.alreadyApplied("LandingSoft", gameObject))
        {
            if (isOnLedge)
            {
                factor = -1;
            }
        }

        if (rigidBody.velocity.x > 0 && commands.Contains(PlayerInputMovement.Left))
        {
            AchieveMaxSpeed(factor*speedRolling / 2);
        }
        else if (rigidBody.velocity.x < 0 && commands.Contains(PlayerInputMovement.Right))
        {
            AchieveMaxSpeed(factor*speedRolling / 2);
        }
        else
            AchieveMaxSpeed(factor*speedRolling);

        switch (nextAnimationInput)
        {
            case PlayerInputAnimation.QuickAttack:
            case PlayerInputAnimation.HeavyAttack:
            case PlayerInputAnimation.HoldingAttack:
                if (Time.time - lastAttack >  attackCD)
                    return PrepareAttack( AttackType.Rolling);
                break;
        }

        return "activateRunning";
    }

    private string Attacking(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands, bool isStatic=false)
    {
        string nextState = "activateChilling";

        foreach (PlayerInputMovement co in commands)
        {
            switch (co)
            {
                case PlayerInputMovement.Right:
                    goto case PlayerInputMovement.Left;

                case PlayerInputMovement.Left:
                    AchieveMaxSpeed(speedRunning / 2);
                    nextState = "activateRunning";
                    break;


            }

        }

        switch (nextAnimationInput)
        {
            case PlayerInputAnimation.QuickAttack:
            case PlayerInputAnimation.HeavyAttack:
                animator.SetTrigger("triggerFinishAttack");
                break;
        }

        if(isStatic == false)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            {
                if (isOnGround)
                {
                    if (isOnLedge)
                    {
                            AchieveMaxSpeed(-speedRunning / 2);
                    }
                    else
                    {
                            AchieveMaxSpeed(speedRunning / 2);
                    }
                }
            }
        }

        return nextState;
    }

    private string AttackingAir(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands, bool isStatic = false)
    {
          string nextState = "activateChilling";

        foreach (PlayerInputMovement co in commands)
        {
            switch (co)
            {
                case PlayerInputMovement.Right:
                    //print("enterRight");
                    AchieveMaxSpeed(getDirection()*speedRunning / 2);
                    nextState = "activateRunning";
                    break;

                case PlayerInputMovement.Left:
                    //print("enterLeft");
                    AchieveMaxSpeed(-1*getDirection()*speedRunning / 2);
                    nextState = "activateRunning";
                    break;


            }

        }

        return nextState;
    }

    private string AttackingBow(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands, int attackType)
    {
        foreach (PlayerInputMovement co in commands)
        {
            int reflectAngle = 180;
            switch (co)
            {

                case PlayerInputMovement.Right:
                    reflectAngle = 0;
                    goto case PlayerInputMovement.Left;

                case PlayerInputMovement.Left:
                    transform.eulerAngles = new Vector2(0, reflectAngle);
                    AchieveMaxSpeed(speedRunning / 2);
                    break;

            }

        }

        switch (nextAnimationInput)
        {
            case PlayerInputAnimation.QuickAttack:
                Weapons.WeaponSettings settings = uiManager.getCurrentWeapon();
                if (settings.normalAttack != null)
                    Instantiate(settings.normalAttack, transform.position, transform.rotation);
                animator.SetTrigger("triggerReleaseAttack");
                break;
        }

        return "activateChilling";
    }

    private string AttackingDeathSpecial(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {
        ApplyOnce.apply("OnceInState", gameObject, () =>
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
            {
                if (uiManager.getCurrentWeapon().specialAttack != null)
                {
                    GameObject attack = (GameObject)Instantiate(uiManager.getCurrentWeapon().specialAttack, transform.position, Quaternion.identity);
                    attack.transform.eulerAngles = transform.eulerAngles;
                    if (CheckModifier(Commandments.Modifiers.IncreaseDamage))
                        attack.GetComponent<DeathSpecial>().multiplier *= Commandments.Modifiers.IncreaseDamage.GetValue();
                    if (CheckModifier(Commandments.Modifiers.GreaterIncreaseDamage))
                        attack.GetComponent<DeathSpecial>().multiplier *= Commandments.Modifiers.GreaterIncreaseDamage.GetValue();

                    if (currentState.Contains("heavy"))
                    {
                        attack.GetComponent<DeathSpecial>().isBig = true;
                        attack.transform.localScale *= 3f;
                        attack.GetComponent<DeathSpecial>().maxSpeed *= 0.75f;
                        ChangeMP(-2 * mpCost);
                        return true;
                    }

                    attack = (GameObject)Instantiate(uiManager.getCurrentWeapon().specialAttack, new Vector2(transform.position.x + 1, transform.position.y + 1), Quaternion.identity);
                    attack.transform.eulerAngles = transform.eulerAngles;
                    
                    if (CheckModifier(Commandments.Modifiers.IncreaseDamage))
                        attack.GetComponent<DeathSpecial>().multiplier *= Commandments.Modifiers.IncreaseDamage.GetValue();
                    if (CheckModifier(Commandments.Modifiers.GreaterIncreaseDamage))
                        attack.GetComponent<DeathSpecial>().multiplier *= Commandments.Modifiers.GreaterIncreaseDamage.GetValue();

                    ChangeMP(-mpCost);
                    return true;
                }
            }

            return false;
        });
        return null;
    }
    
    private string AttackingWaterSpecial(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {

        switch (currentState)
        {

            case "attacking.water.normal.heavyp2":
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
                {
                    ApplyOnce.apply("OnceInState", gameObject, () =>
                    {
                        rigidBody.AddForce(new Vector2(getDirection(), 1.5f) * jumpForce * 0.66f);
                        return true;
                    });

                    if (rigidBody.velocity.y <= 0) {
                        return PrepareAttack(AttackType.Air);
                    }

                }
                break;

            case "attacking.water.special.heavy":
                break;

            case "attacking.water.special.hold":
                switch (nextAnimationInput)
                {
                    case PlayerInputAnimation.QuickSpecial:
                    case PlayerInputAnimation.HeavySpecial:
                        if (CheckMPAvailable(2*mpCost))
                        {
                            return PrepareAttack(convertToAttack(nextAnimationInput));
                        }

                        return "activateChilling";
                }

                foreach (PlayerInputMovement co in commands)
                {
                    int reflectAngle = 180;
                    switch (co)
                    {
                        case PlayerInputMovement.Right:
                            reflectAngle = 0;
                            goto case PlayerInputMovement.Left;

                        case PlayerInputMovement.Left:
                            transform.eulerAngles = new Vector2(0, reflectAngle);
                            break;
                    }
                }

                ChangeMP(-mpChannelCost);
                if (mp <= 0)
                {
                        return "activateChilling";
                }
                if (isOnLedge)
                    AchieveMaxSpeed(-speedRunning / 2);
                else
                    AchieveMaxSpeed(speedRunning/2);

                return null;
        }
           
        return null;
    }

    private bool CheckMPAvailable(float spellValue)
    {
        if (CheckModifier(Commandments.Modifiers.InfiniteMP))
            return true;

        if (mp > spellValue)
        {
            return true;
        }
        return false;
    }

    private string TakingDamage(PlayerInputAnimation nextAnimationInput, List<PlayerInputMovement> commands)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            return null;

        if (CheckModifier(Commandments.Modifiers.Paralysis))
            return null;

        return "activateChilling";
    }

    public override void Dying()
    {
        rigidBody.velocity = Vector2.zero;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            return;

        LoadManager.getInstance().Reload();
    }
    
    #endregion

    #region On Event Functions

    void OnTriggerEnter2D(Collider2D colisor)
    {

        if (colisor.gameObject.tag == "Ladder")
        {
            currentLadder = colisor.gameObject;
            return;
        }


        if (colisor.gameObject.transform.parent == null)
            return;


        if (colisor.gameObject.transform.parent.gameObject.tag == "Enemy" && colisor.gameObject != null)
        {
            /*eu bati*/
            
            Enemy enemy = colisor.gameObject.GetComponentInParent<Enemy>();

            foreach (Collider2D col in colliders)
            {
                if (col.isActiveAndEnabled)
                {

                    if (enemy.ColliderContains(col))
                    {

                        float multiplier = 1;
                        if (CheckModifier(Commandments.Modifiers.IncreaseDamage))
                            multiplier *= Commandments.Modifiers.IncreaseDamage.GetValue();
                        if (CheckModifier(Commandments.Modifiers.GreaterIncreaseDamage))
                            multiplier *= Commandments.Modifiers.GreaterIncreaseDamage.GetValue();


                        float criticalValue = 100 * UnityEngine.Random.value;

                        bool critical = false;
                        float auxCrit = criticalChance;
                        print("criticalChance = " + auxCrit);
                        if (CheckModifier(Commandments.Modifiers.IncreaseCriticalChance))
                            auxCrit *= Commandments.Modifiers.IncreaseCriticalChance.GetValue();
                        print("auxCrit = " + auxCrit);
                        print("criticalValue = " + criticalValue);
                        if (criticalValue < auxCrit)
                        {
                            print("critical = true");
                            critical = true;
                            multiplier *= 2;
                        }

                        float damageDealt = enemy.ReceiveDamage(Utilities.standardVector(colisor.transform.position.x - transform.position.x), damageForce, multiplier * attackDamage, critical,uiManager.getSelectedWeapon().toElement(), this);
                        if (CheckModifier(Commandments.Modifiers.Vampire))
                        {
                            print("Vampire = " + damageDealt * Commandments.Modifiers.Vampire.GetValue());
                            ChangeHP(damageDealt * Commandments.Modifiers.Vampire.GetValue());
                        }

                        return;
                    }
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D colisor)
    {
        if (colisor.gameObject.tag == "Ladder")
        {
            if (!currentState.Contains("climbing"))
            {
                currentLadder = null;
                return;
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D colision)
    {
        if (colision.gameObject.tag == "Ground")
        {
            groundContact = true;
        }

        if (colision.gameObject.GetComponent<Floor>() != null)
        {
            currentFloor = colision.gameObject;
        }

        if (isInvulnerable())
        {
            if (colision.gameObject.GetComponent<Enemy>() != null)
            {
                ignoredCollisions.Add(colision.gameObject.GetComponent<Enemy>().myBounds);
                Physics2D.IgnoreCollision(myBounds, colision.gameObject.GetComponent<Enemy>().myBounds);

                return;
            }
        }


        base.OnCollisionEnter2D(colision);

        if (colision.gameObject.tag == "Wall")
        {
            isOnWall = true;
            currentWall = colision.gameObject;
        }


    }

    protected virtual void OnCollisionStay2D(Collision2D colision)
    {
        if (colision.collider.GetComponent<Floor>() != null)
        {
            //Utilities.//print((("enter");
            currentFloor = colision.collider.gameObject;
        }
    }

    protected override void OnCollisionExit2D(Collision2D colision)
    {
        base.OnCollisionExit2D(colision);

        if (colision.gameObject.tag == "Wall")
        {
            isOnWall = false;
        }
        /*AQUI*/
        if (colision.collider.GetComponent<Floor>() != null)
        {
                //Physics2D.IgnoreCollision(GetComponentInChildren<BoxCollider2D>(), colision.collider.GetComponent<BoxCollider2D>(), false);

            if (currentFloor == colision.collider.gameObject)
                currentFloor = null;
        }

        if (colision.gameObject.tag == "Ground")
        {
            groundContact = false;
        }
    }

    public bool CheckModifier(Commandments.Modifiers modifier)
    {
        return modifiers.Contains(modifier);
    }

    public bool ReceiveModifier(Commandments.Modifiers status, float upTime = float.PositiveInfinity)
    {
        return ReceiveModifier(status, upTime, true);
    }

    public bool RemoveModifier(Commandments.Modifiers status)
    {
        return ReceiveModifier(status, 0, false);
    }

    private bool ReceiveModifier(Commandments.Modifiers status, float upTime, bool active)
    {
        //print((("ReceiveStatus");
        if (isInvulnerable() && !status.isBuff() && active)
            return false;


        if (active)
        {
            if (CoolDownManager.RemainingTimeAbsolute("mod_"+status.ToString(), null) > upTime)
            {
                return true;
            }
            CoolDownManager.Remove("mod_" + status.ToString(), null);
            modifiers.Remove(status);
            CoolDownManager.Apply("mod_" + status.ToString(), null, upTime, () =>
            {
                switch (status)
                {
                    case Commandments.Modifiers.Paralysis:
                        animator.SetTrigger("triggerTakingDamage");       
                        break;
                    default:
                        break;
                }
                modifiers.Add(status);
                return true;
            });
        }
        else
        {
            modifiers.Remove(status);
            CoolDownManager.Remove("mod_" + status.ToString(), null);
        }

        
        return true;
    }

    public override float ReceiveDamage(Vector2 direction, float force, float damage, bool critical = false, Commandments.Element enemyElement = Commandments.Element.NEUTRAL, Character source = null)
    {
        if (currentState.Contains("skill_death"))
            return 0;
        if (currentState == "dying")
            return 0;

        bool ret = CoolDownManager.Apply(Commandments.Modifiers.Invulnerability.ToString(), gameObject, invulnerabilityTime, () =>
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            rigidBody.AddForce(direction * force);
            lastVulnerability = Time.time;
            blinker.Play();

            if (source != null)
            {
                ignoredCollisions.Add(source.myBounds);
                Physics2D.IgnoreCollision(myBounds, source.myBounds);
            }
            ApplyOnce.remove(Commandments.Modifiers.RemoveIgnoreColisions.ToString(), gameObject);

            if (!ApplyOnce.alreadyApplied("ReceiveDamage", gameObject))
            {
                if (takingDamageParameter != null)
                {
                    animator.SetTrigger(takingDamageParameter.name);
                }
                ApplyOnce.apply("ReceiveDamage", gameObject, () => { return true; });
            }

            if (CheckModifier(Commandments.Modifiers.ReduceDamage))
            {
                damage *= Commandments.Modifiers.ReduceDamage.GetValue();
            }

            if (CheckModifier(Commandments.Modifiers.ManaShield))
            {
                print("oi");
                if (CheckMPAvailable(damage * Commandments.Modifiers.ManaShield.GetValue()))
                {
                    print("mp is available");
                    ChangeMP(-damage * Commandments.Modifiers.ManaShield.GetValue());
                    ChangeHP(-damage * (1 - Commandments.Modifiers.ManaShield.GetValue()));
                }
                else
                {
                    ChangeHP(-damage);
                }
            }
            else
            {
                ChangeHP(-damage);
            }


            if (CheckModifier(Commandments.Modifiers.Thorns))
            {
                if (source != null)
                {
                    float dam = source.ReceiveDamage(-direction, force / 2, damage * Commandments.Modifiers.Thorns.GetValue());
                    if (CheckModifier(Commandments.Modifiers.Vampire))
                    {
                        ChangeHP(dam * Commandments.Modifiers.Vampire.GetValue());
                    }
                }
            }

            if (hp <= 0)
            {
                animator.SetTrigger("triggerDying");
            }

            /*shale camera*/
            gameManager.getCurrentCamera().StartBouncing(forca, frames, cushined);

            return true;
        });

        if (!ret)
            damage = 0;

        return damage;
    }


    #endregion

    #region Ui Events

    public override bool GetItem(Item item)
    {
        return uiManager.AddItem(item);

    }

    public void IncreaseUlt(float increase)
    {
        if (CheckModifier(Commandments.Modifiers.IncreaseUltimateRate) && value > 0)
            increase *= Commandments.Modifiers.IncreaseUltimateRate.GetValue();

        if (ultimate + increase > 1f)
            ultimate = 1f;
        else
            ultimate += increase;
        uiManager.ChangeUlt(ultimate);
    }

    public override void ChangeHP(float value)
    {
        if (isInvulnerable() && value < 0)
            return;

        if (CheckModifier(Commandments.Modifiers.IncreaseHealingRate) && value > 0)
            value *= Commandments.Modifiers.IncreaseHealingRate.GetValue();
        base.ChangeHP(value);
        uiManager.ChangeHP(hp / maxHP);
    }

    public bool ChangeMP(float increase) {
        if (modifiers.Contains(Commandments.Modifiers.InfiniteMP) && increase<0)
        {
            return true;
        }

        if (mp + increase < 0)
            return false;

        if (mp >= maxMP && increase >0)
            return false;

        mp += increase;
        if (mp > maxMP)
            mp = maxMP;

        //print((("mp = " + mp);
        mp = ((int)(mp * 100))/100.0f;
        //print((("mp = " + mp);
        uiManager.ChangeMP(mp/maxMP);
        return true;

    }

    public bool IsAvailable()
    {
        if (currentState == null)
            return false;

        if (currentState.Contains("sitting"))
            return true;

        switch (currentState.ToLower())
        {
            case "chilling":
            case "running":
                return true;
            default :
                return false;
        }
    }

    public void Reload()
    {
        ChangeMP(maxMP);
        ChangeHP(maxHP);
        ClearDebuffs();
        ResetCooldowns();
        VerifyAbilities();
    }

    public void VerifyAbilities()
    {
        foreach (Ability.AbilityId id in Enum.GetValues(typeof(Ability.AbilityId)))
        {
            CoolDownManager.Remove("mod_" + id.ToString());
        }

        foreach (Ability.AbilitySettings ability in Ability.GetSelectedAbilities())
        {
            if (ability.isActivatable)
                continue;
            foreach (Commandments.Modifiers mod in Enum.GetValues(typeof(Commandments.Modifiers)))
            {
                if (ability.description.id.ToString() == mod.ToString())
                {
                    ReceiveModifier(mod);
                    break;
                }
            }
                
        }

        foreach (Commandments.Modifiers mod in modifiers)
        {
            print("modifier [" + mod.ToString() + "]->Active");
        }
    }

    private void ResetCooldowns()
    {
        foreach (Ability.AbilityId id in Enum.GetValues(typeof(Ability.AbilityId)))
        {
            CoolDownManager.Remove(id.ToString());
        }
    }

    private void ClearDebuffs()
    {
        for(int i = 0;i<modifiers.Count;i++)
        { 
            if (!modifiers[i].isBuff())
            {
                RemoveModifier(modifiers[i]);
                i = -1;
            }
        }
    }


    #endregion

    #region Moviments Functions

    protected override bool GroundVerification()
    {
        Vector2 leftPoint, rightPoint;
        leftPoint = rightPoint = Vector2.zero;

        float yOffset;
        if (currentFloor != null)
        {
            if (currentFloor.GetComponent<EdgeCollider2D>() != null && currentFloor.GetComponent<EdgeCollider2D>().isTrigger == false)
            {
                Vector2[] points = currentFloor.GetComponent<EdgeCollider2D>().points;
                Vector2 offSet = currentFloor.GetComponent<EdgeCollider2D>().offset;

                yOffset = currentFloor.GetComponent<EdgeCollider2D>().offset.y;

                leftPoint = Vector2.Scale((points[0] + offSet), currentFloor.transform.lossyScale) + (Vector2)currentFloor.transform.position;
                rightPoint = Vector2.Scale((points[1] + offSet), currentFloor.transform.lossyScale) + (Vector2)currentFloor.transform.position;
                
                if (leftPoint.x > rightPoint.x)
                {
                    Vector2 aux = leftPoint;
                    leftPoint = rightPoint;
                    rightPoint = aux;
                }

                int i = 1;
                while (transform.position.x > rightPoint.x)
                {
                    i++;
                    if (i >= points.Length)
                    {
                        return false;
                    }

                    Vector2 auxPoint = Vector2.Scale((points[i] + offSet), currentFloor.transform.lossyScale) + (Vector2)currentFloor.transform.position;

                    if (auxPoint.x > rightPoint.x)
                    {
                        leftPoint = rightPoint;
                        rightPoint = auxPoint;
                    } else
                    {
                        rightPoint = leftPoint;
                        leftPoint = auxPoint;
                    }

                }

            }
            else if (currentFloor.GetComponent<BoxCollider2D>() != null  && currentFloor.GetComponent<BoxCollider2D>().isTrigger == false)
            {
                Vector2 floorSize = currentFloor.GetComponent<BoxCollider2D>().size;
                float rotation = currentFloor.transform.rotation.eulerAngles.z;
                Vector2 scale = new Vector2(currentFloor.transform.localScale.x, currentFloor.transform.localScale.y);
                Vector2 floorPosition = currentFloor.transform.position;
                floorPosition += currentFloor.GetComponent<BoxCollider2D>().offset;

                leftPoint.y = floorPosition.y + Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * scale.y - Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * scale.x;
                leftPoint.x = floorPosition.x - Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * scale.y - Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * scale.x;

                rightPoint.y = floorPosition.y + Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * scale.y + Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * scale.x;
                rightPoint.x = floorPosition.x - Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * scale.y + Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * scale.x;

                yOffset = currentFloor.GetComponent<BoxCollider2D>().offset.y;
            }
            else
            {
                return false;
            }


            float deltaX, deltaY;
            deltaX = rightPoint.x - leftPoint.x;
            deltaY = (rightPoint.y - leftPoint.y);



            if ((transform.position.x - myBounds.bounds.extents.x > rightPoint.x + 0.2f) || (transform.position.x + myBounds.bounds.extents.x < leftPoint.x - 0.2f))
            {
                return false;
            }

            if (transform.position.y >= (((transform.position.x - leftPoint.x) * deltaY / deltaX) + leftPoint.y + yOffset))
            {
                return true;
            }

        }

        return groundContact;
    }

    protected override bool LedgeVerification()
    {
        if (!isOnGround)
            return false;

        /*if (currentFloor == null)
            return false;*/

        if (currentFloor != null)
        {
            Floor.Boundaries currentFloorBoundaries = currentFloor.GetComponent<Floor>().GetLedge();
            switch ((Direction)getDirection())
            {
                case Direction.Left:
                    if (transform.position.x < currentFloorBoundaries.leftPoint.x + ledgeVerificationRange)
                    {
                        return true;
                    }
                    break;
                case Direction.Right:
                    if (transform.position.x > currentFloorBoundaries.rightPoint.x - ledgeVerificationRange)
                    {
                        return true;
                    }
                    break;
            }
        }

        return false;
    }

    public bool isInvulnerable()
    {
        return CoolDownManager.RemainingTimePercent(Commandments.Modifiers.Invulnerability.ToString(), gameObject) > 0;
    }

    protected override void ControlVelocity()
    {
        
        if (CheckModifier(Commandments.Modifiers.Paralysis))
            return;

        if (ApplyOnce.alreadyApplied("ReceiveDamage", gameObject))
            return;

        base.ControlVelocity();
    }

    protected override void SetVerticalSpeed()
    {
        base.SetVerticalSpeed();

        float verticalSpeed = rigidBody.velocity.y;
        if (!isOnGround && verticalSpeed < -0.05 )
        {
            verticalSpeed /= -1*maxSpeed;
            
            if (verticalSpeed > 0.99f)
            {
                animator.SetInteger("landingLevel", 2);
            }
            else if (verticalSpeed > 0.8f)
            {
                animator.SetInteger("landingLevel", 1);
            }
            else
            {
                animator.SetInteger("landingLevel", 0);
            }
        }
        
    }
    #endregion

    #region Misc

    private string PrepareAttack(AttackType type)
    {
        //print((("Attack Type = " + type);
        animator.SetInteger("attackType", (int)type);
        animator.SetInteger("weapon", (int)uiManager.getSelectedWeapon());
        nextAnimationInput = PlayerInputAnimation.Nothing;
        return "activateAttacking";
    }

    public override Commandments.Element getElement()
    {
        return uiManager.getSelectedWeapon().toElement();
    }

   

    public void ReceiveNotification(Notification notice)
    {
        if (notice == Notification.Return)
        {
            animator.SetTrigger("triggerReturn");
        }
        else
        {
            animator.ResetTrigger("triggerReturn");
            animator.SetTrigger("triggerNotification");
        }
        animator.SetInteger("notificationType", (int)notice);

        ApplyOnce.apply("Notification", gameObject, () => {

            //print("ReceiveNot: " + notice);

           
            return true;
        });
    }

    private void ActivateAbility(PlayerInputAnimation input)
    {
        //Todo Corrigir
        Ability.AbilityId id = Ability.GetID((ActiveAbilitiesMenu.Options)(input - PlayerInputAnimation.Ability_Triangle));
        Ability.AbilitySettings settings = Ability.GetAbilityInfo(id);

        CoolDownManager.Apply(id.ToString(), null, settings.active.cooldown, () =>
        {

            switch (id) //TODO Corrigir
            {
                case Ability.AbilityId.InfiniteMP:

                    CoolDownManager.Apply(Commandments.Modifiers.InfiniteMP.ToString(), gameObject, settings.active.cooldown, () =>
                    {
                        ReceiveModifier(Commandments.Modifiers.InfiniteMP, settings.active.upTime);
                        return true;
                    });
                    break;

                case Ability.AbilityId.Invulnerability:

                    CoolDownManager.Apply(Commandments.Modifiers.Invulnerability.ToString(), gameObject, settings.active.cooldown, () =>
                    {
                        ReceiveModifier(Commandments.Modifiers.Invulnerability, settings.active.upTime);
                        return true;
                    });
                    break;

                case Ability.AbilityId.ManaShield:

                    CoolDownManager.Apply(Commandments.Modifiers.ManaShield.ToString(), gameObject, settings.active.cooldown, () =>
                    {
                        ReceiveModifier(Commandments.Modifiers.ManaShield, settings.active.upTime);
                        return true;
                    });
                    break;

                case Ability.AbilityId.Sonar:
                    mySize = GetComponentInChildren<BoxCollider2D>().size * sprite.transform.localScale.y;
                    uiManager.ActivateAbility(Ability.AbilityId.Sonar, new Vector3(transform.position.x, transform.position.y + mySize.y, transform.position.z));
                    break;
            }
            return true;
        });

    }

    AttackType convertToAttack(PlayerInputAnimation anim)
    {

        switch (anim)
        {

            case PlayerInputAnimation.HeavyAttack:
                return AttackType.HeavyAttack;

            case PlayerInputAnimation.HeavySpecial:
                return AttackType.HeavySpecial;

            case PlayerInputAnimation.HoldingAttack:
                return AttackType.HoldingAttack;

            case PlayerInputAnimation.HoldingSpecial:
                return AttackType.HoldingSpecial;

            case PlayerInputAnimation.QuickAttack:
                return AttackType.QuickAttack;

            case PlayerInputAnimation.QuickSpecial:
                return AttackType.QuickSpecial;

            default:
                return AttackType.Nothing;
        }

    }
    
    IEnumerator ChangeLevel()
    {
        float fadeTime = GameObject.Find("Fader").GetComponent<Fader>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        Application.LoadLevel(Application.loadedLevel);
        fadeTime = GameObject.Find("Fader").GetComponent<Fader>().BeginFade(-1);
    }

    IEnumerator CreateShadows()
    {
            speedRunning *= factorR;
            //float direction = 1;
            float direction = getDirection();

            //print(((direction*dashForce);

            if (rigidBody.velocity.y < 0)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            }
            if (!isOnGround)
                rigidBody.AddForce(new Vector2(direction * dashForce, dashForce * Mathf.Sin(Mathf.Deg2Rad * value)));
            else
            {
                //print(((new Vector2(direction * dashForce, 0));
                rigidBody.AddForce(new Vector2(direction * dashForce, 0));
            }
            
          
            for (int i = 0; i < 9; i++)
            {
                GameObject mirror = (GameObject)Instantiate(gameObject, new Vector3(transform.position.x, transform.position.y, transform.position.z + 10), Quaternion.identity);
                mirror.transform.eulerAngles = gameObject.transform.eulerAngles;
                mirror.GetComponent<Rigidbody2D>().isKinematic = true;
                mirror.GetComponentInChildren<Animator>().StopPlayback();
                foreach (Collider2D c in mirror.GetComponentsInChildren<Collider2D>())
                {
                    c.enabled = false;
                }
                mirror.GetComponent<Asderek>().enabled = false;
                mirror.AddComponent<ObjectFader>();

                yield return new WaitForSeconds(0.05f);
            }

            speedRunning /= factorR;
            //rigidBody.gravityScale = 1;

    }

    protected bool CheckDownJump()
    {
        if (currentFloor != null && currentFloor.GetComponent<Floor>().isPassable)
        {
            if (transform.position.y > currentFloor.GetComponent<Floor>().GetYPoint(transform.position.x))
            {

                foreach (Floor bc in currentFloor.transform.parent.gameObject.GetComponentsInChildren<Floor>())
                {
                    bc.ignoreColision(gameObject.GetComponentInChildren<BoxCollider2D>(), true);
                }
                isDownJumping = true;
                return true;
            }
        }


        return false;
    }

    public Vector2 getPlayerPosition()
    {
        return (Vector2)transform.position + new Vector2(0, mySize.y);
    }

    public void LerpToPosition(Vector2 destiny)
    {
        TurnCharacter(destiny);
        goingToLerpLocation = true;
        this.destiny = destiny;
    }

    public void ResetLerp()
    {
        goingToLerpLocation = false;
    }

    public float StateOfAnimation(string animation)
    {
        animation = animation.ToLower();

        if (currentState.ToLower() != animation)
            return -1;

        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
        
    public bool FinishedAnimation(string animation, float percentage=1)
    {
        animation = animation.ToLower();

        //print("FINISHED_animation = " + animation + " - currentState = " + currentState.ToLower());
        if (currentState.ToLower() != animation)
            return false;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= percentage)
            return true;


        return false;
    }

    public bool CanInteract()
    {
        switch (currentState.ToLower().Replace("_", "."))
        {

            case "sitting.death":
            case "sitting.fire":
            case "sitting.earth":
            case "sitting.ice":
            case "sitting.steel":
            case "sitting.water":
            case "chilling":
            case "running":
                return true;

            case "climbing.right":
            case "climbing.left":
            case "climbing.pullingup":
            case "climbing.preparing":
            case "climbing.downprep":
            case "jumping.rising":
            case "jumping.falling":
            case "jumping.double":
            case "jumping.preparing":
            case "jumping.landingsoft":
            case "jumping.landingheavy":
            case "jumping.sliding":
            case "rolling":
            case "pullingup":
            case "attacking.bow.holding":
            case "attacking.bow.walking":
            case "attacking.bow.release":
            case "attacking.death.normal.quickp1":
            case "attacking.death.normal.quickp2":
            case "attacking.death.normal.quickp3":
            case "attacking.death.normal.heavyp1":
            case "attacking.death.normal.heavyp2":
            case "attacking.death.normal.air":
            case "attacking.death.normal.airwait":
            case "attacking.water.normal.air":
            case "attacking.water.normal.airwait":
            case "attacking.water.normal.quickp1":
            case "attacking.water.normal.quickp2":
            case "attacking.water.normal.quickp3":
            case "attacking.water.normal.heavyp1":
            case "attacking.death.special.quick":
            case "attacking.death.special.heavy":
            case "attacking.water.special.heavy":
            case "attacking.water.special.quick":
            case "attacking.death.normal.hold":
            case "attacking.death.special.hold":
            case "attacking.water.normal.hold":
            case "attacking.water.normal.heavyp2":
            case "attacking.water.special.hold":
            case "attacking.water.special.preparehold":
            case "gettingup.faster":
            case "gettingup.slower":
            case "takingdamage":
            case "notifications.water.enter":
            case "notifications.water.stay":
            case "notifications.water.exit":
            case "kneeling.goingdown":
            case "kneeling.gettingup":
            case "skill.death.fase.1":
            case "skill.death.fase.2":
            case "skill.death.glide":
            case "skill.death.return":
            case "dying":
                break;
        }
        return false;
    }

    protected override void AchieveMaxSpeed(float MaxSpeed)
    {
        if (currentState.ToLower().Contains("rolling"))
        {
            base.AchieveMaxSpeed(MaxSpeed * Commandments.Modifiers.IncreaseRolling.GetValue());
            return;
        }

        base.AchieveMaxSpeed(MaxSpeed * Commandments.Modifiers.IncreaseMoveSpeed.GetValue());
    }

    private void ApplyModifiers()
    {

        foreach (Commandments.Modifiers mod in modifiers)
        {
            switch (mod)
            {
                case Commandments.Modifiers.Poison:
                    ChangeHP(mod.GetValue());
                    break;
                case Commandments.Modifiers.Regen:
                    ChangeHP(mod.GetValue());
                    break;
            }
        }
    }

    #endregion
}