using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    public Slider trainSpeedSlider;

    public void onBackClick()
    {
        SceneManager.LoadScene("Menu");
    }


    public void Slider_Changed(float newValue)
    {
        Data.currentProfile.trainSpeed = newValue;
    }

    void Start()
    {
        // Try to find the desired GameObject.
        // This will only find active GameObjects in the scene.
        GameObject temp = GameObject.Find("Slider-TrainSpeed");
        if (temp != null)
        {
            // Get the Slider Component
            trainSpeedSlider = temp.GetComponent<Slider>();

            // If a Slider Component was found on the GameObject.
            if (trainSpeedSlider != null)
            {
                // This is a Conditional Statement. 
                // Basically if volumeLevel isn't null, 
                // then it uses it's value, 
                // otherwise it uses the DefaultVolumeLevel that we've set above.
                trainSpeedSlider.value = Data.currentProfile.trainSpeed;
            }
            else
            {
                Debug.LogError("[" + temp.name + "] - Does not contain a Slider Component!");
            }

        }
        else
        {
            Debug.LogError("Could not find an active GameObject named Slider-TrainSpeed!");
        }

    }
}
