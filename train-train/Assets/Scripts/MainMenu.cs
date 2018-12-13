using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void onPlayClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void onSettingsClick()
    {
        SceneManager.LoadScene("Settings");
    }

    void Start()
    {
        Debug.Log("[DEBUG: trainSpeedValue at MainMenu open is " + Data.currentProfile.trainSpeed + "]");
    }

}
