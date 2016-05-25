using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PanelList : MonoBehaviour {
    
    public static GameObject networkOptionsPanel;
    public static GameObject topOptionsPanel;

    public static GameObject mainMenu;
    public static GameObject pingUIPanel;

    private GameObject[] OptionsPanelArray;
    
    void Awake () {
        mainMenu = GameObject.Find("MainMenu");
        pingUIPanel = GameObject.Find("PingUI"); //the ping isn't a panel, because it's displayed and controled by the setting.


        networkOptionsPanel = GameObject.Find("NetworkOptionsPanel");
        topOptionsPanel = GameObject.Find("TopOptionsPanel");

        //build an array of options panels, we need to do this after storing each one since we'll be hiding those panels
        OptionsPanelArray = GameObject.FindGameObjectsWithTag("OptionsPanel");
        //once we've registered them all, we'll remove them until they're called   
        hideAllOptionsPanels();
    }
    void Update()
    {
        if (Input.GetKeyDown("escape"))
            openPanel(topOptionsPanel);
    }

    public void openOptions()
    {
        openPanel(topOptionsPanel);
    }

    public void hideAllOptionsPanels()
    {
        for(int i = 0; i < OptionsPanelArray.Length; i++)
        {
            OptionsPanelArray[i].SetActive(false);
        }
    }
    public void hidePanel(GameObject uiPanel)
    {
        uiPanel.SetActive(false);
    }
    public void openPanel(GameObject uiPanel)
    {
        hideAllOptionsPanels();
        uiPanel.SetActive(true);
    }
}
