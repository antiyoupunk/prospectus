using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MainMenuNetworkController : MonoBehaviour {
    public NetworkManager netManager;
	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (NetworkClient.active || NetworkServer.active)
        {
            PanelList.mainMenu.SetActive(false);
        }
        else
        {
            PanelList.mainMenu.SetActive(true);
        }
    }
    public void startClient()
    {
        netManager.StartClient();
    }
    public void stopClient()
    {
        netManager.StopClient();
    }
}
