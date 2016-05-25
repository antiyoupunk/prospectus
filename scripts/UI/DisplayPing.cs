using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class DisplayPing : NetworkBehaviour {
    private Text pingText;
    private NetworkManager nClient;

    private int step = 0;

    void Awake()
    {
        pingText = GameObject.Find("PingText").GetComponent<Text>();
        nClient = GameObject.Find("networkController").GetComponent<NetworkManager>();
    }

    void FixedUpdate () {
        step++;
        if (step > 20)
        {
            if (nClient.client != null && Preferences.isShowingPing)
                pingText.text = nClient.client.GetRTT().ToString();
        }
    }
}
