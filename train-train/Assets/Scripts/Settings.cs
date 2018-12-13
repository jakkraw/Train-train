using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PicturePickerTarget
{
    NOT_SELECTED,
    DRIVER,
    PASSENGER,
    STATION_SYMBOL
}

public class Settings : MonoBehaviour
{

    public Slider trainSpeedSlider;
    public Toggle doesGameEndToogle;
    public static PicturePickerTarget picturePickerTarget;

    public void onBackClick()
    {
        SceneManager.LoadScene("Menu");
    }

    public void onPassengerPicturePickClick()
    {
        picturePickerTarget = PicturePickerTarget.PASSENGER;
        SceneManager.LoadScene( "PicturePicker" );
    }

    public void onDriverPicturePickClick()
    {
        picturePickerTarget = PicturePickerTarget.DRIVER;
        SceneManager.LoadScene( "PicturePicker" );
    }

    public void onSymbolPicturePickClick()
    {
        picturePickerTarget = PicturePickerTarget.STATION_SYMBOL;
        SceneManager.LoadScene( "PicturePicker" );
    }

    public void Slider_Changed(float newValue)
    {
        Data.currentProfile.trainSpeed = newValue;
    }

    public void OnToggleDoesGameEndClick(bool newValue)
    {
        Data.currentProfile.doesEnd = newValue;
        Debug.Log("[DEBUG: doesEnd after toggle change is " + Data.currentProfile.doesEnd + "]");
    }

    void Start()
    {
        picturePickerTarget = PicturePickerTarget.NOT_SELECTED;
        // Try to find the desired GameObject.
        // This will only find active GameObjects in the scene.
        GameObject trainSpeedSlider_Object = GameObject.Find("Slider-TrainSpeed");
        if (trainSpeedSlider_Object != null)
        {
            // Get the Slider Component
            trainSpeedSlider = trainSpeedSlider_Object.GetComponent<Slider>();

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
                Debug.LogError("[" + trainSpeedSlider_Object.name + "] - Does not contain a Slider Component!");
            }

        }
        else
        {
            Debug.LogError("Could not find an active GameObject named Slider-TrainSpeed!");
        }


        GameObject doesGameEndToogle_Object = GameObject.Find("Game-ends-toggle");
        if (doesGameEndToogle_Object != null)
        {
            // Get the Slider Component
            doesGameEndToogle = doesGameEndToogle_Object.GetComponent<Toggle>();

            // If a Slider Component was found on the GameObject.
            if (doesGameEndToogle != null)
            {
                // This is a Conditional Statement. 
                // Basically if volumeLevel isn't null, 
                // then it uses it's value, 
                // otherwise it uses the DefaultVolumeLevel that we've set above.
                doesGameEndToogle.isOn = Data.currentProfile.doesEnd;
            }
            else
            {
                Debug.LogError("[" + doesGameEndToogle.name + "] - Does not contain a Toggle Component!");
            }

        }
        else
        {
            Debug.LogError("Could not find an active GameObject named Game-ends-toggle!");
        }
    }
}
