using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCamera : MonoBehaviour {

    [Range(0f, 1f)]
    public float staticTargetPositionX = 0.5f; //onde o personagem deve ficar na tela quando parado
    [Range(0f, 1f)]
    public float staticTargetPositionY = 0.5f; //onde o personagem deve ficar na tela quado parado

    [Range(0f, 1f)]
    public float movingTargetPositionX = 0.3f; //onde o personagem deve ficar na tela
    [Range(0f, 1f)]
    public float movingTargetPositionY = 0.3f; //onde o personagem deve ficar na tela
    [Range(0f, 1f)]
    public float cameraXThreashold = 0.66f;   //a partir de onde, a tela deve ser alinhada
    [Range(0f, 1f)]
    public float cameraYThreashold = 0.6f;   //a partir de onde, a tela deve ser alinhada
    public float ERROR = 0.1f;
    public Vector2 velocity;

    protected GameManager manager;

    public MonoBehaviour target;
    public Vector2 specificLocation;
    public MonoBehaviour tempTarget;
    public GameObject greenBall;
    public GameObject blueBall;


    protected Camera myCamera;
    private Vector2 cameraPosition;
    private Vector2 screenSize;

    public Vector3 auxVector;

    #region Bools
    bool allignLeft = false;
    bool allignRight = false;
    bool allignUp = false;
    bool allignDown = false;
    private bool newLocation;
    #endregion

    #region BounceVariables
    private float disturbanceUpdates = 0;
    private float currentDisturbance = 0;
    private float speedDisturbance = 0.2f;
    private float sizeOfDisturbance;
    private float withoutDisturbance;
    private bool cushioned = false;
    private bool bouncing = false;
    private List<Collider2D> ignoredCollisions;
    #endregion

    // Use this for initialization
    protected virtual void Start() {
        manager = GameManager.GetInstance();
        manager.RegisterCamera(this);

        if (target == null)
        {
            target = manager.getPlayer();
        }

        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);

        myCamera = GetComponent<Camera>();
        screenSize = new Vector2(myCamera.orthographicSize * myCamera.aspect, myCamera.orthographicSize) * 2;

        GetComponent<BoxCollider2D>().size = screenSize;



        ignoredCollisions = new List<Collider2D>();
        ignoredCollisions.Remove(GetComponent<BoxCollider2D>());
        print("start = " + ignoredCollisions);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate() {

        cameraPosition = (Vector2)transform.position - screenSize / 2;

        if (newLocation)
            GoToLocation();
        else if (tempTarget != null)
            FollowTarget(tempTarget);
        else
            FollowTarget(target);

        if (bouncing)
        {
            ApplyBouncing();
            return;
        }

    }

    public Area.Position GetTargetPosition()
    {
        float desvioX = (target.transform.position.x - cameraPosition.x)/screenSize.x;
        float desvioY = 1f - (target.transform.position.y - cameraPosition.y)/screenSize.y - 0.1f;
        return new Area.Position(desvioX, desvioY);
    }

    public Area.Position GetTargetPositionOnScreen(Vector2 target)
    {
        float desvioX = (target.x - cameraPosition.x) / screenSize.x;
        float desvioY = 1f - (target.y - cameraPosition.y) / screenSize.y;
        return new Area.Position(desvioX, desvioY);
    }

    protected virtual void FollowTarget(MonoBehaviour target)
    {
        float newX;
        float newY;
        Vector3 novaPosicao;

        Asderek playerScript = target.GetComponent<Asderek>();
        if (playerScript != null)
        {
            float state = playerScript.StateOfAnimation("dying");
            /*if (state > 0)
            {
                newX = target.transform.position.x - (cameraPosition.x + screenSize.x * staticTargetPositionX) + transform.position.x;
                newY = target.transform.position.y - (cameraPosition.y + screenSize.y * staticTargetPositionY) + transform.position.y;

                novaPosicao = new Vector3(newX, newY, transform.position.z) - auxVector;
                transform.position = Vector3.Lerp(transform.position, novaPosicao, Mathf.Min((state),1));
                return;
            }*/

            state = playerScript.StateOfAnimation("sitting_neutral");
            if (state > 0)
            {
                newX = target.transform.position.x - (cameraPosition.x + screenSize.x * staticTargetPositionX) + transform.position.x;
                newY = target.transform.position.y - (cameraPosition.y + screenSize.y * staticTargetPositionY) + transform.position.y;

                novaPosicao = new Vector3(newX, newY, transform.position.z) - auxVector;
                transform.position = Vector3.Lerp(transform.position, novaPosicao, Mathf.Min(2*state,1));
                return;
            }

            state = playerScript.StateOfAnimation("seated");
            if (state > 0)
            {
                newX = target.transform.position.x - (cameraPosition.x + screenSize.x * staticTargetPositionX) + transform.position.x;
                newY = target.transform.position.y - (cameraPosition.y + screenSize.y * staticTargetPositionY) + transform.position.y;

                transform.position = new Vector3(newX, newY, transform.position.z) - auxVector;
                return;
            }
        }


        if (target.GetComponent<Rigidbody2D>() == null)
        {
            SetGoToLocation(target.transform.position);
            ClearTempTarget();
            return;
        }


        float desvioX = target.transform.position.x - (cameraPosition.x + screenSize.x * staticTargetPositionX);
        float desvioY = target.transform.position.y - (cameraPosition.y + screenSize.y * staticTargetPositionY);

        Vector2 cam = myCamera.orthographicSize * myCamera.rect.size;
        newX = transform.position.x;
        newY = transform.position.y;

        if (target.GetComponent<Rigidbody2D>().velocity.x > GameManager.globalZero)
        {
            allignLeft = false;
            if (target.transform.position.x > (cameraPosition.x + screenSize.x * cameraXThreashold) || allignRight) //Se eu tiver que corrigir
            {
                newX = target.transform.position.x - (cameraPosition.x + screenSize.x * movingTargetPositionX) + transform.position.x;
                allignRight = true;
            }
        }
        else if (target.GetComponent<Rigidbody2D>().velocity.x < -GameManager.globalZero)
        {
            allignRight = false;
            if (target.transform.position.x < (cameraPosition.x + screenSize.x * (1 - cameraXThreashold)) || allignLeft)
            {
                newX = target.transform.position.x - (cameraPosition.x + screenSize.x * (1 - movingTargetPositionX)) + transform.position.x;
                allignLeft = true;
            }

        }
        else
        {
            allignLeft = allignRight = false;
            if (transform.position.x != target.transform.position.x)
            {
                newX = target.transform.position.x - (cameraPosition.x + screenSize.x * staticTargetPositionX) + transform.position.x;
            }
        }


        if (target.GetComponent<Rigidbody2D>().velocity.y > GameManager.globalZero)
        {
            //print("velocidade ,aopr");
            /*allignDown = false;
            if ((transform.position.y <= (target.transform.position.y - cam.y * 0.35f)) || allignUp)
            {
                newY += desvioY * 0.05f;
                newY += target.GetComponent<Rigidbody2D>().velocity.y * 0.02f;
                allignUp = true;
            }*/
            allignDown = false;
            if (target.transform.position.y > (cameraPosition.y + screenSize.y * (cameraYThreashold)) || allignUp) //Se eu tiver que corrigir
            {
                //print("posicao maior");
                newY = target.transform.position.y - (cameraPosition.y + screenSize.y * (movingTargetPositionY)) + transform.position.y;
                allignUp = true;
            }
        }
        else if (target.GetComponent<Rigidbody2D>().velocity.y < -GameManager.globalZero)
        {
            //print("velocidade menor");
            /*allignUp = false;
            if ((transform.position.y >= (target.transform.position.y + cam.y * 0.35f)) || allignDown)
            {

                newY += desvioY * 0.05f;
                newY += target.GetComponent<Rigidbody2D>().velocity.y * 0.02f;
                allignDown = true;
            }*/
            allignUp = false;
            if (target.transform.position.y < (cameraPosition.y + screenSize.y * (1-cameraYThreashold)) || allignDown) //Se eu tiver que corrigir
            {
                //print("posicao menor");
                newY = target.transform.position.y - (cameraPosition.y + screenSize.y * (1-movingTargetPositionY)) + transform.position.y;
                allignDown = true;
            }
        }
        else
        {
            allignUp = allignDown = false;
            if (transform.position.y != target.transform.position.y)
            {
                newY = target.transform.position.y - (cameraPosition.y + screenSize.y * staticTargetPositionY) + transform.position.y;
            }
        }




        novaPosicao = new Vector3(newX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, novaPosicao, velocity.x * Time.deltaTime);

        novaPosicao = new Vector3(transform.position.x, newY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, novaPosicao, velocity.y * Time.deltaTime);
    }

    protected virtual void GoToLocation()
    {
        Vector3 desiredPosition = new Vector3(specificLocation.x, specificLocation.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, velocity.x * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        try
        {
            switch ((Commandments.LimitsType)Enum.Parse(typeof(Commandments.LimitsType), collision.otherCollider.gameObject.tag ))
            {
                case Commandments.LimitsType.CameraLimitUp:
                    if (collision.otherCollider.transform.position.y < transform.position.y)
                    {
                        MyIgnoreCollision(collision, true);
                    }
                    break;
                case Commandments.LimitsType.CameraLimitDown:
                    if (collision.transform.position.y > transform.position.y)
                    {
                        MyIgnoreCollision(collision, true);
                    }
                    break;
                case Commandments.LimitsType.CameraLimitLeft:
                    if (collision.transform.position.x > transform.position.x)
                    {
                        MyIgnoreCollision(collision, true);
                    }
                    break;
                case Commandments.LimitsType.CameraLimitRight:
                    if (collision.transform.position.x < transform.position.x)
                    {
                        MyIgnoreCollision(collision, true);
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            return;
        }
    }

    private void MyIgnoreCollision(Collision2D collision, bool ignore)
    {

        if (ignoredCollisions == null)
            ignoredCollisions = new List<Collider2D>();
        Physics2D.IgnoreCollision(collision.otherCollider, collision.collider, ignore);
        if (ignore)
        {
            ignoredCollisions.Add(collision.collider);
        }
        else
        {
            print("Other =" + collision.collider.gameObject.tag);
            if (ignoredCollisions.Contains(collision.collider))
                ignoredCollisions.Remove(collision.collider);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        MyIgnoreCollision(collision, false);
    }
    public virtual void StartBouncing(float disturbanceSize, float numberUpdates, bool cushioned = false)
    {
        sizeOfDisturbance = disturbanceSize;
        speedDisturbance = sizeOfDisturbance / 3f;
        withoutDisturbance = transform.position.x;
        disturbanceUpdates = numberUpdates;
        this.cushioned = cushioned;

        bouncing = true;
    }

    protected virtual void ApplyBouncing()
    {
        //if (Time.timeScale == 0)
        //{

            if (disturbanceUpdates == 0)
                return;
            disturbanceUpdates--;

            if (currentDisturbance >= sizeOfDisturbance)
            {
                speedDisturbance *= -1;
                currentDisturbance = sizeOfDisturbance;
                if (cushioned)
                {
                    sizeOfDisturbance *= 0.9f;
                }
            }
            else if (currentDisturbance <= -sizeOfDisturbance)
            {
                speedDisturbance *= -1;
                currentDisturbance = sizeOfDisturbance;
                if (cushioned)
                {
                    sizeOfDisturbance *= 0.9f;
                }
            }

            currentDisturbance += speedDisturbance;
            transform.position = new Vector3(transform.position.x + currentDisturbance, transform.position.y, transform.position.z);
        
    }

    public void SetTempTarget(MonoBehaviour newTarget)
    {
        tempTarget = newTarget;
    }

    private void ClearTempTarget()
    {
        tempTarget = null;
    }

    public void SetGoToLocation(Vector2 location)
    {
        newLocation = true;
        specificLocation = location;
    }

    private void ClearGoToLocation()
    {
        newLocation = false;
    }

    public void ClearChanges()
    {
        ClearTempTarget();
        ClearGoToLocation();
    }

    public void InstantChangeLocation(GameObject target)
    {
        //print("target.transform.position.x" + target.transform.position.x);
        //print("cameraPosition.x " + cameraPosition.x);
        //print("screenSize.x " + screenSize.x);
        //print("staticTargetPositionX.x " + staticTargetPositionX);
        //print("transform.position.x " + transform.position.x);

        float newX = target.transform.position.x - (cameraPosition.x + screenSize.x * staticTargetPositionX) + transform.position.x;
        float newY = target.transform.position.y - (cameraPosition.y + screenSize.y * staticTargetPositionY) + transform.position.y;

        transform.position = new Vector3(newX, newY, transform.position.z) - auxVector;
    }


    public bool isOnCamera(Transform objectPosition)
    {
        Area.Position pos = GetTargetPositionOnScreen(objectPosition.position);
        if ((pos.x < 0) || (pos.x > 1))
            return false;
        if ((pos.y < 0) || (pos.y > 1))
            return false;

        return true;
    }

    private void OnDestroy()
    {
        if (manager)
            manager.RegisterCamera(null);
    }

}
