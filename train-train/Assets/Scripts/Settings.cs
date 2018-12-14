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
    public static PicturePickerTarget picturePickerTarget;     public Toggle SymbolsToLettersToggle;

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

    public void Symbol_type_selected(System.Int32 newValue)
    {
        var symbols = new List<Symbol_>();

        switch (newValue)
        {
            case 0:
                symbols.AddRange(Data.currentProfile.selectedSymbols);
                break;
            case 1:
                for (int i = 1; i <= 7; i++) {
                    Data.currentProfile.symbols.Add(new Symbol_(i.ToString()));
                }
                break;

            case 2:
                symbols.Add(new Symbol_("a"));
                symbols.Add(new Symbol_("b"));
                symbols.Add(new Symbol_("c"));
                symbols.Add(new Symbol_("d"));
                symbols.Add(new Symbol_("e"));
                symbols.Add(new Symbol_("f"));
                symbols.Add(new Symbol_("g"));
                break;
        }

        Data.currentProfile.selectedSymbols = symbols;

    }

    public void OnToggleDoesGameEndClick(bool newValue)
    {
        Data.currentProfile.doesEnd = newValue;
        Debug.Log("[DEBUG: doesEnd after toggle change is " + Data.currentProfile.doesEnd + "]");
    }

    void Start()
    {
        picturePickerTarget = PicturePickerTarget.NOT_SELECTED;     }
}
