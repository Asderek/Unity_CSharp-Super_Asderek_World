using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class FreeCamera : BaseCamera
{

    public GameObject oldTarget;
    private GameObject sprite;

    public float targetXLocation = 0.4f;
    public float targetYLocation = 0.2f;
    public float controlDeviation = 0.15f;
    public float dampeningFactor = 0.025f;


    private Vector2 screen = new Vector2(20, 10);
    private Vector2 destiny = new Vector2(0, 0);

    float direction = 1;
    float currentDirection = 1;
    float invertDirection = 0;
    private float colldown = 1f;


    public float targetMaxYLimit = 0.6f;
    public float targetMinYLimit = 0.1f;

    private bool fixCameraY = false;
    private float cameraMaxVerticalLimit, cameraMinVerticalLimit;
    private float cameraMaxHorizontalLimit, cameraMinHorizontalLimit;
    private float error = 0.6f;

    protected override void Start()
    {
        base.Start();

        if (oldTarget == null)
        {
            oldTarget = GameObject.FindGameObjectWithTag("Player");
            transform.position = new Vector3(oldTarget.transform.position.x,oldTarget.transform.position.y,transform.position.z);
        }

        foreach (Transform obj in oldTarget.GetComponentsInChildren<Transform>())
        {
            if (obj.gameObject.tag == "PlayerSpriteTag")
            {
                sprite = obj.gameObject;
                break;
            }
        }

        cameraMaxHorizontalLimit = cameraMinHorizontalLimit = cameraMaxVerticalLimit = cameraMinVerticalLimit = float.NaN;
        destiny.y = oldTarget.transform.position.y + ((0.5f - targetYLocation) * screen.y);

    }
    
    void Update()
    {

        //base.Update();

        if ((((int)oldTarget.transform.eulerAngles.y) / -90) + 1 != currentDirection)
        {
            // //print((("Inverti");
            invertDirection = Time.time;
            currentDirection = (((int)oldTarget.transform.eulerAngles.y) / -90) + 1;
        }
        //print((("Invert" + invertDirection + "time " + Time.time);
        if (Time.time - invertDirection > colldown)
        {
            //print((("Aceitei inversao");
            direction = (((int)oldTarget.transform.eulerAngles.y) / -90) + 1;
        }

        int factor = 1;

        if (direction > 0)
        {
            destiny.x = oldTarget.transform.position.x + (0.5f - targetXLocation) * screen.x;
        }
        else
        {
            destiny.x = oldTarget.transform.position.x - (0.5f - targetXLocation) * screen.x;
        }


        /*<Calculando Y com limits verticais>*/


        float yVelocity = 0;

        //print((("Target Position = " + target.transform.position.y + " - " + "Camera Position = " + transform.position.y + " - " + "TargetPosition - CameraPosition = " + (target.transform.position.y - transform.position.y) + " - " + "TargetMinVertLimit = " + (targetMinYLimit - 0.5f) * screen.y + "TargetMaxVertLimit = " + (targetMaxYLimit - 0.5f) * screen.y);

        if (oldTarget.transform.position.y - transform.position.y < (targetMinYLimit - 0.5f) * screen.y || oldTarget.transform.position.y - transform.position.y > (targetMaxYLimit - 0.5f) * screen.y)
        {
            fixCameraY = true;

        }
        if (fixCameraY == true)
        {
            if (transform.position.y > (oldTarget.transform.position.y + ((0.5f - targetYLocation) * screen.y) - error) && transform.position.y < (oldTarget.transform.position.y + ((0.5f - targetYLocation) * screen.y) + error))
            {
                fixCameraY = false;
            }
            else
            {
                destiny.y = oldTarget.transform.position.y + ((0.5f - targetYLocation) * screen.y);
                yVelocity += (yVelocity - oldTarget.GetComponent<Rigidbody2D>().velocity.y) / 2;
            }

        }
        /*</Calculando Y com limits verticais>*/

        if (ButtonManager.Get(ButtonManager.ButtonID.L_RIGHT))
        {
            destiny.x += controlDeviation * screen.x;
            factor = 3;
        }
        else if (ButtonManager.Get(ButtonManager.ButtonID.L_LEFT))
        {
            destiny.x -= controlDeviation * screen.x;
            factor = 3;
        }

        float destinYChange = 0f;
        //print(((Input.GetAxisRaw("AnalogLeftY") + " - " + Input.GetAxisRaw("AnalogLeftX"));
        if (ButtonManager.Get(ButtonManager.ButtonID.L_DOWN))
        {
            destinYChange = 1.0f;
            factor = 3;
        }
        else if (ButtonManager.Get(ButtonManager.ButtonID.L_UP))
        {
            destinYChange = -1;
            factor = 3;
        }
        destinYChange *= controlDeviation * screen.y;

        //print((("Destiny = " + destiny);

        Vector3 newPosition = new Vector3(destiny.x, destiny.y + destinYChange, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, dampeningFactor * targetXLocation * factor);//Mathf.Exp((1 - targetLocationOnScreen)) * 0.01f);//dampeningFactor); 

        /*if (oldTarget.GetComponent<Asderek>().status[Commandments.Status.PARALYSIS.toInt()] != true)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(oldTarget.GetComponent<Rigidbody2D>().velocity.x, yVelocity);
        }*/
        
        /*<Se a camera ficar louca, força a posição>*/

        if(oldTarget.transform.position.x > transform.position.x)
        {
            if(oldTarget.transform.position.x -destiny.x   > 0.4f*screen.x)
            {
                destiny.x = oldTarget.transform.position.x - 0.4f*screen.x;
            }
        }
        else
        {
            if(oldTarget.transform.position.x - destiny.x   < -0.4f*screen.x)
            {
                destiny.x = oldTarget.transform.position.x + 0.4f*screen.x;
            }
        }

        Vector3 teste = new Vector3(destiny.x, destiny.y + destinYChange, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, teste, dampeningFactor * targetXLocation * factor);

        /*</Se a camera ficar louca, força a posição>*/



        /*<Se o personagem estiver subindo ou descendo muito rapido>*/

        float cameraDifference = sprite.GetComponent<BoxCollider2D>().size.y * sprite.transform.localScale.y;

        if (oldTarget.transform.position.y - transform.position.y > 0.5f * screen.y - cameraDifference)
            transform.position = new Vector3(transform.position.x, oldTarget.transform.position.y - 0.5f * screen.y + cameraDifference, transform.position.z);
        else if (transform.position.y - oldTarget.transform.position.y > 0.5f * screen.y)
            transform.position = new Vector3(transform.position.x, oldTarget.transform.position.y + 0.5f * screen.y, transform.position.z);

        /*</Se o personagem estiver subindo ou descendo muito rapido>*/

        /*<Testa os limites superiores e inferiores da camera>*/

        if (cameraMaxVerticalLimit.CompareTo(float.NaN) != 0)
        {
            //  //print((("NaN Top");
            if (transform.position.y > cameraMaxVerticalLimit)
                transform.position = new Vector3(transform.position.x, cameraMaxVerticalLimit, transform.position.z);
        }

        if (cameraMinVerticalLimit.CompareTo(float.NaN) != 0)
        {
            // //print((("NaN Bot");
            if (transform.position.y < cameraMinVerticalLimit)
                transform.position = new Vector3(transform.position.x, cameraMinVerticalLimit, transform.position.z);
        }

        if (cameraMaxHorizontalLimit .CompareTo(float.NaN) != 0)
        {
            //  //print((("NaN Top");
            if (transform.position.x > cameraMaxHorizontalLimit)
                transform.position = new Vector3(cameraMaxHorizontalLimit, transform.position.y, transform.position.z);
        }

        if (cameraMinHorizontalLimit.CompareTo(float.NaN) != 0)
        {
            // //print((("NaN Bot");
            if (transform.position.x < cameraMinHorizontalLimit)
                transform.position = new Vector3(cameraMinHorizontalLimit, transform.position.y, transform.position.z);
        }
        /*</Testa os limites superiores e inferiores da camera>*/


    }

    void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.gameObject.CompareTag("CameraLimitY"))
        {
            //print((("enter");
            if (colisor.transform.position.y > transform.position.y)
            {
                cameraMaxVerticalLimit = colisor.transform.position.y - 5;
                //print((("CameraMax = " + cameraMaxVerticalLimit + "     Atual = " + transform.position.y);
            }
            else
            {
                cameraMinVerticalLimit = colisor.transform.position.y + 5;
                // //print((("CameraMin = " + cameraMinVerticalLimit + "     Atual = " + transform.position.y);
            }
        }

        if (colisor.gameObject.CompareTag("CameraLimitX"))
        {
            //print((("enter");
            if (colisor.transform.position.x > transform.position.x)
            {
                cameraMaxHorizontalLimit = colisor.transform.position.x - 10;
                //print((("CameraMax = " + cameraMaxVerticalLimit + "     Atual = " + transform.position.y);
            }
            else
            {
                cameraMinHorizontalLimit = colisor.transform.position.x + 10;
                // //print((("CameraMin = " + cameraMinVerticalLimit + "     Atual = " + transform.position.y);
            }
        }
    }

    void OnTriggerExit2D(Collider2D colisor)
    {
        if (colisor.gameObject.CompareTag("CameraLimitY"))
        {
            //print((("exit");
            if (colisor.transform.position.y > transform.position.y)
            {
                cameraMaxVerticalLimit = float.NaN;
            }
            else
            {
                cameraMinVerticalLimit = float.NaN;
            }
            //print((("cameraMin = "+cameraMinVerticalLimit + "  CameraMax = "+cameraMaxVerticalLimit);
        }

        if (colisor.gameObject.CompareTag("CameraLimitX"))
        {
            //print((("enter");
            if (colisor.transform.position.x > transform.position.x)
            {
                cameraMaxHorizontalLimit = float.NaN;
                //print((("CameraMax = " + cameraMaxVerticalLimit + "     Atual = " + transform.position.y);
            }
            else
            {
                cameraMinHorizontalLimit = float.NaN;
                // //print((("CameraMin = " + cameraMinVerticalLimit + "     Atual = " + transform.position.y);
            }
        }
    }

}
