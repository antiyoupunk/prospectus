using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CameraController : NetworkBehaviour {
    private float smooth = 1f;
    private float maxLead = 7.0f;
    private float currentLead;
    private float yOffset = 6.0f;
    private float threshHold = .1f;
    private Vector3 lastPos, lastTarget, camVelocity, targetPos, magicCameraPos;
    private bool cameraLeadingRight = true;

    // Use this for initialization
    void Start () {
        targetPos = transform.position;
        currentLead = maxLead;
	}
	
	// Update is called once per frame
	void Update () {
        if (isServer)
        {
            Camera.main.orthographicSize = 100;
        }
        if (!isLocalPlayer)
            return; //only need to update the camera  for local players

        moveCamera();
    }

    void moveCamera()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        targetPos = transform.position;
        float xDelta = Mathf.Abs((targetPos - lastPos).x);

        

        if (cameraLeadingRight)//the camera is right of the character
        {
            if (targetPos.x > lastPos.x && xDelta > threshHold)//character still moving right
            {
                currentLead = maxLead;
            }
            else if (targetPos.x < lastPos.x && xDelta > threshHold)//character is moving backwards away from camera
            {
                
                currentLead = currentLead - .1f;
                if (currentLead < maxLead * 0.9)
                    cameraLeadingRight = false; //flip the camera
            }
        }
        else //the camera is left of the character
        {
            if (targetPos.x < lastPos.x && xDelta > threshHold) //character still moving left or standing still
            {
                currentLead = maxLead * -1;
            }
            else if (targetPos.x > lastPos.x && xDelta > threshHold) //character is moving backwards away from camera
            {
                currentLead = currentLead + .1f;
                if (currentLead > maxLead * -0.9)
                    cameraLeadingRight = true; //flip the camera
            }
        }
        //if the player is firing, get the mouse position and extend the camera as far as possible in that direction.
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (PlayerShoot.isFiring)
        {
            if(transform.position.x < mousePos.x)
            {
                currentLead = maxLead;
            }else
            {
                currentLead = maxLead * -1;
            }
        }
        targetPos.x = targetPos.x + currentLead;
        targetPos.z = -10;
        targetPos.y = targetPos.y + yOffset; //offset y should drop camera down some, to allow players to see where they are falling to
        Camera.main.transform.position = Vector3.SmoothDamp(cameraPos, targetPos, ref camVelocity, smooth);
        if (cameraPos.y > transform.position.y + 8)
        {
            Camera.main.transform.position = new Vector3(cameraPos.x, transform.position.y + 8, -10);
        }
        lastPos = transform.position;
    }
}
