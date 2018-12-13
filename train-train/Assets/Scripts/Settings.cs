using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PicturePickerTarget
{
    NOT_SELECTED,
    DRIVER,
    PASSENGER,
    STATION_SYMBOL
}

public class Settings : MonoBehaviour {
    public static PicturePickerTarget picturePickerTarget;

    void Start()
    {
        picturePickerTarget = PicturePickerTarget.NOT_SELECTED;
    }

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

}
