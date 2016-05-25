using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class REMOVE_MainMenuServerScript : MonoBehaviour {
    public NetworkManager netManager;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void startLocalServer()
    {
        Debug.Log(netManager.StartServer());
        
    }
}
