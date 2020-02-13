using UnityEngine;
using System.Collections;

public class CameraScript : BaseCamera
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        myCamera = GetComponent<Camera>();
        //targetPositionOnScreen = 0.5f - targetPositionOnScreen;
        cameraXThreashold = 0.5f - cameraXThreashold;
        

        transform.position = new Vector3(target.transform.position.x,target.transform.position.y,transform.position.z);
    }

    // Update is called once per frame
    protected override void FollowTarget(MonoBehaviour target)
    {

        
    }
}/*


using UnityEngine;
using System.Collections;

public class CameraScript : BaseCamera
{
    bool allignLeft = false;
    bool allignRight = false;
    bool allignUp = false;
    bool allignDown = false;
    private Transform targetTransform;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        myCamera = GetComponent<Camera>();
        targetPositionOnScreen = 0.5f - targetPositionOnScreen;
        cameraCorrectionThreshold = 0.5f - cameraCorrectionThreshold;

        targetTransform = target.transform;

        transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, transform.position.z);
    }

    protected override void Update() { }

    // Update is called once per frame
    protected override void FixedUpdate()
    {

        float desvioX = targetTransform.transform.position.x - transform.position.x;
        float desvioY = (targetTransform.transform.position.y + 4) - transform.position.y;
        //print((("Player" + player.transform.position.y + "Tela " + Screen.height*0.1f + "Eu " +  transform.position.y);

        Vector2 cam = myCamera.orthographicSize * myCamera.rect.size;
        float newX = transform.position.x;
        float newY = transform.position.y;
        
        if (targetTransform.GetComponent<Rigidbody2D>().velocity.x > 0)
        {
            allignLeft = false;
            if ((transform.position.x <= (targetTransform.transform.position.x + cam.x * cameraCorrectionThreshold)) || allignRight)
            {
                newX += ((targetTransform.transform.position.x + cam.x * targetPositionOnScreen) - transform.position.x) * 0.0375f;
                newX += targetTransform.GetComponent<Rigidbody2D>().velocity.x * 0.02f;
                allignRight = true;
            }
        }
        else if (targetTransform.GetComponent<Rigidbody2D>().velocity.x < 0)
        {
            allignRight = false;
            if ((transform.position.x >= (targetTransform.transform.position.x - cam.x * cameraCorrectionThreshold)) || allignLeft)
            {
                newX += ((targetTransform.transform.position.x - cam.x * targetPositionOnScreen) - transform.position.x) * 0.0375f;
                newX += targetTransform.GetComponent<Rigidbody2D>().velocity.x * 0.02f;
                allignLeft = true;
            }

        }
        else
        {
            allignLeft = allignRight = false;
            if (transform.position.x != targetTransform.transform.position.x)
            {
                newX += desvioX * 0.05f;
            }
        }
        if (targetTransform.GetComponent<Rigidbody2D>().velocity.y > 0)
        {
            allignDown = false;
            if ((transform.position.y <= (targetTransform.transform.position.y - cam.y * 0.35f)) || allignUp)
            {
                newY += desvioY * 0.05f;
                newY += targetTransform.GetComponent<Rigidbody2D>().velocity.y * 0.02f;
                allignUp = true;
            }
        }
        else if (targetTransform.GetComponent<Rigidbody2D>().velocity.y < 0)
        {
            allignUp = false;
            if ((transform.position.y >= (targetTransform.transform.position.y + cam.y * 0.35f)) || allignDown)
            {

                newY += desvioY * 0.05f;
                newY += targetTransform.GetComponent<Rigidbody2D>().velocity.y * 0.02f;
                allignDown = true;
            }
        }
        else
        {
            allignUp = allignDown = false;
            if (transform.position.y != targetTransform.transform.position.y)
            {
                newY += desvioY * 0.05f;
            }
        }




        Vector3 novaPosicao = new Vector3(newX, newY, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, novaPosicao, Time.time);

    }
}
*/