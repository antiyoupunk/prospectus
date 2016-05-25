using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
//Server-authoritative movement with Client-side prediction, dead reconing and reconciliation
//we can advance this script by reviewing items here: https://github.com/GenaSG/UnityUnetMovement/blob/master/Scripts/NetworkMovement.cs
//channel #0: Reliable Sequenced
//channel #1: Unreliable Sequenced
[NetworkSettings(channel = 1, sendInterval = 0.05f)]
public class PlayerMove : NetworkBehaviour {
    
    //request these from server
    public float playerMaxSpeed;
    public float playerAgility;
    public float playerJumpStrength;
    public float positionThreshhold;


    private Transform myTransform;
    private Animator myAnimator;            // Reference to the player's animator component.
    private Rigidbody2D myBody;

    private int lerpRate = 15;
    private bool inputJump;
    private float xInput;

    public static bool isRightFacing, isMoving;  // For determining which way the player is currently facing.

    private float hSpeed, newSpeed;
    private Vector2 lastPos;
    private Vector3 lastPositionUpdate;
    private float lastPositionUpdateTime;

    public struct localPositionStructure
    {
        public Vector3 position;
        public Vector3 velocity;
        public float timestamp;
    }
    public struct syncPositionStructure
    {
        public Vector3 position;
        public Vector3 velocity;
        public float timeStamp;
    }
    [SyncVar] private syncPositionStructure syncPosition;   //Server-Side movement info to send to clients for reconciliation
    private localPositionStructure localPosition;           //local predictive movement info

    private void Awake()
    {
        // Setting up references.
        myTransform = GetComponent<Transform>();
        myAnimator = GetComponent<Animator>();
        myBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(!inputJump)
            inputJump = Input.GetKeyDown("space");
        xInput = Input.GetAxis("Horizontal");

        //we're going to update player's positions thusly, running this in fixed update would result in much slower movement
        //we only need new updates
        if (syncPosition.timeStamp <= lastPositionUpdateTime)
            return;

        lastPositionUpdateTime = syncPosition.timeStamp;
        if (!isServer)  
            lerpPos(); // updates non-player players, server should already know where they're at
        if (localPosition.timestamp <= syncPosition.timeStamp && isLocalPlayer)
        {
            reconcilePlayerPosition(syncPosition.position); //updates client position in case it's out of whack
        }

    }
    void FixedUpdate()
    {
        if (isLocalPlayer && !isServer)
        {
            CmdMovePlayer(myBody.velocity.x, xInput, inputJump, Time.time);
            predictPlayerMove(myBody.velocity.x, xInput, inputJump);
        }

        inputJump = false;

    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        isRightFacing = !isRightFacing;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    [Client]
    private void reconcilePlayerPosition(Vector3 validPosition)
    {
        //for now we just update the position, later we can smooth this out
        //we might need to match these positions to tiume stamps if this list is getting out of sync
        //changing the network sendrate may improve this drastically
        //for smoother but less accurate positioning: https://www.youtube.com/watch?v=mIeQhEhi1QQ

        //this is only sent over when localposition <= syncposition, that way we only compare coordinates which are equal in the simulation time
        if (Vector3.Distance(localPosition.position, syncPosition.position) > positionThreshhold) //something got out of what if this is true, the threshold covers minor time differences, but this should allow for lag without massive rubber-banding
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPosition.position, lerpRate * Time.deltaTime);
            myBody.velocity = syncPosition.velocity;
            Debug.Log("SHIT GOT WHACK! WE HAD TO MOVE THE PLAYER!!! - off by: " + Vector3.Distance(localPosition.position, syncPosition.position));
        }
    }
    [Client]
    private void lerpPos()
    {
        //update position of other players
        if (!isLocalPlayer) {
            //flip
            bool movingRight = (myBody.velocity.x > 0);
            if ((isRightFacing != movingRight) && (myBody.velocity.x != 0))
                Flip();
            myTransform.position = Vector3.Lerp(myTransform.position, syncPosition.position, lerpRate * Time.deltaTime);
            myBody.velocity = syncPosition.velocity;
            
        }
    }

    [Command(channel=0)] private void CmdMovePlayer(float currentSpeed, float xMoveForce, bool doJump, float timeStamp)
    {
        //finding my new position, and lerping here would likely be ideal.
        if (doJump)
            myBody.AddForce(new Vector2(myBody.velocity.x, playerJumpStrength));
        newSpeed = Mathf.Clamp(currentSpeed + (playerAgility * xMoveForce), playerMaxSpeed * -1, playerMaxSpeed);
        Vector3 newVelocity = new Vector3(newSpeed, myBody.velocity.y, 0.0f);
        myBody.velocity = newVelocity;
        hSpeed = Mathf.Round(myBody.velocity.x * 100f) / 100f;
        myAnimator.SetFloat("hSpeed", Mathf.Abs(hSpeed));
        //send position to clients
        syncPositionStructure tempPos;
            tempPos.position = myTransform.position;
            tempPos.velocity = newVelocity;
            tempPos.timeStamp = timeStamp;
        syncPosition = tempPos;
    } 

    //predictive - local only!
    private void predictPlayerMove(float currentSpeed, float xMoveForce, bool doJump)
    {
        //finding my new position, and lerping here would likely be ideal.
        if (doJump)
            myBody.AddForce(new Vector2(myBody.velocity.x, playerJumpStrength));
        newSpeed = Mathf.Clamp(currentSpeed + (playerAgility * xMoveForce), playerMaxSpeed * -1, playerMaxSpeed);
        Vector3 newVelocity = new Vector3(newSpeed, myBody.velocity.y, 0.0f);
        myBody.velocity = newVelocity;
        hSpeed = Mathf.Round(myBody.velocity.x * 100f) / 100f;
        myAnimator.SetFloat("hSpeed", Mathf.Abs(hSpeed));
        if ((isRightFacing && hSpeed < 0) || (!isRightFacing && hSpeed > 0))
        {
            Flip();
        }
        if (localPosition.timestamp <= syncPosition.timeStamp)
        {
            localPositionStructure tempPos;
            tempPos.position = myTransform.position;
            tempPos.velocity = newVelocity;
            tempPos.timestamp = Time.time;
            localPosition = tempPos;
        }

    }
}
