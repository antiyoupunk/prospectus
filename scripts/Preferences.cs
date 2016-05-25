using UnityEngine;
using UnityEngine.Networking; //needed for remote prefs
using UnityEngine.UI;
using System.Collections;

public class Preferences : NetworkBehaviour {
    //set prefs
    //note: Storing refrences to objects in the staticUI.PanelList class
    public static bool isShowingPing;
    public static bool serverIP;
    public static bool serverPort; //we'll replace these later with cool last-used or favorites, it'll be awesome!

    private GameObject[] networkOptionsArray;

    void Awake()
    {
        //store all network options in an array so we can update them all at once
    }
    public void applyPrefs()
    {
        isShowingPing = getPingSetting();
    }
    public void updatePingSetting(bool pingSetting)
    {
        PlayerPrefs.SetInt("ShowingPing", pingSetting ? 1 : 0);
        applyPrefs(); //this setting updates immediately, cause why not?
        PanelList.pingUIPanel.SetActive(pingSetting); //since the script is no longer running, we'll have to update it in the prefs
    }
    public bool getPingSetting()
    {
        PanelList.pingUIPanel.SetActive(PlayerPrefs.GetInt("ShowingPing") == 1); //ping is a bit different since we don't want it happening at all if the player isn't interested.
        return PlayerPrefs.GetInt("ShowingPing") == 1;
        
    }

}
