using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerID : NetworkBehaviour {

    [SyncVar]
    private string playerUniqueID;
    private NetworkInstanceId playerNetID;

    public static GameObject myPlayer;

    public override void OnStartLocalPlayer()
    {
        GetNetID();
        SetID();
    }
    void Awake()
    {
        if (!isLocalPlayer)
            return;
        myPlayer = transform.parent.gameObject;
    }
    // Update is called once per frame
    void Update ()
    {
        if (isLocalPlayer)
        {
            if (transform.name == "" || transform.name == "Player(Clone)")
            {
                SetID();
            }
        }
	}
    [Client]
    void GetNetID()
    {
        playerNetID = GetComponent<NetworkIdentity>().netId;
        if(isLocalPlayer)
            CmdTellServerMyID(MakeUniqueID());
    }
    [Client]
    void SetID()
    {
        if (!isLocalPlayer)
        {
            transform.name = playerUniqueID;
        }else
        {
            transform.name = MakeUniqueID();
        }
    }
    string MakeUniqueID()
    {
        string uniqueID = "Player_" + playerNetID.ToString();
        return uniqueID;
    }
    [Command]
    void CmdTellServerMyID(string myID)
    {
        playerUniqueID = myID;
    }
}
