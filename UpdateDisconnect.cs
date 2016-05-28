using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class UpdateDisconnect : NetworkBehaviour {
    public GameObject disconnectButton;
	void Update () {
        if (!NetworkClient.active)
        {
            disconnectButton.SetActive(false);
        }
        else
        {
            disconnectButton.SetActive(true);
        }
	}
}
